namespace Common.RabbitMq;

public class Producer : IProducer
{
    private readonly IConnection _connection;

    public Producer(IConnection connection)
    {
        _connection = connection;
    }

    public void Send(object message, string exchangeName, string queueName, string routingKey, string exchangeType = ExchangeType.Fanout)
    {
        var serializedMessage = JsonSerializer.Serialize(message);
        var byteMessage = Encoding.UTF8.GetBytes(serializedMessage);
        
        using var channel = _connection.CreateModel();
        
        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

        // bind queue with exchange 
        channel.QueueBind(queueName, exchangeName, routingKey);

        // publish message 
        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: byteMessage
        );
    }
}