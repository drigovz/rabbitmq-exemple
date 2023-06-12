Na raiz do projeto teremos uma pasta chamada **.docker** que irá conter os arquivos Dockerfile do projeto, no caso dessa edição do RabbitMQ, nós precisaremos de uma versão modificada, com alguns plugins adicionais instalados, são eles:

- rabbitmq_shovel
- rabbitmq_shovel_management
- rabbitmq_delayed_message_exchange
- rabbitmq_consistent_hash_exchange


Teremos um arquivo chamado rabbitmq.dockerfile, com a seguinte configuração:

<code>
		FROM rabbitmq:3.11.16-management
		RUN apt-get update
		RUN apt-get install -y curl

		RUN curl -L https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/releases/download/v3.12.0/rabbitmq_delayed_message_exchange-3.12.0.ez > $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.12.0.ez
		RUN chown rabbitmq:rabbitmq $RABBITMQ_HOME/plugins/rabbitmq_delayed_message_exchange-3.12.0.ez

		RUN rabbitmq-plugins enable --offline rabbitmq_shovel
		RUN rabbitmq-plugins enable --offline rabbitmq_shovel_management
		RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange
		RUN rabbitmq-plugins enable --offline rabbitmq_consistent_hash_exchange
</code>

Teremos também uma pasta chamada **Common** e dentro dela, os projetos compartilhados entre todos os microsserviços. Nessa pasta teremos o projeto chamado **Rabbitmq.Helper**. Dentro desse projeto, teremos as pastas:

- **Clients**: contendo informações sobre as conexões do RabbitMQ.
- **Interfaces**: contendo as interfaces disponíveis para a injeção de dependência.
- **Utils**: contendo classes de utilidade em toda a aplicação do RabbitMQ.

Na pasta Cliente, teremos as classes:

- **Configure** - Que irá conter o método que irá criar a conexão com o RabbitMQ.

<code>
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
</code>


- **Connection** - Que irá de fato a conexão com o RabbitMQ.

<code>
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
</code>

















































