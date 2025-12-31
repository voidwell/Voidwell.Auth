using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace Voidwell.Auth;

public static class LoggingBuilderExtensions
{
    private const string _applicationName = "Voidwell.Auth";

    public static ILoggingBuilder AddServiceLogging(this ILoggingBuilder builder, IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", _applicationName);

        if (hostEnvironment.IsDevelopment())
        {
            loggerConfig.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}][{SourceContext}]{NewLine}{Message:lj}{NewLine}{Exception}");
        }
        else
        {
            loggerConfig.WriteTo.Console(new CompactJsonFormatter());
        }

        builder.AddSerilog(loggerConfig.CreateLogger());

        return builder;
    }
}
