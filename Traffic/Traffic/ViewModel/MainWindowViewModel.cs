using System;
using Logger;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Tools.Commands;
using UnitsNet;
using Traffic.Generated.Interfaces;

namespace Traffic.ViewModel;



public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;

    public MainWindowViewModel(
        Generated.HMI.HMI hmi,
        ILogger logger)
    {
        Hmi = hmi;
        Logger = logger;

        StartCommand = new DelegateCommand(Start);
        StopCommand = new DelegateCommand(Stop);

        Hmi.PropertyChanged += HmiOnPropertyChanged;
        _timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0,0,1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    void Timer_Tick(object? sender, EventArgs e)
    {

        Hmi.Update();
    }


    private void HmiOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Hmi));
    }

    public ILogger Logger { get; set; }
    public Generated.HMI.HMI Hmi { get; }

    #region MixerControl

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    #endregion


    private void Start()
    {
        Hmi.I1Client.Start();
    }

    private void Stop()
    {
        Hmi.I1Client.Stop();
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}