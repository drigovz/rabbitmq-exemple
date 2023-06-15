Na raiz do projeto teremos uma pasta chamada **.docker** que irá conter os arquivos Dockerfile do projeto, no caso dessa edição do RabbitMQ, nós precisaremos de uma versão customizada da imagem do RabbitMQ, com alguns plugins adicionais instalados. São eles:

- rabbitmq_shovel
- rabbitmq_shovel_management
- rabbitmq_delayed_message_exchange
- rabbitmq_consistent_hash_exchange

Teremos um arquivo chamado **rabbitmq.dockerfile**, com a seguinte configuração:

```dockerfile
FROM rabbitmq:3.11.16-management
RUN apt-get update
RUN apt-get install -y curl

RUN curl -L https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/releases/download/v3.12.0/rabbitmq_delayed_message_exchange-3.12.0.ez > $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.12.0.ez
RUN chown rabbitmq:rabbitmq $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.12.0.ez

RUN rabbitmq-plugins enable --offline rabbitmq_shovel
RUN rabbitmq-plugins enable --offline rabbitmq_shovel_management
RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange
RUN rabbitmq-plugins enable --offline rabbitmq_consistent_hash_exchange
```

Os plugins instalados nessa versão customizada do RabbitMQ são necessários para:

- **rabbitmq_shovel** - É um plug-in para que o RabbitMQ transfira as mensagens de uma fila para outra. Utilizamos isso para caso seja necessário reprocessar algumas mensagens, nós poderemos ter essas mensagens em uma fila específica para o reprocessamento.
- **rabbitmq_shovel_management** - UI para gerenciamento do Shovel dentro do RabbitMQ Manager.
- **rabbitmq_delayed_message_exchange** - No RabbitMQ existe uma feature chamada Delayed Messages. Ela serve para caso o processamento das mensagens de uma fila falhe, podemos jogá-las novamente na fila, mas informando que ela deve ser processada novamente apenas daqui a x tempo.

Após isso, na raiz do projeto, nós teremos o nosso arquivo do docker compose que irá subir de fato uma instância do RabbitMQ utilizando a imagem customizada que acabamos de criar.


```dockerfile
version: "3.7"

services:
  rabbitmq:
    build:
      context: .
      dockerfile: ./.docker/rabbitmq.dockerfile
    container_name: rabbitmq
    user: root
    ports:
      - "5672:5672"
      - "15672:15672"
      - "25676:25676"
    networks:
      - dev-network
    volumes:
      - C:\containers\rabbitmq\data:/var/lib/rabbitmq/
      - C:\containers\rabbitmq\log:/var/log/rabbitmq
      - C:\containers\rabbitmq\mnesia:/var/lib/rabbitmq/mnesia
    environment:
      RABBITMQ_DEFAULT_USER: ${RABBITMQ_USERNAME}
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD}
      RABBITMQ_DEFAULT_VHOST: ${RABBITMQ_DEFAULT_VHOST}
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "-q", "ping"]
      interval: 5s
      timeout: 10s
      retries: 5

networks:
  dev-network:
    driver: bridge
```

Com isso, teremos a nossa instância do RabbitMQ pronta para uso, e podemos acessá-la através do endereço: **localhost:15672/#/**

### Common
Teremos também uma pasta chamada **Common** e dentro dela, os projetos compartilhados entre todos os microsserviços. Nessa pasta teremos o projeto chamado **Rabbitmq.Helper**. Dentro desse projeto, teremos as pastas:

- **Clients**: contendo informações sobre as conexões do RabbitMQ.
- **Interfaces**: contendo as interfaces disponíveis para a injeção de dependência.
- **Utils**: contendo classes de utilidade em toda a aplicação do RabbitMQ.

#### Clients
Na pasta Clients, teremos as classes:

- **Configure** - Que irá conter o método que irá criar a conexão com o RabbitMQ.

```C#
namespace RabbitMq.Helper.Client;

internal class Configure
{
	private readonly string _connectionString;
	private readonly string _providerName;

	public Configure(string connectionString, string providerName)
	{
		_connectionString = connectionString;
		_providerName = providerName;
	}

	public IConnection CreateConnection()
	{
		var factory = new ConnectionFactory
		{
			Uri = new Uri(_connectionString),
			ClientProvidedName = _providerName,
			DispatchConsumersAsync = true
		};

		return factory.CreateConnection();
	}
	}
```

- **Connection** - Que irá criar de fato a conexão com o RabbitMQ.

````C#
namespace RabbitMq.Helper.Client;

public static class Connection
{
	public static IConnection Connect(string connectionString, string providerName)
	{
		var connectionFactory = new Configure(connectionString, providerName);
		var connection = connectionFactory.CreateConnection();
		return connection;
	}
}
````

#### Interfaces
Na pasta Interfaces, nós temos os arquivos de interface de contrato **IConsumer** e **IProducer**.

