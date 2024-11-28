using Microsoft.Extensions.Logging;

namespace SuperTestLibrary.Logger
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] [{LogLevel}] {Category}: {Message}";
        }
    }
}
