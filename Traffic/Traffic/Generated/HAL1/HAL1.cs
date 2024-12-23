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
// Source    : HAL1 
// ---------------------------------------------------------------------- 

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

#pragma warning disable CS8618

using System;
using HalFramework;
using ExtensionMethods;
using InterfaceServices;
using InterfaceServices.Model;
using StatemachineFramework.Components;
using HalReference.Components.PandI;
using HalReference.Components.Common;

namespace Traffic.Generated.HAL1;

public partial class HAL1 : HalFramework.Hal
{
    public override string Name => "HAL1";

#region Standard Components

    public HalReference.Components.PandI.NormallyClosedValve CarRed { get; set; }
    public HalReference.Components.PandI.NormallyClosedValve CarYellow { get; set; }
    public HalReference.Components.PandI.NormallyClosedValve CarGreen { get; set; }
    public HalReference.Components.PandI.NormallyClosedValve PedRed { get; set; }
    public HalReference.Components.PandI.NormallyClosedValve PedGreen { get; set; }
    public HalReference.Components.Common.DigitalSensor PedestrianRequest { get; set; }

#endregion

#region Custom Components


#endregion


    public HAL1Communication HAL1Communication { get; set; }
    public Action? CustomBuilder { get; set; }

    public virtual void Build()
    {
        CustomBuilder ??= BuildInternal;
        CustomBuilder.Invoke();
    }

    /// <summary>
    /// Build HAL elements with a built-in interface and implementation.
    /// </summary>
    public virtual void BuildStandardElements()
    {
        CarRed = new HalReference.Components.PandI.NormallyClosedValve(HAL1Communication.CarRed);
        CarYellow = new HalReference.Components.PandI.NormallyClosedValve(HAL1Communication.CarYellow);
        CarGreen = new HalReference.Components.PandI.NormallyClosedValve(HAL1Communication.CarGreen);
        PedRed = new HalReference.Components.PandI.NormallyClosedValve(HAL1Communication.PedRed);
        PedGreen = new HalReference.Components.PandI.NormallyClosedValve(HAL1Communication.PedGreen);
        PedestrianRequest = new HalReference.Components.Common.DigitalSensor(HAL1Communication.PedestrianRequest);
    }


    private void BuildInternal()
    {
        BuildStandardElements();
    }

    /// <summary>
    /// Called after all components have been built. Add model information for HAL elements. 
    /// This information is standard (derived from the model) for 'standard' and 'custom' HAL elements.
    /// </summary>
    public virtual void PostCreationSetup() 
    {
        CarRed.FullyQualifiedName = $"{Name}.CarRed";
        CarYellow.FullyQualifiedName = $"{Name}.CarYellow";
        CarGreen.FullyQualifiedName = $"{Name}.CarGreen";
        PedRed.FullyQualifiedName = $"{Name}.PedRed";
        PedGreen.FullyQualifiedName = $"{Name}.PedGreen";
        PedestrianRequest.FullyQualifiedName = $"{Name}.PedestrianRequest";

        // Instance ids are not yet supported
        CarRed.Port.Source = new EventSource(17, 1);
        CarYellow.Port.Source = new EventSource(18, 1);
        CarGreen.Port.Source = new EventSource(19, 1);
        PedRed.Port.Source = new EventSource(20, 1);
        PedGreen.Port.Source = new EventSource(21, 1);
        PedestrianRequest.Port.Source = new EventSource(30, 1);

        HalElements.Add(CarRed);
        HalElements.Add(CarYellow);
        HalElements.Add(CarGreen);
        HalElements.Add(PedRed);
        HalElements.Add(PedGreen);
        HalElements.Add(PedestrianRequest);
    }
}
