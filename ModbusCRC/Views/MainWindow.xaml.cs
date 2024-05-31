using System.Windows;
using ModbusCRC.ViewModels;

namespace ModbusCRC.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();

    }
}