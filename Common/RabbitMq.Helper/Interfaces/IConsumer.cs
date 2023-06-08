namespace RabbitMq.Helper.Interfaces;

public interface IConsumer
{
    public void Setup(QueueConfig queue, ExchangeConfig exchange, QueueConfig? deadLetterQueue = null, ExchangeConfig? deadLetterExchange = null);
}