**IConsumer.cs**

```C#
namespace RabbitMq.Helper.Interfaces;

public interface IConsumer
{
	public void Setup(QueueConfig queue, ExchangeConfig exchange, QueueConfig? deadLetterQueue = null, ExchangeConfig? deadLetterExchange = null);
}
```

**IProducer.cs**

```C#
namespace RabbitMq.Helper.Interfaces;

public interface IProducer
{
	public void Send(object message, QueueConfig queue, ExchangeConfig exchange, QueueConfig deadLetterQueue = null, ExchangeConfig deadLetterExchange = null);
}
```

#### Utils
Na pasta Utils, nós temos os arquivos de métodos e classes úteis para todo o projeto e implementação do RabbitMQ. Nessa pasta temos as classes:

**ExchangeConfig.cs** - Responsável por conter as propriedades de criação de um Exchange no RabbitMQ.

```C#
namespace RabbitMq.Helper.Utils;

public class ExchangeConfig
{
	public string Name { get; set; }
	public string Type { get; set; } = ExchangeType.Fanout;
	public bool Durable { get; set; } = false;
	public bool AutoDelete { get; set; } = false;
	public IDictionary<string, object>? Arguments { get; set; } = null;
}
```

**QueueConfig.cs** - Responsável por conter as propriedades de criação de uma fila no RabbitMQ.

```C#
namespace RabbitMq.Helper.Utils;

public class QueueConfig
{
    public string Name { get; set; }
    public string RoutingKey { get; set; }
    public bool Durable { get; set; } = false;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object>? Arguments { get; set; } = null;
    public IBasicProperties? BasicPublishProperties { get; set; } = null;
}
```

**Message.cs** - Contendo métodos úteis para a serialização e deserialização de mensagens para envio e recebimento do RabbitMQ.

```C#
namespace RabbitMq.Helper.Utils;

public static class Message
{
    public static string Deserialize(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        return Encoding.UTF8.GetString(body);
    }
    
    public static T? Deserialize<T>(BasicDeliverEventArgs eventArgs)
    {
        var json = Deserialize(eventArgs);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static byte[] Serialize(object message)
    {
        var serializedMessage = JsonConvert.SerializeObject(message);
        return Encoding.UTF8.GetBytes(serializedMessage);
    }
}
```


**Queue.cs** - Essa é a classe que contém os métodos de criação de uma Queue no RabbitMQ, nela fizemos o bind e a publicação de uma mensagem.

```C#
namespace RabbitMq.Helper;

internal static class Queue
{
    public static void Declare(IModel model, string queueName, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments) =>
        model.QueueDeclare(queue: queueName, durable: durable, exclusive: exclusive, autoDelete: autoDelete, arguments: arguments);

    public static void Bind(IModel model, string queueName, string exchangeName, string routingKey) =>
        model.QueueBind(queueName, exchangeName, routingKey);
    
    public static void Publish(IModel model, string exchangeName, string routingKey, byte[] message, IBasicProperties properties) =>
        model.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: message
        );
}
```

**Exchange.cs** - Essa é a classe que contém os métodos de criação de um Exchange no RabbitMQ, nela fizemos a criação de um Exchange.

**Consumer.cs** - Temos também a classe Consumer, essa classe é a implementação da interface **IConsumer**, e nela temos o método **Setup**, onde nele, iremos implementar a declaração de uma fila, a criação de um Exchange, o bind dessa fila com esse Exchange e também iremos verificar se uma dead-letter-queue deve ser implementada ou não.

```C#
namespace RabbitMq.Helper;

public class Consumer : IConsumer
{
    private readonly IModel _model;

    public Consumer(IConnection connection)
    {
        _model = connection.CreateModel();
    }
    
    public void Setup(QueueConfig queue, ExchangeConfig exchange, QueueConfig? deadLetterQueue = null, ExchangeConfig? deadLetterExchange = null)
    {
        Queue.Declare(_model, queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
        Exchange.Create(_model, exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);
        Queue.Bind(_model, queue.Name, exchange.Name, queue.RoutingKey);
        
        if (deadLetterQueue is not null && deadLetterExchange is not null)
            Queue.Bind(_model, deadLetterQueue.Name, deadLetterExchange.Name, deadLetterQueue.RoutingKey);
    }
}
```

**Producer.cs** - Por fim, temos a classe Producer que é a implementação da interface **Producer**, nessa classe, temos o método **Send**, esse método recebe um objeto como mensagem, e também recebe os objetos de **QueueConfig**, **ExchangeConfig** e os objetos opcionais de dead letter. Da mesma forma que na classe **Consumer**, aqui também estaremos declarando uma fila, estaremos criando um exchange, estaremos fazendo o bind dessa fila com esse exchange, se tivermos configuração para dead letter, estaremos aplicando ela e por fim, realizaremos a publicação dessa mensagem com o método **Publish** da classe **Queue**.

```C#


d