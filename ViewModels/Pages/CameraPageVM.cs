using Cognex.VisionPro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPF_VisionPro_Demo.Views.Windows;

namespace WPF_VisionPro_Demo.ViewModels.Pages;

public partial class CameraPageVM : ObservableObject
{
    public CogAcqFifoTool AcqFifoToolControl { get; set; }
    public CogAcqFifoEditV2 AcqFifoEditV2Control { get; set; }

    private LoadingWindow _loadingWindow;


    [RelayCommand]
    public void Loaded()
    {
        if (AcqFifoEditV2Control == null)
        {
            _loadingWindow = App.Current.Services.GetRequiredService<LoadingWindow>();
            _loadingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _loadingWindow.Owner = App.Current.Services.GetRequiredService<MainWindow>();
            _loadingWindow.Show();
            _loadingWindow.Close();
        }
    }
}

