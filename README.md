## Lib de Abstração do RabbitMQ
Na raiz do projeto teremos uma pasta chamada **.docker** que irá conter os arquivos Dockerfile do projeto, no caso dessa edição do RabbitMQ, nós precisaremos de uma versão customizada da imagem do RabbitMQ, com alguns plugins adicionais instalados. 

São eles:

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

- **rabbitmq_shovel** - É um plug-in para que o RabbitMQ transfira as mensagens de uma fila para outra. Utilizamos isso para caso seja necessário reprocessar algumas mensagens, nós podermos ter essas mensagens em uma fila específica para o reprocessamento.
- **rabbitmq_shovel_management** - UI para gerenciamento do Shovel dentro do RabbitMQ Manager.
- **rabbitmq_delayed_message_exchange** - No RabbitMQ existe uma feature chamada Delayed Messages. Ela serve para caso o processamento das mensagens de uma fila falhe, podermos jogá-las novamente na fila, mas informando que ela deve ser processada novamente apenas em um período de tempo no qual formos definir na própria configuração da fila, ou seja, informamos que desejamos reprocessar essas mensagens, somente daqui a 5 minutos, por exemplo, e não necessariamente agora.

Após isso, na raiz do projeto, nós teremos o nosso arquivo do **docker compose** que irá subir de fato uma instância do RabbitMQ utilizando a imagem customizada que acabamos de criar.

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

Na raiz do projeto teremos um arquivo **.env** contendo informações do RabbitMQ, como:

```.env
RABBITMQ_USERNAME= Nome de usuário para o login no RabbitMQ
RABBITMQ_PASSWORD= Senha de usuário para o login no RabbitMQ
RABBITMQ_DEFAULT_VHOST= Nome do Virtual Host que criaremos para a aplicação
```

Com isso, teremos a nossa instância do RabbitMQ pronta para uso, e podemos acessá-la através do endereço:
**localhost:15672/#/**

#### Common
Teremos também uma pasta chamada **Common** e dentro dela, os projetos compartilhados entre todos os microsserviços. Nessa pasta teremos o projeto chamado **Rabbitmq.Helper**. Dentro desse projeto, teremos as pastas:

- **Client**: contendo informações sobre as conexões do RabbitMQ.
- **Interfaces**: contendo as interfaces disponíveis para a injeção de dependência.
- **Utils**: contendo classes de utilidade em toda a aplicação do RabbitMQ.

##### Client
Na pasta Client, teremos as classes:

- **Configure** - Que irá conter o método que irá criar a conexão com o RabbitMQ, essa classe foi definida como internal devido a sua não necessidade de exposição para fora dessa Lib.

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

- **Connection** - Que irá criar de fato a conexão com o RabbitMQ (*Essa será a classe utilizada na classe Program dos projetos*).

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

##### Interfaces
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

##### Utils
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

Já na pasta raiz do projeto, nós temos as classes:

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

```C#
namespace RabbitMq.Helper;

internal static class Exchange
{
    public static void Create(IModel model, string exchangeName, string exchangeType, bool durable, bool autoDelete, IDictionary<string, object> arguments) =>
        model.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: durable, autoDelete: autoDelete, arguments: arguments);
}
```

**Consumer.cs** - Temos também a classe Consumer, essa classe é a implementação da interface **IConsumer**, e nela temos o método **Setup**, onde nele, iremos implementar a declaração de uma fila, a criação de um Exchange, o bind dessa fila com esse Exchange e também iremos verificar se uma *dead-letter-queue* deve ser implementada ou não.

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

**Producer.cs** - Por fim, temos a classe Producer que é a implementação da interface **IProducer**, nessa classe, temos o método **Send**, esse método recebe um objeto como mensagem, e também recebe os objetos de **QueueConfig**, **ExchangeConfig** e os objetos opcionais de dead letter. Da mesma forma que na classe **Consumer**, aqui também estaremos declarando uma fila, estaremos criando um exchange, estaremos fazendo o bind dessa fila com esse exchange, se tivermos configuração para dead letter, estaremos aplicando ela e por fim, realizaremos a publicação dessa mensagem com o método **Publish** da classe **Queue**.

