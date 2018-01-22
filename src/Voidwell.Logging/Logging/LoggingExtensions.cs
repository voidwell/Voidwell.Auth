using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;

namespace Voidwell.Common.Logging
{
    public static class LoggingExtensions
    {
        public static IWebHostBuilder UseCommonLogging(this IWebHostBuilder builder, params ILogEventEnricher[] enrichers)
        {
            return UseCommonLogging(builder, new LoggingOptions(), enrichers);
        }

        public static IWebHostBuilder UseCommonLogging(this IWebHostBuilder builder, LoggingOptions loggingOptions, params ILogEventEnricher[] enrichers)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices(services => {
                services.AddSingleton<ILoggerFactory>(provider => new ConfiguredLoggerFactory(provider, loggingOptions, enrichers));
            });

            return builder;
        }
    }
}
