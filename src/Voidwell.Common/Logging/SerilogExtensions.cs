using System.Collections.Generic;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;
using Serilog;

namespace Voidwell.Common.Logging
{
    public static class SerilogExtensions
    {
        private static readonly IDictionary<string, ColumnWriterBase> _columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            {"\"RaiseDate\"", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            { "\"Level\"", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            {"\"Message\"", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            {"\"MessageTemplate\"", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            {"\"Exception\"", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            {"\"Properties\"", new LogEventSerializedColumnWriter(NpgsqlDbType.Json) },
            {"\"PropsTest\"", new PropertiesColumnWriter(NpgsqlDbType.Json) },
            {"\"Environment\"", new SinglePropertyColumnWriter("Environment", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
            {"\"Service\"", new SinglePropertyColumnWriter("Service", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
        };

        public static LoggerConfiguration WriteToDatabase(this LoggerConfiguration configuration, SerilogOptions options)
        {
            return configuration.WriteTo.PostgreSQL(options.SerilogConnectionString, options.SerilogTable, _columnWriters);
        }
    }
}
