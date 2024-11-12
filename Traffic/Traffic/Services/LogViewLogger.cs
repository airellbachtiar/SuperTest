using Logger;
using StatemachineFramework.Logging;
using StatemachineFramework.Logging.EventHandlers;
using LogLevel = Logger.LogLevel;

namespace Traffic.Services;

public class LogViewLogger : IStatemachineLogger
{
    private readonly ILogger _logger;

    public LogViewLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void LogState(StateChangeEventArgs e)
    {
        _logger.Log(LogLevel.Information, $"{e.Statemachine.Name}: {e.FromState.Name} -> {e.ToState.Name}");
    }

    public void LogStandardTransition(StandardTransitionEventArgs e) { }

    public void LogInterfaceTransition(InterfaceTransitionEventArgs e) { }

    public void LogInterfaceTransition(CompoundInterfaceTransitionEventArgs e) { }

    public void Flush() { }
}