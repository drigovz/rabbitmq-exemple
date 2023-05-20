namespace Common.Utils;

public static class MessageUtilities
{
    public static string Deserialize(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        return Encoding.UTF8.GetString(body);
    }

    public static byte[] Serialize(object message)
    {
        var serializedMessage = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(serializedMessage);
    }
}