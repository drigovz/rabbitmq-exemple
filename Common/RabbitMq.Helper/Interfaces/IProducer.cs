namespace RabbitMq.Helper.Interfaces;

public interface IProducer
{
    public void Send(object message, QueueConfig queue, ExchangeConfig exchange);
}