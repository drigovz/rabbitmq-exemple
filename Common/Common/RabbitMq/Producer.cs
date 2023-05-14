namespace Common.RabbitMq;

public class Producer : IProducer
{
    private readonly IModel _model;

    public Producer(IConnection connection)
    {
        _model = connection.CreateModel();
    }
    
    private void CreateQueues(string queueName)
    {
        _model.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    private void CreateExchange(string exchangeName, string exchangeType)
    {
        _model.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
    }

    private void BindQueues(string exchangeName, string queueName, string routingKey)
    {
        _model.QueueBind(queueName, exchangeName, routingKey);
    }
    
    public void Send(object message, string exchangeName, string queueName, string routingKey, string exchangeType = ExchangeType.Fanout)
    {
        var serializedMessage = JsonSerializer.Serialize(message);
        var byteMessage = Encoding.UTF8.GetBytes(serializedMessage);
        
        CreateQueues(queueName);
        CreateExchange(exchangeName, exchangeType);
        BindQueues(exchangeName, queueName, routingKey);
        
        _model.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: byteMessage
        );
    }
}