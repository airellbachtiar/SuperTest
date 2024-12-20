using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBus;

namespace TrafficTest.Models
{
    public class TrafficLightState
    {
        public LightResponse CarRed { get; set; } = new();
        public LightResponse CarYellow { get; set; } = new();
        public LightResponse CarGreen { get; set; } = new();
        public LightResponse PedestrianRed { get; set; } = new();
        public LightResponse PedestrianGreen { get; set; } = new();

        public double TimeStamp { get; set; }
    }
}
