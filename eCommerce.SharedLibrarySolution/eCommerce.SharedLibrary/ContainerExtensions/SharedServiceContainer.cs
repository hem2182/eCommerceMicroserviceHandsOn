using eCommerce.SharedLibrary.MIddlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.ContainerExtensions
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration configuration, 
            string fileName) where TContext: DbContext
        {
            // 1. Add DbContext
            services.AddDbContext<TContext>(options => options.UseSqlServer(configuration.GetConnectionString("eCommerceDefaultConnection"), 
                sqlServerOptionsAction => sqlServerOptionsAction.EnableRetryOnFailure()));

            // 2. Add Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.File(
                    path: $"{fileName}-.text", 
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();

            // 3. Add authentication scheme
            services.AddJwtAuthenticationScheme(configuration);
            //JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedServices(this IApplicationBuilder app)
        {
            // use global exception
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // block all outside api calls directly.
            // TODO: Uncomment post creating API Gateway.
            // app.UseMiddleware<ListenToOnlyApiGatewayMiddleware>();

            return app;
        }
    }
}
