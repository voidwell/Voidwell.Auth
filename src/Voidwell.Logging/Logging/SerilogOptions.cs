namespace Voidwell.Common.Logging
{
    public class SerilogOptions
    {
        public string SerilogConnectionString { get; set; }
        public string SerilogTable { get; set; } = "\"ServiceLogs\"";
    }
}
