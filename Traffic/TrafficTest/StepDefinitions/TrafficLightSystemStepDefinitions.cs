using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestBus;
using TechTalk.SpecFlow;

namespace TrafficTest.Steps
{
    [Binding]
    public class TrafficLightSteps
    {
        private readonly TestSim.TestSimClient _client;

        public TrafficLightSteps()
        {
            _client = SpecFlowHooks.Client;
        }

        [Given(@"the traffic light system is off")]
        public async Task GivenTheTrafficLightSystemIsOff()
        {
            var response = await _client.GetCarRedLightStateAsync(new Empty());
            response.LightState.Should().Be("Off");

            var pedestrianResponse = await _client.GetPedestrianRedLightStateAsync(new Empty());
            pedestrianResponse.LightState.Should().Be("Off");
        }

        [Given(@"the traffic light system is not started")]
        public async Task GivenTheTrafficLightSystemIsNotStarted()
        {
            var carResponse = await _client.GetCarRedLightStateAsync(new Empty());
            carResponse.LightState.Should().Be("Off");

            var pedestrianResponse = await _client.GetPedestrianRedLightStateAsync(new Empty());
            pedestrianResponse.LightState.Should().Be("Off");
        }

        [Given(@"the traffic light system has started")]
        public async Task GivenTheTrafficLightSystemHasStarted()
        {
            WhenTheUserPressesTheStartButton();
        }

        [When(@"the user presses the start button")]
        public void WhenTheUserPressesTheStartButton()
        {
            SpecFlowHooks.ClickButton("StartButton");
        }

        [When(@"the user starts the traffic light system")]
        public void WhenTheUserStartsTheTrafficLightSystem()
        {
            WhenTheUserPressesTheStartButton();
            Thread.Sleep(500);
            SpecFlowHooks.ClickButton("StopButton");
        }

        [When(@"(\d+) second(?:s)? \b(?:has|have)\b passed")]
        public async Task WhenSecondsHasPassed(int seconds)
        {
            await Task.Delay(seconds * 1000);
            SpecFlowHooks.ClickButton("StopButton");
        }

        [Then(@"the traffic light system should start")]
        public async Task ThenTheTrafficLightSystemShouldStart()
        {
            Thread.Sleep(200);
            var response = await _client.GetCarGreenLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"all traffic lights should be off")]
        public async Task ThenAllTrafficLightsShouldBeOff()
        {
            var carResponse = await _client.GetCarRedLightStateAsync(new Empty());
            carResponse.LightState.Should().Be("Off");

            var pedestrianResponse = await _client.GetPedestrianRedLightStateAsync(new Empty());
            pedestrianResponse.LightState.Should().Be("Off");
        }

        [Then(@"all pedestrian lights should be off")]
        public async Task ThenAllPedestrianLightsShouldBeOff()
        {
            var response = await _client.GetPedestrianRedLightStateAsync(new Empty());
            response.LightState.Should().Be("Off");
        }

        [Then(@"the car's green light should turn on")]
        public async Task ThenTheCarSGreenLightShouldTurnOn()
        {
            var response = await _client.GetCarGreenLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the pedestrian red light should turn on")]
        public async Task ThenThePedestrianRedLightShouldTurnOn()
        {
            var response = await _client.GetPedestrianRedLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the car's green light should switch to yellow")]
        public async Task ThenTheCarSGreenLightShouldSwitchToYellow()
        {
            var response = await _client.GetCarYellowLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the pedestrian red light should remain on")]
        public async Task ThenThePedestrianRedLightShouldRemainOn()
        {
            var response = await _client.GetPedestrianRedLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the car's yellow light should switch to red")]
        public async Task ThenTheCarSYellowLightShouldSwitchToRed()
        {
            var response = await _client.GetCarRedLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the pedestrian green light should turn on")]
        public async Task ThenThePedestrianGreenLightShouldTurnOn()
        {
            var response = await _client.GetPedestrianGreenLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the pedestrian light should switch to red")]
        public async Task ThenThePedestrianLightShouldSwitchToRed()
        {
            var response = await _client.GetPedestrianRedLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the car green light should turn back on")]
        public async Task ThenTheCarGreenLightShouldTurnBackOn()
        {
            var response = await _client.GetCarGreenLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the following events should occur in order:")]
        public async Task ThenTheFollowingEventsShouldOccurInOrder(Table table)
        {
            foreach (var row in table.Rows)
            {
                int time = int.Parse(row["Time (seconds)"]);
                string carLight = row["Car Light"];
                string pedestrianLight = row["Pedestrian Light"];

                await WhenSecondsHasPassed(time);

                var carResponse = await GetLightStateAsync(carLight, true);
                carResponse.Should().Be("On");

                var pedestrianResponse = await GetLightStateAsync(pedestrianLight, false);
                pedestrianResponse.Should().Be("On");
            }
        }

        private async Task<string> GetLightStateAsync(string light, bool isCar)
        {
            LightResponse response;
            switch (light)
            {
                case "Green":
                    response = isCar
                        ? await _client.GetCarGreenLightStateAsync(new Empty())
                        : await _client.GetPedestrianGreenLightStateAsync(new Empty());
                    break;
                case "Yellow":
                    response = await _client.GetCarYellowLightStateAsync(new Empty());
                    break;
                case "Red":
                    response = isCar
                        ? await _client.GetCarRedLightStateAsync(new Empty())
                        : await _client.GetPedestrianRedLightStateAsync(new Empty());
                    break;
                default:
                    throw new ArgumentException($"Unknown light state: {light}");
            }

            return response.LightState;
        }
    }
}
