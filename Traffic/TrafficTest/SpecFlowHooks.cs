using Microsoft.Extensions.DependencyInjection;
using TrafficSim.Testing;

namespace TrafficTest
{
    [Binding]
    public class SpecFlowHooks
    {
        private static ServiceProvider _serviceProvider;
        public static ITestAccess TestAccess { get; private set; }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // Set up the service provider and register TestAccess
            _serviceProvider = TestServiceProvider.ConfigureServices();
            TestAccess = _serviceProvider.GetService<ITestAccess>();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _serviceProvider.Dispose();
        }
    }
}
