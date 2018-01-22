﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Filters;
using System;
using Voidwell.Common.Configuration;

namespace Voidwell.Common.Logging
{
    public class ConfiguredLoggerFactory : ILoggerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SerilogLoggerProvider _provider;

        public ConfiguredLoggerFactory(IServiceProvider serviceProvider, LoggingOptions loggingOptions, ILogEventEnricher[] enrichers)
        {
            _serviceProvider = serviceProvider;
            var config = CreateLoggerConfiguration(loggingOptions, enrichers);

            _provider = new SerilogLoggerProvider(config.CreateLogger(), false);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            SelfLog.WriteLine("Ignoring added logger provider {0}", provider);
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return _provider.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            _provider.Dispose();
        }

        private LoggerConfiguration CreateLoggerConfiguration(LoggingOptions loggingOptions,
             ILogEventEnricher[] enrichers)
        {
            var loggerConfig = new LoggerConfiguration()
                  .MinimumLevel.Is(loggingOptions.MinLogLevel)
                  .Enrich.FromLogContext()
                  .Enrich.WithProperty("ContainerId", Environment.MachineName)
                  .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("Environment"))
                  .Enrich.With(enrichers)
                  .Destructure.With<JTokenDestructuringPolicy>();

            if (!loggingOptions.IncludeMicrosoftInformation)
            {
                loggerConfig = loggerConfig.Filter.ByExcluding(FromMicrosoftInformation);
            }

            loggingOptions?.IgnoreRules?.ForEach(rule => loggerConfig = loggerConfig.Filter.ByExcluding(rule));

            var serviceProperties = _serviceProvider.GetService<ServiceProperties>();

            if (serviceProperties == null)
            {
                SelfLog.WriteLine("Service Properties not configured in DI. Cannot get application name");
            }
            else
            {
                loggerConfig = loggerConfig.Enrich.WithProperty("Service", serviceProperties.Name);
            }

            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();

            if (httpContextAccessor == null)
            {
                SelfLog.WriteLine("HttpContextAccessor not configured in DI. Cannot adding ClaimsPrincipal logging");
            }
            else
            {
                loggerConfig = loggerConfig.Enrich.With(new ClaimsPrincipalEnricher(httpContextAccessor));
            }

            loggerConfig.WriteTo.Console(outputTemplate: "[{Level:u4} {Timestamp:HH:mm:ss.fff}] {SourceContext}{NewLine}{Message} {Exception}{NewLine}");


            var serilogConnectionString = Environment.GetEnvironmentVariable("SerilogConnectionString");
            if (serilogConnectionString != null)
            {
                var serilogOptions = new SerilogOptions
                {
                    SerilogConnectionString = serilogConnectionString
                };
                //loggerConfig.WriteToDatabase(serilogOptions);
            }

            return loggerConfig;
        }

        private static bool FromMicrosoftInformation(LogEvent evt)
        {
            return evt.Level == LogEventLevel.Information
                && Matching.FromSource("Microsoft")(evt)
                && !Matching.FromSource<Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.ValidateAntiforgeryTokenAuthorizationFilter>()(evt);
        }
    }
}
