namespace Producer.Api.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(BaseResponse).Assembly));

        services.AddScoped<NotificationContext>();
        services.AddControllers().AddFluentValidation(_ => _.RegisterValidatorsFromAssemblyContaining<NotificationContext>());
        
        return services;
    }
}