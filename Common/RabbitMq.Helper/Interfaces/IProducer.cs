namespace RabbitMq.Helper.Interfaces;

public interface IProducer
{
    public void Send(object message, QueueConfig queue, ExchangeConfig exchange, QueueConfig deadLetterQueue = null, ExchangeConfig deadLetterExchange = null);
}