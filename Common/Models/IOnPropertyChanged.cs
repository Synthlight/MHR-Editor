using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RE_Editor.Common.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IOnPropertyChanged : INotifyPropertyChanged {
    public void OnPropertyChanged(PropertyChangedEventArgs eventArgs);

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null);

    public void OnPropertyChanged(IEnumerable<string> propertyName);

    public void OnPropertyChanged(params string[] propertyName);
}

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public abstract class OnPropertyChangedBase : IOnPropertyChanged {
#pragma warning disable CS0067 // The event is never used
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

    public void OnPropertyChanged(PropertyChangedEventArgs eventArgs) {
        PropertyChanged?.Invoke(this, eventArgs);
    }

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public void OnPropertyChanged(IEnumerable<string> propertyName) {
        foreach (var name in propertyName) {
            OnPropertyChanged(name);
        }
    }

    public void OnPropertyChanged(params string[] propertyName) {
        foreach (var name in propertyName) {
            OnPropertyChanged(name);
        }
    }
}