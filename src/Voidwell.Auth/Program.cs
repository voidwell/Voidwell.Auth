using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog.Events;
using System;
using System.Collections.Generic;
using Voidwell.Auth;
using Voidwell.Common.Logging;

namespace Voidwell.VoidwellAuth.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000")
                .UseCommonLogging(new LoggingOptions
                {
                    IgnoreRules = new List<Func<LogEvent, bool>>
                    {
                        TokenValidationLoggingDegrader.DegradeEvents
                    }
                })
                .Build();

            host.Run();
        }
    }
}
