namespace RabbitMq.Helper;

public class Producer : IProducer
{
    private readonly IModel _model;

    public Producer(IConnection connection)
    {
        _model = connection.CreateModel();
    }
    
    public void Send(object message, QueueConfig queue, ExchangeConfig exchange)
    {
        var byteMessage = Message.Serialize(message);

        Queue.Declare(_model, queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
        Exchange.Create(_model, exchange.Name, exchange.Type);
        Queue.Bind(_model, queue.Name, exchange.Name, queue.RoutingKey);
        Queue.Publish(_model, exchange.Name, queue.RoutingKey, byteMessage, queue.BasicPublishProperties);
    }
}