```C#
namespace RabbitMq.Helper;

public class Producer : IProducer
{
    private readonly IModel _model;

    public Producer(IConnection connection)
    {
        _model = connection.CreateModel();
    }
    
    public void Send(object message, QueueConfig queue, ExchangeConfig exchange, QueueConfig? deadLetterQueue = null, ExchangeConfig? deadLetterExchange = null)
    {
        var byteMessage = Message.Serialize(message);

        Queue.Declare(_model, queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
        Exchange.Create(_model, exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);
        Queue.Bind(_model, queue.Name, exchange.Name, queue.RoutingKey);
        
        if (deadLetterQueue is not null && deadLetterExchange is not null)
        {
            Queue.Declare(_model, deadLetterQueue.Name, deadLetterQueue.Durable, deadLetterQueue.Exclusive, deadLetterQueue.AutoDelete, deadLetterQueue.Arguments);
            Exchange.Create(_model, deadLetterExchange.Name, deadLetterExchange.Type, deadLetterExchange.Durable, deadLetterExchange.AutoDelete, deadLetterExchange.Arguments);
            Queue.Bind(_model, deadLetterQueue.Name, deadLetterExchange.Name, deadLetterQueue.RoutingKey);
        }
        
        Queue.Publish(_model, exchange.Name, queue.RoutingKey, byteMessage, queue.BasicPublishProperties);
    }
}
```

## Producer - Implementação da  biblioteca auxiliar
Para a utilização dessa biblioteca, teremos na aplicação produtora de mensagens a inclusão nos serviços da nossa aplicação e a conexão com o RabbitMQ, que está na nossa biblioteca auxiliar:

```C#
var connectionString = configuration.GetConnectionString("RabbitMq");
var rabbitMqConnection = Connection.Connect(connectionString, Consts.AppProviderName);
services.AddSingleton(rabbitMqConnection);
services.AddTransient<IProducer, RabbitMq.Helper.Producer>();
```

Teremos também a inclusão no container de injeção de dependência do .NET Core a interface **IProducer** e a sua implementação presente na classe **Producer** ambos da nossa biblioteca auxiliar do RabbitMQ.

Por fim, teremos a utilização do nosso método **Send** do producer no Handler ou serviço no qual desejamos criar uma mensagem que enviaremos para o RabbitMQ.

```C#
private readonly IProducer _producer;

public AddPersonHandler(IProducer producer)
{
    _producer = producer;
}

...

var queueConfig = QueueExchangeObjects.AddPersonQueue;
var queueConfigDeadLetter = QueueExchangeObjects.AddPersonQueueDeadLetter;
var exchangeConfig = QueueExchangeObjects.AddPersonExchange;
var exchangeConfigDeadLetter = QueueExchangeObjects.AddPersonExchangeDeadLetter;

_producer.Send(result, queueConfig, exchangeConfig, queueConfigDeadLetter, exchangeConfigDeadLetter);	
```

No exemplo acima, estamos criando uma fila e um exchange normais, e uma fila e um exchange para serem seu dead letter. Percebe-se que estamos utilizando os objetos que estão presentes na classe **QueueExchangeObjects**, essa classe está em uma biblioteca comum a todos os microsserviços, e define a criação de objetos do tipo QueueConfig e ExchangeConfig para que possamos informar a nossa biblioteca de abstração do RabbitMQ como desejamos criar nossas filas e exchanges.

#### QueueExchangeObjects
Essa classe define como os objetos QueueConfig e ExchangeConfig devem ser criados, e pode possuir códigos semelhantes ao seguinte:

```C#
using RabbitMQ.Client;
using RabbitMq.Helper.Utils;

namespace Common.Utils;

public static class QueueExchangeObjects
{
    public static readonly QueueConfig AddPersonQueue =
        new()
        {
            Name = Consts.AddPersonQueueName,
            RoutingKey = Consts.AddPersonRoutingKey,
            Arguments = new Dictionary<string, object>
            {
                { "x-max-length", 6 },
                { "x-dead-letter-exchange", Consts.AddPersonExchangeNameDeadLetter },
                { "x-dead-letter-routing-key", Consts.AddPersonRoutingKey },
            }
        };

    public static readonly QueueConfig AddPersonQueueDeadLetter =
        new() { Name = Consts.AddPersonQueueNameDeadLetter, RoutingKey = Consts.AddPersonRoutingKey, };

    public static readonly ExchangeConfig AddPersonExchange = 
        new()
        {
            Name = Consts.AddPersonExchangeName,
            Type = "x-delayed-message",
            Arguments = new Dictionary<string, object>
            {
                { "x-delayed-type", ExchangeType.Direct },
            }
        };
    
    public static readonly ExchangeConfig AddPersonExchangeDeadLetter = 
        new() { Name = Consts.AddPersonExchangeNameDeadLetter, Type = ExchangeType.Direct, };
}
```

