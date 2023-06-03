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