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

using StatemachineFramework.Components;
using ExtensionMethods;
using InterfaceServices.Model;
using InterfaceServices.Exception;
using HalFramework;
using Traffic.Generated.Interfaces;
using HalFramework.Interfaces.Reference;

namespace Traffic.Generated;

public class Decomp1Factory
{
    public IList<Component> Components { get; } = new List<Component>();
    public IList<IBaseInterface> Interfaces { get; } = new List<IBaseInterface>();

    public Controller.Controller Controller { get => GetProperty(ref _Controller); protected set => _Controller = value; }
    public HMI.HMI HMI { get => GetProperty(ref _HMI); protected set => _HMI = value; }


    public virtual void Build(HalFactory halFactory)
    {
        Controller = Activator.CreateInstance<Controller.Controller>();
        HMI = Activator.CreateInstance<HMI.HMI>();

        Components.Add(Controller);
        Components.Add(HMI);
        Components.ForEach(component => component.Build());


    }

    public virtual void Connect(HalFactory halFactory)
    {

        #region Component connections

        Connect<StartStop, StartStopImpl>(Controller.I1, HMI.I1Client);

        #endregion

        #region Hal connections

        Connect<NormallyClosedValveItf, NormallyClosedValveItfImpl>(halFactory.hAL1.CarRed, Controller.CarRed);
        Connect<NormallyClosedValveItf, NormallyClosedValveItfImpl>(halFactory.hAL1.CarYellow, Controller.CarYellow);
        Connect<NormallyClosedValveItf, NormallyClosedValveItfImpl>(halFactory.hAL1.CarGreen, Controller.CarGreen);
        Connect<NormallyClosedValveItf, NormallyClosedValveItfImpl>(halFactory.hAL1.PedRed, Controller.PedRed);
        Connect<NormallyClosedValveItf, NormallyClosedValveItfImpl>(halFactory.hAL1.PedGreen, Controller.PedGreen);
        #endregion

    }


    public void Connect<T1, T2>(IHalElement<T1> provider, Port<T1> client) 
        where T1 : IBaseInterface
        where T2 : BaseInterface
    {
        if (provider.Port is null)
        {
            throw new NotImplementedException($"Hal element {provider.GetType()} has an unimplemented port. " +
                                              "Please contact the SuperModels support team to resolve this issue.");
        }

        Connect<T1, T2>(provider.Port, client);
    }


    public void Connect<T1, T2>(Port<T1> provider, Port<T1> client) 
        where T1 : IBaseInterface
        where T2 : BaseInterface
    {
        var instance = provider.Impl;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        // The provider implementation is null, the first time, a client connects to it.
        // Reuse the implementation after that.
        if (instance is null)
        {
            if (Activator.CreateInstance(typeof(T2)) is not T1 t1) throw new InterfaceActivationException(typeof(T2));
            instance = t1;

            Interfaces.Add(instance);
        }

        Connect(provider, client, instance);
    }

    public static void Connect<T>(Port<T> provider, Port<T> client, T instance) where T : IBaseInterface
    {
        instance.Provider.EventBuffer = provider.EventBuffer;
        instance.Provider.PortName = provider.PortName;

        instance.MultiClientStrategy.AddClient(client);

        provider.Impl = instance;
        client.Impl = instance;
    }

    #region internal

    private Controller.Controller? _Controller;
    private HMI.HMI? _HMI;


    private T GetProperty<T>(ref T? value) => 
        value ?? throw new InvalidOperationException($"Component with context {GetType()} is not yet built");

    #endregion
}
