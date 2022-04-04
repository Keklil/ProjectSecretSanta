using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace SecretSanta_Backend.Configuration
{
    public static class ServiceExtension
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigurePostgreSqlContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        }
    }
}
