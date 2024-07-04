namespace RabbitMq.Helper.Utils;

public class ExchangeConfig
{
	public string Name { get; set; }
	public string Type { get; set; } = ExchangeType.Fanout;
	public bool Durable { get; set; } = false;
	public bool AutoDelete { get; set; } = false;
	public IDictionary<string, object>? Arguments { get; set; } = null;
}
