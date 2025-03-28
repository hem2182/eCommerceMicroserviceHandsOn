﻿using eCommerce.SharedLibrary.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.ContainerExtensions
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IOrderService, OrderService>(options => {
                options.BaseAddress = new Uri(configuration["ApiGateway:BaseAddress"]);
                options.Timeout = TimeSpan.FromSeconds(2);
            });

            var retryStrategy = new RetryStrategyOptions() 
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant, 
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = (args) => {
                    string message = $"OnRetry, AttemptNumber: {args.AttemptNumber} Outcomme: {args.Outcome}";
                    LogException.LogToConsole(message);
                    LogException.LogToDebugger(message);
                    return ValueTask.CompletedTask;
                }
            };

            services.AddResiliencePipeline("my-retry-pipeline", builder => { builder.AddRetry(retryStrategy); });
            
            return services;
        }
    }
}
