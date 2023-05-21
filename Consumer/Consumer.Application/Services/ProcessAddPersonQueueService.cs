using Consumer.Application.Core.Emails.Commands;

namespace Consumer.Application.Services;

public class ProcessAddPersonQueueService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IModel _model;
    private readonly string _exchangeName = Consts.AddPersonExchangeName;
    private readonly string _queueName = Consts.AddPersonQueueName;
    private readonly string _exchangeType = ExchangeType.Fanout;
    private readonly string _routingKey = Consts.AddPersonRoutingKey;

    public ProcessAddPersonQueueService(IConnection connection, IMediator mediator)
    {
        _model = connection.CreateModel();
        _model.BasicQos(0, 10, false);
        _mediator = mediator;
    }
    
    private void CreateQueues(string queueName) =>
        _model.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

    private void CreateExchange(string exchangeName, string exchangeType) =>
        _model.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

    private void BindQueues(string exchangeName, string queueName, string routingKey) =>
        _model.QueueBind(queueName, exchangeName, routingKey);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CreateQueues(_queueName);
        CreateExchange(_exchangeName, _exchangeType);
        BindQueues(_exchangeName, _queueName, _routingKey);

        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += ProcessMessages;

        _model.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
    }
    
    private async Task ProcessMessages(object sender, BasicDeliverEventArgs ea)
    {
        var message = MessageUtilities.Deserialize(ea);
        
        var sendEmailDto = JsonConvert.DeserializeObject<SendEmailDTO>(message);
        var request = new SendEmailCommand
        {
            Name = $"{sendEmailDto.FirstName} {sendEmailDto.LastName}",
            Body = "Exemple",
            Email = sendEmailDto.Email,
        };
        
        await _mediator.Send(request);
    }
}