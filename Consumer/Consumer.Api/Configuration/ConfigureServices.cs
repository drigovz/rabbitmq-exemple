namespace Consumer.Api.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(BaseResponse).Assembly));
        
        var connectionString = configuration.GetConnectionString("RabbitMq");
        var rabbitMqConnection = Connection.Connect(connectionString, Consts.AppProviderName);
        services.AddSingleton(rabbitMqConnection);
        services.AddHostedService<ProcessAddPersonQueueService>();
        services.AddTransient<IConsumer, RabbitMq.Helper.Consumer>();

        return services;
    }
}