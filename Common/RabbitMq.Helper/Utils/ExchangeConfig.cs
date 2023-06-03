namespace RabbitMq.Helper.Utils;

public class ExchangeConfig
{
    public string Name { get; set; }
    public string Type { get; set; } = ExchangeType.Fanout;
}