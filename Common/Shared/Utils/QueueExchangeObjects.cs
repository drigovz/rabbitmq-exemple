namespace Shared.Utils;

public static class QueueExchangeObjects
{
    public static readonly QueueConfig AddPersonQueue =
        new()
        {
            Name = Consts.AddPersonQueueName,
            RoutingKey = Consts.AddPersonRoutingKey,
            Arguments = new Dictionary<string, object>
            {
                //TODO: doc - número máximo de tentativas de reprocessamento das mensagens antes de elas entrarem na dead letter
                { "x-max-length", 6 },
                //TODO: doc - adicionar um atraso em milissegundos a cada mensagem da fila antes de elas serem processadas
                { "x-delay", 2500 },
                //TODO: documentar - para qual dead letter as mensagens irão 
                { "x-dead-letter-exchange", Consts.AddPersonExchangeNameDeadLetter },
                { "x-dead-letter-routing-key", Consts.AddPersonRoutingKey },
            }
        };

    public static readonly QueueConfig AddPersonQueueDeadLetter =
        new() { Name = Consts.AddPersonQueueNameDeadLetter, RoutingKey = Consts.AddPersonRoutingKey, };

    public static readonly ExchangeConfig AddPersonExchange = 
        new()
        {
            Name = Consts.AddPersonExchangeName,
            Type = "x-delayed-message",
            Arguments = new Dictionary<string, object>
            {
                //TODO: doc - Para conseguir utilizar o plugin Delayed Message, é preciso adicionar o argumento x-delayed-type com o valor direct. nos argumentos do exchange
                { "x-delayed-type", ExchangeType.Direct },
            }
        };
    
    public static readonly ExchangeConfig AddPersonExchangeDeadLetter = 
        new() { Name = Consts.AddPersonExchangeNameDeadLetter, Type = ExchangeType.Direct, };
}