namespace Common.RabbitMq;

public interface IProducer
{
    public void Send(object message, string exchangeName, string queueName, string routingKey, string exchangeType = ExchangeType.Fanout);
}