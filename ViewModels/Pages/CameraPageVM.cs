using Cognex.VisionPro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPF_VisionPro_Demo.Services;
using WPF_VisionPro_Demo.Views.Windows;

namespace WPF_VisionPro_Demo.ViewModels.Pages;

public partial class CameraPageVM : ObservableObject
{
    public CogAcqFifoTool AcqFifoToolControl { get; set; }
    public CogAcqFifoEditV2 AcqFifoEditV2Control { get; set; }

    // ctor 是在切换导航时触发，也可在 App.xaml.cs 启动时注入。Loaded 是页面渲染完成时触发
    public CameraPageVM()
    {
    }

    [RelayCommand]
    public void Loaded()
    {
    }
}

