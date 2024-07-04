namespace Shared.Utils;

public static class QueueExchangeObjects
{
    public static readonly QueueConfig AddPersonQueue =
        new()
        {
            Name = Consts.AddPersonQueueName,
            RoutingKey = Consts.AddPersonRoutingKey,
            Arguments = new Dictionary<string, object>
            {
                { "x-max-length", 6 },
                { "x-delay", 2500 },
                { "x-dead-letter-exchange", Consts.AddPersonExchangeNameDeadLetter },
                { "x-dead-letter-routing-key", Consts.AddPersonRoutingKey },
            }
        };

    public static readonly QueueConfig AddPersonQueueDeadLetter =
        new() { Name = Consts.AddPersonQueueNameDeadLetter, RoutingKey = Consts.AddPersonRoutingKey, };

    public static readonly ExchangeConfig AddPersonExchange =
        new()
        {
            Name = Consts.AddPersonExchangeName,
            Type = "x-delayed-message",
            Arguments = new Dictionary<string, object>
            {
                { "x-delayed-type", ExchangeType.Direct },
            }
        };

    public static readonly ExchangeConfig AddPersonExchangeDeadLetter =
        new() { Name = Consts.AddPersonExchangeNameDeadLetter, Type = ExchangeType.Direct, };
}
