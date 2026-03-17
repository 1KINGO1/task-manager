using System.Windows;
using TaskManager.WpfApp.ViewModels;

namespace TaskManager.WpfApp;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
