using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace VisionPro_Tool.ViewModels.Windows;

public partial class LoadingWindowVM : ObservableObject
{
    [ObservableProperty]
    private string _message = "加载中...";
    [ObservableProperty]
    private string _title = "加载中";
    [ObservableProperty]
    private bool _isShowSpinner = false;
    [ObservableProperty]
    private WindowStartupLocation _showLocation = WindowStartupLocation.CenterScreen;

    public LoadingWindowVM()
    {
    }
}
