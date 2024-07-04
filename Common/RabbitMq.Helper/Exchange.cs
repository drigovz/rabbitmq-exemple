namespace RabbitMq.Helper;

internal static class Exchange
{
    public static void Create(IModel model, string exchangeName, string exchangeType, bool durable, bool autoDelete, IDictionary<string, object> arguments) =>
        model.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: durable, autoDelete: autoDelete, arguments: arguments);
}
