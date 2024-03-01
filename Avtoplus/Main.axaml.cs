using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Avtoplus;

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
    }

    private void OpenKlientForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenMasinaForm(object? sender, RoutedEventArgs e)
    {
        Masina MasinaForm = new Masina();
        MasinaForm.Show();
    }

    private void OpenRemontForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenYslygaForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenDetaliForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenZakazForm(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenRabForm(object? sender, RoutedEventArgs e)
    {
        
    }
}