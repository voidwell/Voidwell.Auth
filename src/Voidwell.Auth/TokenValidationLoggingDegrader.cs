using Serilog;
using Serilog.Events;
using Serilog.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.Auth
{
    public static class TokenValidationLoggingDegrader
    {
        // This is very brittle and will work specifically for IdentityServer 1.5.0. 
        // Beyond that, this needs to be looked at.
        public static bool DegradeEvents(LogEvent logEvent)
        {
            if (!IsInvalidReferenceToken(logEvent))
            {
                return false;
            }

            var newLogEvent = new LogEvent(logEvent.Timestamp, LogEventLevel.Warning, logEvent.Exception,
                logEvent.MessageTemplate, logEvent.Properties.Select(a => new LogEventProperty(a.Key, a.Value)));

            Log.Write(newLogEvent);

            return true;
        }

        private static bool IsInvalidReferenceToken(LogEvent logEvent)
        {
            return logEvent.Level == LogEventLevel.Error
                && Matching.FromSource("IdentityServer4.Validation.TokenValidator")(logEvent)
                && MessagesToDegrade.Any(a => logEvent.MessageTemplate.Text.Contains(a));
        }

        private static IEnumerable<string> MessagesToDegrade
        {
            get
            {
                yield return "Invalid reference token.";
                yield return "Token expired.";
            }
        }
    }
}
