using Consumer.Application.Core.Emails.Commands;
using RabbitMq.Helper.Interfaces;
using RabbitMq.Helper.Utils;

namespace Consumer.Application.Services;

public class ProcessAddPersonQueueService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IConsumer _consumer;
    private readonly IModel _model;

    public ProcessAddPersonQueueService(IConnection connection, IMediator mediator, IConsumer consumer)
    {
        _model = connection.CreateModel();
        _model.BasicQos(0, 10, false);
        _mediator = mediator;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueConfig = QueueExchangeObjects.AddPersonQueueConfig;
        var exchangeConfig = QueueExchangeObjects.AddPersonExchangeConfig;
        
        _consumer.Setup(queueConfig, exchangeConfig);

        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += ProcessMessages;

        _model.BasicConsume(queue: queueConfig.Name, autoAck: false, consumer: consumer);
    }
    
    private async Task ProcessMessages(object sender, BasicDeliverEventArgs ea)
    {
        var message = Message.Deserialize(ea);
        
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