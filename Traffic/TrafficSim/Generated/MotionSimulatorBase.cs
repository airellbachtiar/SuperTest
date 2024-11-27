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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunicationFramework;
using HolodeckGrpcServer.Interfaces;
using MotionSimComponents;

namespace Generated;

public abstract class MotionSimulatorBase : INotifyPropertyChanged, IHoloMotion
{
    private readonly List<IMotorProtocol> _protocols = new();

    #region Simulator management

    private bool _manualMode;
    private double _zoomFactor;
    private double _systemHeight;

    protected readonly PassiveSubAssembly _systemSubAssembly = new PassiveSubAssembly(24, "System")
    {
        Id = 24,
        OffsetX = 0,
        OffsetY = 0,
        OffsetZ = 0,
        OffsetRotX = 0,
        OffsetRotY = 0,
        OffsetRotZ = 0,
        VisualState = 0
    };

    public bool ManualMode
    {
        get => _manualMode;
        set
        {
            if (_manualMode == value) return;
            _manualMode = value;
            if (_manualMode)
            {
                DisableCommunicationLayerCommands(Motors.Values.ToList());
            }
            else
            {
                EnableCommunicationLayerCommands(Motors.Values.ToList());
            }
        }
    }

    public double ZoomFactor
    {
        get => _zoomFactor;
        set
        {
            if (_systemSubAssembly is null || _zoomFactor.Equals(value)) return;

            _zoomFactor = value;
            _systemSubAssembly.ScaleFactor = _zoomFactor;
            OnPropertyChanged();
        }
    }

    public double SystemHeight
    {
        get => _systemHeight;
        set
        {
            if (_systemSubAssembly is null || _systemHeight.Equals(value)) return;

            _systemHeight = value;
            _systemSubAssembly.OffsetZ = _systemHeight;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Simulation elements

    public Dictionary<uint, Motor> Motors { get; } = new();
    public Dictionary<uint, Load> Loads { get; } = new();
    public Dictionary<uint, ILoadSensor> Sensors { get; } = new();
    public Dictionary<uint, RfIdReader> RfidReaders { get; } = new();
    public Dictionary<uint, DigitalSensor> DigitalSensors {get;} = new ();

    #endregion

    #region Holodeck

    public List<IHoloElement> HoloElements { get; set; } = new();
    public Dictionary<uint, PassiveSubAssembly> SubAssemblies { get; } = new Dictionary<uint, PassiveSubAssembly>();
    public Dictionary<uint, HoloDetectionSensor> DetectionSensors { get; set; } = new();
    public Dictionary<uint, HoloSwitch> Switches { get; set; } = new();
    public IHoloProducts Products { get; set; } = new Products();

    public void InitSensors()
    {
        foreach (var load in HoloElements.OfType<Load>())
        {
            load.InitSensors();
        }
    }

    #endregion

    private void EnableCommunicationLayerCommands(List<Motor> motors)
    {
        for (int i = 0; i < motors.Count; i++)
        {
            _protocols[i].MoveToPositionRequest += motors[i].HandleMoveToPositionRequest;
            _protocols[i].MoveAtVelocityRequest += motors[i].HandleMoveAtVelocityRequest;
            _protocols[i].SetInternalPositionRequest += motors[i].HandleSetInternalPositionRequest;
        }
    }

    private void DisableCommunicationLayerCommands(List<Motor> motors)
    {
        for (int i = 0; i < motors.Count; i++)
        {
            _protocols[i].MoveToPositionRequest -= motors[i].HandleMoveToPositionRequest;
            _protocols[i].MoveAtVelocityRequest -= motors[i].HandleMoveAtVelocityRequest;
            _protocols[i].SetInternalPositionRequest += motors[i].HandleSetInternalPositionRequest;
        }
    }

    public void SimCycle(TimeSpan simulationLoopTime)
    {
        foreach (var motor in Motors.Values)
        {
            motor.DoUpdateElement(simulationLoopTime);
        }
    }

    protected void ResetSimulator()
    {
        ManualMode = false;
        ZoomFactor = 1;
        SystemHeight = 0;
    }

    public void ResetZoom()
    {
        ZoomFactor = 1;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}