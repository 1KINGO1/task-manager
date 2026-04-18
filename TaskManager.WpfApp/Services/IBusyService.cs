using System.ComponentModel;

namespace TaskManager.WpfApp.Services;

public interface IBusyService : INotifyPropertyChanged
{
    bool IsBusy { get; }
    string? Message { get; }

    IDisposable BeginWork(string? message = null);
}
