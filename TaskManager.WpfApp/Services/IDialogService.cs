namespace TaskManager.WpfApp.Services;

public interface IDialogService
{
    void ShowInfo(string message, string? title = null);
    void ShowError(string message, string? title = null);
    bool Confirm(string message, string? title = null);
}
