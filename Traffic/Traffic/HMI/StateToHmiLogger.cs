using Microsoft.AspNetCore.SignalR;
using StatemachineFramework.Logging;
using StatemachineFramework.Logging.EventHandlers;

namespace Traffic.HMI
{
    internal class StateToHmiLogger : IStatemachineLogger
    {


        public StateToHmiLogger(IServiceProvider serviceProvider)
        {
          
        }

        public void LogState(StateChangeEventArgs e)
        {
            var componentName = e.ParentComponent.Name;
            var stateName = e.ToState.Name;
            SendStateToClients(componentName, stateName);
        }

        private async void SendStateToClients(string componentName, string state)
        {
           
        }

        public void LogStandardTransition(StandardTransitionEventArgs e)
        {
        }

        public void LogInterfaceTransition(InterfaceTransitionEventArgs e)
        {
        }

        public void LogInterfaceTransition(CompoundInterfaceTransitionEventArgs e)
        {
        }

        public void Flush()
        {
        }
    }
}
