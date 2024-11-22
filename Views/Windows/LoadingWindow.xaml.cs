using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VisionPro_Tool.ViewModels.Windows;

namespace VisionPro_Tool.Views.Windows
{
    /// <summary>
    /// LoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindowVM ViewModel { get; }
        public LoadingWindow()
        {
            Loaded += LoadingWindow_Loaded;
            ViewModel = App.Current.Services.GetRequiredService<LoadingWindowVM>();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void LoadingWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
