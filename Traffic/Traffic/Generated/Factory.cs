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

#pragma warning disable CS8618

using System;
using System.Collections.Generic;
using InterfaceServices;
using StatemachineFramework.Components;
using StatemachineFramework.Runner;

namespace Traffic.Generated;

public class Factory
{
    public TrafficDomainModelFactory TrafficDomainModel { get; set; } = new();
    public HalFactory TrafficDomainModelHal { get; set; } = new();

    public virtual void Build()
    {
        BuildHal();
        BuildComponents();
    }

    public virtual void BuildHal()
    {
        TrafficDomainModelHal.Build();
    }

    public virtual void BuildComponents()
    {
        TrafficDomainModel.Build(TrafficDomainModelHal);
        TrafficDomainModel.Connect(TrafficDomainModelHal);
    }
}