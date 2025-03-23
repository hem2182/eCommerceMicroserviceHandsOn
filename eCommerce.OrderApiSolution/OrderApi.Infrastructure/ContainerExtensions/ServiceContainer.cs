using eCommerce.SharedLibrary.ContainerExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Repository;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repository;

namespace OrderApi.Infrastructure.ContainerExtensions
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedServices<OrderDbContext>(configuration, configuration["MySerilog:FileName"]);
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructureService(this IApplicationBuilder app)
        {
            app.UseSharedServices();
            return app;
        }
    }
}
