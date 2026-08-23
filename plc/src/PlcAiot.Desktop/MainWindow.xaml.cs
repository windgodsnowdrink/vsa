using System.Windows;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using PlcAiot.Desktop.Components;

namespace PlcAiot.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Resources.Add("services", App.Services);

        BlazorWebView.RootComponents.Add(new RootComponent
        {
            Selector = "#app",
            ComponentType = typeof(Routes)
        });
    }
}
