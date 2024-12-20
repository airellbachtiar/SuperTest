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
// Source    : TrafficDomainModel.Controller.TrafficStates.Operational 
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
using StatemachineFramework.Statemachines.Triggers;
using Traffic.Generated.Interfaces;
using HalFramework.Interfaces.Reference;
using HalFramework.Interfaces.Reference.Common;

namespace Traffic.Generated.Controller;

public class TrafficStatesOperational : Statemachine
{
    public TrafficStatesOperational(ControllerContext context, EventBuffer inBuffer)
    {
        Name = "States";

        // Sub statemachines
        var TrafficStatesOperationalPedestrianGreenLightFlickerBlinkingStates = new TrafficStatesOperationalPedestrianGreenLightFlickerBlinkingStates(context, inBuffer);

        // States
        var initial2 = new StateBuilder(StateId.state_initial2_6, StateBuilder.CreationType.Initial)
            .Name("Initial2")
            .Build();
        var carGreenLight = new StateBuilder(StateId.state_carGreenLight_7)
            .Name("CarGreenLight")
            .OnEntry(context.CarMayDrive)
            .Build();
        var carYellowLight = new StateBuilder(StateId.state_carYellowLight_8)
            .Name("CarYellowLight")
            .OnEntry(context.CarShouldSlowDown)
            .Build();
        var carRedLight = new StateBuilder(StateId.state_carRedLight_9)
            .Name("CarRedLight")
            .OnEntry(context.CarsShouldStop)
            .Build();
        var pedestrianGreenLightFlicker = new StateBuilder(StateId.state_pedestrianGreenLightFlicker_10)
            .Name("PedestrianGreenLightFlicker")
            .SubStatemachine(TrafficStatesOperationalPedestrianGreenLightFlickerBlinkingStates)
            .Build();
        var pedestrianRedLight = new StateBuilder(StateId.state_pedestrianRedLight_17)
            .Name("PedestrianRedLight")
            .OnEntry(context.__ENTRY_state_pedestrianRedLight_17)
            .Build();
        var pedestrianGreenLight = new StateBuilder(StateId.state_pedestrianGreenLight_18)
            .Name("PedestrianGreenLight")
            .OnEntry(context.__ENTRY_state_pedestrianGreenLight_18)
            .OnExit(context.__EXIT_state_pedestrianGreenLight_18)
            .Build();

        States = new List<StatemachineFramework.Statemachines.State>
        {
            initial2,
            carGreenLight,
            carYellowLight,
            carRedLight,
            pedestrianGreenLightFlicker,
            pedestrianRedLight,
            pedestrianGreenLight
        };

        // Transitions
        var t2 = new TransitionBuilder(TransitionId.transition_t2_8)
            .Name("t2")
            .From(StateId.state_initial2_6)
            .To(StateId.state_carGreenLight_7)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Build();
        var t4 = new TransitionBuilder(TransitionId.transition_t4_9)
            .Name("t4")
            .From(StateId.state_carGreenLight_7)
            .To(StateId.state_carYellowLight_8)
            .Guard(context.__GUARD_transition_t4_9)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarYellow.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarYellow.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Promise(() => context.CarGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Build();
        var t5 = new TransitionBuilder(TransitionId.transition_t5_10)
            .Name("t5")
            .From(StateId.state_carYellowLight_8)
            .To(StateId.state_carRedLight_9)
            .Guard(context.__GUARD_transition_t5_10)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarYellow.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarYellow.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Build();
        var t6 = new TransitionBuilder(TransitionId.transition_t6_11)
            .Name("t6")
            .From(StateId.state_carRedLight_9)
            .To(StateId.state_pedestrianGreenLight_18)
            .Guard(context.__GUARD_transition_t6_11)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.PedGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Build();
        var t7 = new TransitionBuilder(TransitionId.transition_t7_12)
            .Name("t7")
            .From(StateId.state_pedestrianGreenLightFlicker_10)
            .To(StateId.state_pedestrianRedLight_17)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .FinishedTrigger("Final1", pedestrianGreenLightFlicker.SubStatemachines, FinishedTriggerType.All)
            .Build();
        var t8 = new TransitionBuilder(TransitionId.transition_t8_13)
            .Name("t8")
            .From(StateId.state_pedestrianRedLight_17)
            .To(StateId.state_carGreenLight_7)
            .Guard(context.__GUARD_transition_t8_13)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Build();
        var t30 = new TransitionBuilder(TransitionId.transition_t30_14)
            .Name("t30")
            .From(StateId.state_pedestrianGreenLight_18)
            .To(StateId.state_pedestrianGreenLightFlicker_10)
            .Guard(context.__GUARD_transition_t30_14)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.PedGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Build();
        var it2 = new TransitionBuilder(TransitionId.transition_it2_15)
            .Name("it2")
            .From(StateId.state_carGreenLight_7)
            .To(StateId.state_idleFlicker_1)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Stop), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it2_15())
            .Build();
        var it5 = new TransitionBuilder(TransitionId.transition_it5_16)
            .Name("it5")
            .From(StateId.state_carYellowLight_8)
            .To(StateId.state_idleFlicker_1)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Stop), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarYellow.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarYellow.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it5_16())
            .Build();
        var it6 = new TransitionBuilder(TransitionId.transition_it6_17)
            .Name("it6")
            .From(StateId.state_carRedLight_9)
            .To(StateId.state_idleFlicker_1)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Stop), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it6_17())
            .Build();
        var it7 = new TransitionBuilder(TransitionId.transition_it7_18)
            .Name("it7")
            .From(StateId.state_pedestrianGreenLightFlicker_10)
            .To(StateId.state_idleFlicker_1)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Stop), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it7_18())
            .Build();
        var it8 = new TransitionBuilder(TransitionId.transition_it8_19)
            .Name("it8")
            .From(StateId.state_pedestrianRedLight_17)
            .To(StateId.state_idleFlicker_1)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Stop), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.PedRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.PedRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it8_19())
            .Build();
        var it26 = new TransitionBuilder(TransitionId.transition_it26_20)
            .Name("it26")
            .From(StateId.state_carGreenLight_7)
            .To(StateId.state_carGreenLight_7)
            .InterfaceEvent(new InterfaceServices.Model.EventId("I1", StartStop.Events.Start), inBuffer)
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Guard(_ => !inBuffer.ContainsIncoming("p", typeof(NormallyClosedValveItf.Events)))
            .Promise(() => context.CarGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Promise(() => context.CarGreen.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarGreen.Impl.Provider.PortName, NormallyClosedValveItf.Events.Open)))
            .Promise(() => context.CarRed.Impl.Provider.EventBuffer.Promise(new InterfaceServices.Model.EventId(context.CarRed.Impl.Provider.PortName, NormallyClosedValveItf.Events.Close)))
            .InterfaceEffect((InterfaceServices.Model.EventArgs _) => context.__EFFECT_transition_it26_20())
            .Build();

        Transitions = new List<StatemachineFramework.Statemachines.Transition>
        {
            t2,
            t4,
            t5,
            t6,
            t7,
            t8,
            t30,
            it2,
            it5,
            it6,
            it7,
            it8,
            it26
        };

        InitialState = initial2;
    }

    /// <inheritdoc />
    public override string Name { get; }

    /// <inheritdoc />
    public override IList<StatemachineFramework.Statemachines.State> States { get; }

    /// <inheritdoc />
    public override IList<StatemachineFramework.Statemachines.Transition> Transitions { get; }
}
