using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.WpfApp.Services;

public sealed class BusyService : IBusyService
{
    private int _workCount;
    private string? _message;

    public bool IsBusy => _workCount > 0;

    public string? Message
    {
        get => _message;
        private set
        {
            if (_message == value)
                return;
            _message = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IDisposable BeginWork(string? message = null)
    {
        var wasBusy = IsBusy;
        _workCount++;
        Message = message ?? Message ?? "Виконується операція...";
        if (!wasBusy)
            OnPropertyChanged(nameof(IsBusy));
        return new WorkScope(this);
    }

    private void EndWork()
    {
        if (_workCount == 0)
            return;
        _workCount--;
        if (_workCount == 0)
        {
            Message = null;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private sealed class WorkScope : IDisposable
    {
        private readonly BusyService _owner;
        private bool _disposed;

        public WorkScope(BusyService owner) => _owner = owner;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _owner.EndWork();
        }
    }
}
