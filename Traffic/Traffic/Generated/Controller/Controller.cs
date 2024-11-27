// ---------------------------------------------------------------------- 
// GENERATED FILE 
// All code in this file is generated by the SuperModels workbench 
// (version 5.0.0.1292). Any changes made to this file may lead to 
// incorrect behaviour and will be lost if the code is generated again. 
// Modify the model instead. 
// 
// Copyright : Sioux Technologies 
// Model     : Traffic.sms (Traffic) 
// Generator : C# state machine generator (Decomp1) 
// Source    : TrafficDomainModel.Controller 
// ---------------------------------------------------------------------- 

// ReSharper disable IdentifierTypo
// ReSharper disable CheckNamespace
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantUsingDirective
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable once ClassNeverInstantiated.Global

#pragma warning disable CS8618

using System;
using System.Collections.Generic;
using InterfaceServices.Model;
using StatemachineFramework.Components;
using StatemachineFramework.Statemachines;
using StatemachineFramework.Statemachines.Builder;
using ExtensionMethods;
using HalFramework.Interfaces.Reference;
using HalFramework.Interfaces.Reference.Common;
using Traffic.Generated.Interfaces;

namespace Traffic.Generated.Controller;

public partial class Controller : Component
{
    private readonly Func<ControllerContext> _contextFactory = Activator.CreateInstance<ControllerContext>;
    public ControllerContext Context { get; private set; }

    public override string Name => "Controller";
    public override IList<Statemachine> Statemachines { get; } = new List<Statemachine>();
    public override ushort TypeId => 1;

    #region Providing interfaces

    public Port<StartStop> I1 => Context.I1;

    #endregion

    #region Requiring interfaces

    public Port<NormallyClosedValveItf> CarGreen => Context.CarGreen;
    public Port<NormallyClosedValveItf> CarRed => Context.CarRed;
    public Port<NormallyClosedValveItf> CarYellow => Context.CarYellow;
    public Port<NormallyClosedValveItf> PedGreen => Context.PedGreen;
    public Port<NormallyClosedValveItf> PedRed => Context.PedRed;
    public Port<DigitalSensorItf> PedestrianRequest => Context.PedestrianRequest;

    #endregion

    public override void Build()
    {
        Context = _contextFactory.Invoke();
        Context.I1 = new Port<StartStop>(InBuffer, "I1", new EventSource(TypeId, InstanceId));
        Context.CarGreen = new Port<NormallyClosedValveItf>(InBuffer, "CarGreen", new EventSource(TypeId, InstanceId));
        Context.CarRed = new Port<NormallyClosedValveItf>(InBuffer, "CarRed", new EventSource(TypeId, InstanceId));
        Context.CarYellow = new Port<NormallyClosedValveItf>(InBuffer, "CarYellow", new EventSource(TypeId, InstanceId));
        Context.PedGreen = new Port<NormallyClosedValveItf>(InBuffer, "PedGreen", new EventSource(TypeId, InstanceId));
        Context.PedRed = new Port<NormallyClosedValveItf>(InBuffer, "PedRed", new EventSource(TypeId, InstanceId));
        Context.PedestrianRequest = new Port<DigitalSensorItf>(InBuffer, "PedestrianRequest", new EventSource(TypeId, InstanceId));

        Context.I1.UsedEvents = Array.Empty<int>();
        Context.CarGreen.UsedEvents = Array.Empty<int>();
        Context.CarRed.UsedEvents = Array.Empty<int>();
        Context.CarYellow.UsedEvents = Array.Empty<int>();
        Context.PedGreen.UsedEvents = Array.Empty<int>();
        Context.PedRed.UsedEvents = Array.Empty<int>();
        Context.PedestrianRequest.UsedEvents = Array.Empty<int>();

        Statemachines.Add(new TrafficStates(Context, InBuffer));

        Statemachines.ForEach(statemachine => statemachine.Build(new BuildInput(Name, this)));
    }
}
