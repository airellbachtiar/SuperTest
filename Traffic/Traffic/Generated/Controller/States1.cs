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
// Source    : Decomp1.Controller.States1 
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

using System.Collections.Generic;
using InterfaceServices.Model;
using StatemachineFramework.Statemachines;
using StatemachineFramework.Statemachines.Builder;
using ExtensionMethods;
using Traffic.Generated.Interfaces;
using HalFramework.Interfaces.Reference;
using HalFramework.Interfaces.Reference.Common;

namespace Traffic.Generated.Controller;

public class States1 : Statemachine
{
    public States1(ControllerContext context, EventBuffer inBuffer)
    {
        Name = "States1";

        // Sub statemachines
        var States1Operational = new States1Operational(context, inBuffer);

        // States
        var initial1 = new StateBuilder(StateId.state_initial1_0, StateBuilder.CreationType.Initial)
            .Name("Initial1")
            .Build();
        var poweredDown = new StateBuilder(StateId.state_poweredDown_1)
            .Name("PoweredDown")
            .Build();
        var operational = new StateBuilder(StateId.state_operational_2)
            .Name("Operational")
            .SubStatemachine(States1Operational)
            .Build();

        States = new List<StatemachineFramework.Statemachines.State>
        {
            initial1,
            poweredDown,
            operational
        };

        // Transitions
        var t1 = new TransitionBuilder(TransitionId.transition_t1_16)
            .Name("t1")
            .From(StateId.state_initial1_0)
            .To(StateId.state_poweredDown_1)
            .Build();
        var it1 = new TransitionBuilder(TransitionId.transition_it1_17)
            .Name("it1")
            .From(StateId.state_poweredDown_1)
            .To(StateId.state_operational_2)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Start), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it1_17())
            .Build();

        Transitions = new List<StatemachineFramework.Statemachines.Transition>
        {
            t1,
            it1
        };

        InitialState = initial1;
        ActiveState = InitialState;
    }

    /// <inheritdoc />
    public override string Name { get; }

    /// <inheritdoc />
    public override IList<StatemachineFramework.Statemachines.State> States { get; }

    /// <inheritdoc />
    public override IList<StatemachineFramework.Statemachines.Transition> Transitions { get; }
}
