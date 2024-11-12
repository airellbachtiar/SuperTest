using TrafficSim.Generated;
using System;

namespace TrafficSim.Services;

public class HolodeckFluidStateSyncService
{
    private PandISimulator _fluidSimulator;

    public HolodeckFluidStateSyncService(PandISimulator fluidSimulator)
    {
        _fluidSimulator = fluidSimulator;
    }

    public void Cycle()
    {
    }
}
