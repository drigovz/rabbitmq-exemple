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
