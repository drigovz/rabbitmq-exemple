namespace RabbitMq.Helper.Utils;

public class QueueConfig
{
    public string Name { get; set; }
    public string RoutingKey { get; set; }
    public bool Durable { get; set; } = false;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object>? Arguments { get; set; } = null;
    public IBasicProperties? BasicPublishProperties { get; set; } = null;
}