## Consumer - Implementação da  biblioteca auxiliar
Para o consumer, devemos primeiramente implementar a conexão do RabbitMQ e a interface **IConsumer** com a sua implementação **Consumer**.

```C#
var connectionString = configuration.GetConnectionString("RabbitMq");
var rabbitMqConnection = Connection.Connect(connectionString, Consts.AppProviderName);
services.AddSingleton(rabbitMqConnection);
services.AddHostedService<ProcessAddPersonQueueService>();
services.AddTransient<IConsumer, RabbitMq.Helper.Consumer>();
```

Após isso, devemos configurar o serviço que irá ficar escutando alguma fila do RabbitMQ como um serviço em background por meio do serviço AddHostedService. Com isso, devemos configurar o serviço que irá ficar escutando alguma fila do RabbitMQ como um serviço em background. Esse serviço está presente na classe **ProcessAddPersonQueueService**.

```C#
using Consumer.Application.Core.Emails.Commands;
using RabbitMq.Helper.Interfaces;
using RabbitMq.Helper.Utils;

namespace Consumer.Application.Services;

public class ProcessAddPersonQueueService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IConsumer _consumer;
    private readonly IModel _model;

    public ProcessAddPersonQueueService(IConnection connection, IMediator mediator, IConsumer consumer)
    {
        _model = connection.CreateModel();
        _model.BasicQos(0, 10, false);
        _mediator = mediator;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueConfig = QueueExchangeObjects.AddPersonQueue;
        var exchangeConfig = QueueExchangeObjects.AddPersonExchange;
        
        _consumer.Setup(queueConfig, exchangeConfig);

        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += ProcessMessages;

        _model.BasicConsume(queue: queueConfig.Name, autoAck: false, consumer: consumer);
    }
    
    private async Task ProcessMessages(object sender, BasicDeliverEventArgs ea)
    {
        var sendEmailDto = Message.Deserialize<SendEmailDTO>(ea);
        var request = new SendEmailCommand
        {
            Name = $"{sendEmailDto.FirstName} {sendEmailDto.LastName}",
            Body = "Exemple",
            Email = sendEmailDto.Email,
        };
        
        await _mediator.Send(request);
    }
}
```

Nesta classe nós temos a sobrescrita do método **ExecuteAsync**, nesse método, nós devemos chamar o método **Setup** da classe **Consumer**, perceba que nesse método, nós chamamos de forma funcional um outro método que criamos chamado **ProcessMessages** que será o método que nesse exemplo, estará chamando o Handler de envio de email para cada uma das mensagens que ele encontrar na fila.

Como percebemos, a classe que irá processar as mensagens, está herdando de uma outra classe chamada BackgroundService, essa classe também está em uma biblioteca de classes comuns a todos os microsserviços:

```C#
namespace Shared.Services;

public abstract class BackgroundService : IHostedService, IDisposable
{
    private Task _task;
    private readonly CancellationTokenSource _cancelationTokenSource = new();

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);
    
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _task = ExecuteAsync(_cancelationTokenSource.Token);
        return _task.IsCompleted ? _task : Task.CompletedTask;
    }
    
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _cancelationTokenSource.Cancel();
        }
        finally
        {
            await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
    
    public virtual void Dispose() => _cancelationTokenSource.Cancel();
}
```
Nesta classe, nós temos a assinatura do método **ExecuteAsync** ao qual nós sobreescrevemos na classe exibida anteriormente. Temos também o método **StartAsync** que será o método que ficará executando em background até que a task esteja completa. Temos o método **StopAsync** que será o método que irá matar essa task e por fim o método **Dispose** vindo da interface **IDispose** que irá limpar os recursos não utilizados.

Com isso, temos um serviço executando em background que irá escutar as mensagens de uma ou mais filas e executará alguma ação ou chamará algum serviço quando essas mensagens forem processadas.
