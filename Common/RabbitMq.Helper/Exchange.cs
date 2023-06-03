namespace RabbitMq.Helper;

internal static class Exchange
{
    public static void Create(IModel model, string exchangeName, string exchangeType) =>
        model.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
}