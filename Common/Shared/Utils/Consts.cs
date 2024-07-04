namespace Shared.Utils;

public static class Consts
{
    public const string AppProviderName = "app:person:creation";

    public const string AddPersonExchangeName = "Person.Service";
    public const string AddPersonExchangeNameDeadLetter = "Person.Service.DLX";

    public const string AddPersonQueueName = "Person.Created.Queue";
    public const string AddPersonQueueNameDeadLetter = "Person.Created.Queue.DLX";

    public const string AddPersonRoutingKey = "Person.RK";
}
