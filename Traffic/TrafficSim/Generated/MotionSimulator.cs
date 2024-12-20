// ReSharper disable IdentifierTypo
// ReSharper disable CheckNamespace
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable InconsistentNaming

// ---------------------------------------------------------------------- 
// GENERATED FILE 
// All code in this file is generated by the SuperModels workbench 
// (version 5.0.0.1292). Any changes made to this file may lead to 
// incorrect behaviour and will be lost if the code is generated again. 
// Modify the model instead. 
// 
// Copyright : Sioux Technologies 
// Model     : Traffic.sms (Traffic) 
// Generator : Motion simulator (MotionSystem) 
// Source    : Input 
// ---------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using MotionSimComponents;
using Tools;

namespace Generated;

public partial class MotionSimulator : MotionSimulatorBase
{
    #region Load Sensors

    #endregion

    #region DI DO AI AO

    public DigitalSensor PedestrianRequest { get; }

    #endregion

    #region Encoders


    #endregion

    #region SubAssemblies
    #endregion

    public MotionSimulator()
    {
        ResetSimulator();
        PedestrianRequest = new DigitalSensor(30,  "PedestrianRequest");

        #region Compile collections





        DigitalSensors.Add(30, PedestrianRequest);

        SubAssemblies.Add(0, _systemSubAssembly);
        HoloElements.Add(_systemSubAssembly);

        #endregion

        Products.Initialize(HoloElements);
    }
}