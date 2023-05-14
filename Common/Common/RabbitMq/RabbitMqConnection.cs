namespace Common.RabbitMq;

public static class RabbitMqConnection
{
    public static IConnection Connect(string connectionString)
    {
        var connectionFactory = new RabbitMqConfig(connectionString);
        var connection = connectionFactory.CreateConnection();
        return connection;
    }
}