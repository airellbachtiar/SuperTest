using Microsoft.Extensions.Logging;

namespace SuperTestWPF.Logger
{
    public class LogEntry
    {
        public LogLevel Level { get; set; }
        public required string Message { get; set; }
    }
}
