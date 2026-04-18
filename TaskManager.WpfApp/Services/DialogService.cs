using System.Windows;

namespace TaskManager.WpfApp.Services;

public sealed class DialogService : IDialogService
{
    public void ShowInfo(string message, string? title = null) =>
        MessageBox.Show(message, title ?? "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string? title = null) =>
        MessageBox.Show(message, title ?? "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);

    public bool Confirm(string message, string? title = null)
    {
        var result = MessageBox.Show(message, title ?? "Підтвердження",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }
}
