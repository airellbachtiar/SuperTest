using System;
using System.Collections.Generic;
using Logger;

namespace Traffic.Logger
{
    public class LogEntry
    {
        public DateTime DateTime { get; set; }

        public int Index { get; set; }

        public string Message { get; set; } = "";

        public LogLevel Level { get; set; }
    }

    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; } = new();
    }
}
