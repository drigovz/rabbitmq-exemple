namespace Producer.Api.Configuration;

public static class ConfigureRepository
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
        );

        services.AddScoped(typeof(IBaseRepository<BaseEntity, Guid>), typeof(BaseRepository<BaseEntity, Guid>));
        services.AddScoped<IPersonRepository, PersonRepository>();

        return services;
    }
}