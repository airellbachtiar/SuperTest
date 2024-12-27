using Microsoft.Extensions.Logging;
using Moq;

namespace SuperTestWPF.UnitTests.Helper
{
    public static class LoggerHelper
    {
        public static bool VerifyLog<TState>(Mock<ILogger<TState>> loggerMock, LogLevel expectedLogLevel, string expectedMessage)
        {
            return loggerMock.Invocations.Any(invocation =>
            {
                var logLevel = (LogLevel)invocation.Arguments[0];
                var state = invocation.Arguments[2] as IReadOnlyList<KeyValuePair<string, object>>;

                return logLevel == expectedLogLevel && state != null && state.Any(kv => kv.Value.ToString() == expectedMessage);
            });
        }
    }
}
