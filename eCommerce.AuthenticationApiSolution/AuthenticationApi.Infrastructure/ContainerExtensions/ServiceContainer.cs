using AuthenticationApi.Application.Repository;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repository;
using eCommerce.SharedLibrary.ContainerExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.ContainerExtensions
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedServices<AuthenticationDbContext>(configuration, configuration["MySerilog:FileName"]);
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructureService(this IApplicationBuilder app)
        {
            app.UseSharedServices();
            return app;
        }
    }
}
