using RabbitMq.Helper.Utils;

namespace Common.Utils;

public static class QueueExchangeObjects
{
    public static readonly QueueConfig AddPersonQueueConfig = new QueueConfig { Name = Consts.AddPersonQueueName, RoutingKey = Consts.AddPersonRoutingKey };
    public static readonly ExchangeConfig AddPersonExchangeConfig = new ExchangeConfig { Name = Consts.AddPersonExchangeName, };
}