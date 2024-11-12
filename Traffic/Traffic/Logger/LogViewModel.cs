using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Logger;
using Traffic.Logger;

namespace Traffic.Logger;

public class LogViewModel : ILogger
{
    private int _index;
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    public double ScrollOffset { get; }

    public void Log(LogLevel level, string publicMessage, Exception? ex = null)
    {
        Debug.WriteLine(publicMessage, ex);
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
        Debug.WriteLine(publicMessage, ex);

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
        Debug.WriteLine(string.Format(format, privateArgs ?? publicArgs), ex);

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
        Debug.WriteLine(privateMessage, ex);

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