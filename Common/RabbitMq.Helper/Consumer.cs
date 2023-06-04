namespace RabbitMq.Helper;

public class Consumer : IConsumer
{
    private readonly IModel _model;

    public Consumer(IConnection connection)
    {
        _model = connection.CreateModel();
    }
    
    public void Setup(QueueConfig queue, ExchangeConfig exchange)
    {
        Queue.Declare(_model, queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
        Exchange.Create(_model, exchange.Name, exchange.Type);
        Queue.Bind(_model, queue.Name, exchange.Name, queue.RoutingKey);
    }
}