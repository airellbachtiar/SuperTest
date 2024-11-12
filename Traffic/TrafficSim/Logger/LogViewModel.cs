using System.Collections.ObjectModel;
using System.Windows;
using Logger;

namespace TrafficSim.Logger;

public class LogViewModel : ILogger
{
    private int _index;
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    public double ScrollOffset { get; }

    public void Log(LogLevel level, string publicMessage, Exception? ex = null)
    {
        Application.Current.Dispatcher?.Invoke(() =>
        {
            LogEntries.Add(new LogEntry
            {
                DateTime = DateTime.Now,
                Index = _index++,
                Level = level,
                Message = publicMessage
            });
        });
    }

    public void Log(LogLevel level, string publicMessage, string privateMessage, Exception? ex = null)
    {
        Application.Current.Dispatcher?.Invoke(() =>
        {
            LogEntries.Add(new LogEntry
            {
                DateTime = DateTime.Now,
                Index = _index++,
                Level = level,
                Message = privateMessage ?? publicMessage
            });
        });
    }

    public void Log(LogLevel level, string format, object[] publicArgs, object[] privateArgs, Exception? ex = null)
    {
        Application.Current.Dispatcher?.Invoke(() =>
        {
            LogEntries.Add(new LogEntry
            {
                DateTime = DateTime.Now,
                Index = _index++,
                Level = level,
                Message = string.Format(format, privateArgs ?? publicArgs)
            });
        });
    }

    public void LogPrivate(LogLevel level, string privateMessage, Exception? ex = null)
    {
        Application.Current.Dispatcher?.Invoke(() =>
        {
            LogEntries.Add(new LogEntry
            {
                DateTime = DateTime.Now,
                Index = _index++,
                Level = level,
                Message = privateMessage
            });
        });
    }
}