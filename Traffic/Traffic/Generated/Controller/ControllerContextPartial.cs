namespace Traffic.Generated.Controller
{
    public partial class ControllerContext
    {
        public TimeSpan CarGreenLightDuration { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan CarYellowLightDuration { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan CarAndPedestrianStopDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan BlinkingLightDelay { get; set; } = TimeSpan.FromMilliseconds(100);
        public TimeSpan PedestrianGreenLightDuration { get; set; } = TimeSpan.FromSeconds(1);

    }
}
