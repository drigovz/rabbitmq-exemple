namespace RabbitMq.Helper;

internal static class Exchange
{
    public static void Create(IModel model, string exchangeName, string exchangeType, bool durable, bool autoDelete, IDictionary<string, object> arguments) =>
        model.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: durable, autoDelete: autoDelete, arguments: arguments);
}

/*var exchangeArgumets = new Dictionary<string, object>
    {
        { "x-delayed-type", "header" }
    };
    channel.ExchangeDeclare(exchange: "MyExchangeName", type: "x-delayed-message", durable: true, arguments: exchangeArgumets);*/