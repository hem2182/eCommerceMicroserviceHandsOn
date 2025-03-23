using eCommerce.SharedLibrary.ContainerExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Repository;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repository;

namespace ProductApi.Infrastructure.ContainerExtensions
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add database context, authentication, logging
            services.AddSharedServices<ProductDbContext>(configuration, configuration["MySerilog:FileName"]);

            // Register repository
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructureService(this IApplicationBuilder app)
        {
            // Register middleware such as: GlobalException, ListenToApiGatewayOnly.
            app.UseSharedServices();
            return app;
        }
    }
}
