namespace RabbitMq.Helper;

internal static class Queue
{
	public static void Declare(IModel model, string queueName, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments) =>
		model.QueueDeclare(queue: queueName, durable: durable, exclusive: exclusive, autoDelete: autoDelete, arguments: arguments);

	public static void Bind(IModel model, string queueName, string exchangeName, string routingKey) =>
		model.QueueBind(queueName, exchangeName, routingKey);

	public static void Publish(IModel model, string exchangeName, string routingKey, byte[] message, IBasicProperties properties) =>
		model.BasicPublish(
			exchange: exchangeName,
			routingKey: routingKey,
			basicProperties: properties,
			body: message
		);
}
