using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VisionPro_Tool.Views.Windows;

namespace VisionPro_Tool.Services;
class LoadingService : ILoadingService
{
    private LoadingWindow? _loadingWindow;

    public void SetOwner(Window owner)
    {
        _loadingWindow = App.Current.Services.GetRequiredService<LoadingWindow>();

        App.Current.Dispatcher.Invoke(() =>
        {
            _loadingWindow.Owner = owner;
        });
    }

    public void Show(string title, string message, WindowStartupLocation showLocation = WindowStartupLocation.CenterScreen, bool isShowSpinner = false)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            // 因为LoadingWindow 是 AddTransient，每次创建都不一样
            // 如果 _loadingWindow 为 null，表示没有 setOwner，就新建 LoadingWindow。如果不为 null，就不重新创建
            _loadingWindow ??= App.Current.Services.GetRequiredService<LoadingWindow>();

            _loadingWindow.ViewModel.Title = title;
            _loadingWindow.ViewModel.Message = message;
            _loadingWindow.ViewModel.IsShowSpinner = isShowSpinner;

            _loadingWindow.WindowStartupLocation = showLocation;

            _loadingWindow.Show();
        });
    }

    public void Close()
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            _loadingWindow?.Close();
            _loadingWindow = null;
        });
    }
}

