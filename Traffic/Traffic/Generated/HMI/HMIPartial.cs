using System.ComponentModel;
using System.Runtime.CompilerServices;
using Component = StatemachineFramework.Components.Component;

namespace Traffic.Generated.HMI;

public partial class HMI : INotifyPropertyChanged
{
    public IEnumerable<Component> Components { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    const double Epsilon = 0.0001;

    public void Update() { }
    
    public void SendAllStatesToClients() { }

    public void InitAllStates()
    {
        SendAllStatesToClients();
        Update();
    }
}