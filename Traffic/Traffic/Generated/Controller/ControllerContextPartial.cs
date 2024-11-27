namespace Traffic.Generated.Controller
{
    public partial class ControllerContext
    {
        public TimeSpan CarGreenLightDuration { get; set; } = TimeSpan.FromSeconds(2);
        public TimeSpan CarYellowLightDuration { get; set; } = TimeSpan.FromSeconds(2);
        public TimeSpan CarAndPedestrianStopDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan BlinkingLightDelay { get; set; } = TimeSpan.FromMilliseconds(200);
        public TimeSpan PedestrianGreenLightDuration { get; set; } = TimeSpan.FromSeconds(2);
        public TimeSpan PedestrianFullGreenLightDuration { get; set; } = TimeSpan.FromSeconds(2);
        public TimeSpan PedestrianFlickeringGreenLightDuration { get; set; } = TimeSpan.FromSeconds(2);

    }
}
