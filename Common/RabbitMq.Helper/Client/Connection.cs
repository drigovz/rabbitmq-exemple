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
