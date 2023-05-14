using Consumer.Application.Core;

namespace Consumer.Api.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(BaseResponse).Assembly));

        return services;
    }
}