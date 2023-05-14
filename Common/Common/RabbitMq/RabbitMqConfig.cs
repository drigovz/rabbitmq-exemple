namespace Common.RabbitMq;

public class RabbitMqConfig
{
    private readonly string _connectionString;

    public RabbitMqConfig(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_connectionString),
            ClientProvidedName = "app:person:publisher",
            DispatchConsumersAsync = true
        };

        return factory.CreateConnection();
    }
}