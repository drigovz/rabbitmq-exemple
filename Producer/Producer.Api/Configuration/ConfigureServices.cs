namespace Producer.Api.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(BaseResponse).Assembly));

        services.AddScoped<NotificationContext>();
        services.AddControllers().AddFluentValidation(_ => _.RegisterValidatorsFromAssemblyContaining<NotificationContext>());

        var connectionString = configuration.GetConnectionString("RabbitMq");
        var rabbitMqConnection = Connection.Connect(connectionString, Consts.AppProviderName);
        services.AddSingleton(rabbitMqConnection);
        services.AddTransient<IProducer, RabbitMq.Helper.Producer>();
        
        return services;
    }
}