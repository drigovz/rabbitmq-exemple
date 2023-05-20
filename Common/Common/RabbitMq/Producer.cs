namespace Common.RabbitMq;

public class Producer : IProducer
{
    private readonly IModel _model;

    public Producer(IConnection connection)
    {
        _model = connection.CreateModel();
    }

    private void CreateQueues(string queueName) =>
        _model.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

    private void CreateExchange(string exchangeName, string exchangeType) =>
        _model.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

    private void BindQueues(string exchangeName, string queueName, string routingKey) =>
        _model.QueueBind(queueName, exchangeName, routingKey);
    
    private void PublishMessage(string exchangeName, string routingKey, byte[] byteMessage) =>
        _model.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: byteMessage
        );

    public void Send(object message, string exchangeName, string queueName, string routingKey, string exchangeType = ExchangeType.Fanout)
    {
        var byteMessage = MessageUtilities.Serialize(message);
        
        CreateQueues(queueName);
        CreateExchange(exchangeName, exchangeType);
        BindQueues(exchangeName, queueName, routingKey);
        PublishMessage(exchangeName, routingKey, byteMessage);
    }
}