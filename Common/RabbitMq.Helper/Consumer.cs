namespace RabbitMq.Helper;

public class Consumer : IConsumer
{
	private readonly IModel _model;

	public Consumer(IConnection connection)
	{
		_model = connection.CreateModel();
	}

	public void Setup(QueueConfig queue, ExchangeConfig exchange, QueueConfig? deadLetterQueue = null, ExchangeConfig? deadLetterExchange = null)
	{
		Queue.Declare(_model, queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
		Exchange.Create(_model, exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);
		Queue.Bind(_model, queue.Name, exchange.Name, queue.RoutingKey);

		if (deadLetterQueue is not null && deadLetterExchange is not null)
			Queue.Bind(_model, deadLetterQueue.Name, deadLetterExchange.Name, deadLetterQueue.RoutingKey);
	}
}
