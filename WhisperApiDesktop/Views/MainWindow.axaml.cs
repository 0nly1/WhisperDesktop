using Avalonia.Controls;
using WhisperApiDesktop.Services;

namespace WhisperApiDesktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // WhisperComboBox.Items = WhisperService.ResponseFormats;
        // WhisperComboBox.SelectedIndex = 0; 
    }
}