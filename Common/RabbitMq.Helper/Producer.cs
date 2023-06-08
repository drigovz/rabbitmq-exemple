namespace RabbitMq.Helper;

public class Producer : IProducer
{
    private readonly IModel _model;

    public Producer(IConnection connection)
    {
        _model = connection.CreateModel();
    }
    
    public void Send(object message, QueueConfig queue, ExchangeConfig exchange, QueueConfig? deadLetterQueue = null, ExchangeConfig? deadLetterExchange = null)
    {
        var byteMessage = Message.Serialize(message);

        Queue.Declare(_model, queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
        Exchange.Create(_model, exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);
        Queue.Bind(_model, queue.Name, exchange.Name, queue.RoutingKey);
        Queue.Publish(_model, exchange.Name, queue.RoutingKey, byteMessage, queue.BasicPublishProperties);
        
        if (deadLetterQueue is not null && deadLetterExchange is not null)
            Queue.Bind(_model, deadLetterQueue.Name, deadLetterExchange.Name, deadLetterQueue.RoutingKey);
    }
}