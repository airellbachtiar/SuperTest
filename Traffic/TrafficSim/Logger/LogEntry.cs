using Logger;

namespace TrafficSim.Logger
{
    public class LogEntry
    {
        public DateTime DateTime { get; set; }

        public int Index { get; set; }

        public string Message { get; set; } = string.Empty;

        public LogLevel Level { get; set; }
    }

    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; } = new();
    }
}
