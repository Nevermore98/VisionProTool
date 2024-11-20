using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Wpf.Ui.Controls;
using WPF_VisionPro_Demo.Utils;
using WPF_VisionPro_Demo.ViewModels.Windows;
using WPF_VisionPro_Demo.Views.Pages;

namespace WPF_VisionPro_Demo.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindowVM ViewModel { get; }

        // WPF-UI 导航不支持有参构造函数，所以不能传入依赖接口，依赖注入只能在构造函数中进行
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = App.Current.Services.GetRequiredService<MainWindowVM>();
            ViewModel = viewModel;
            DataContext = viewModel;

            Loaded += MainWindow_Loaded;
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var page = VisualTreeHelperExtensions.FindChild<Page>(RootNavigationView);
            if (page != null) page.Height = ActualHeight - 48;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigationView.Navigate(typeof(RunningPage));
        }

    }
}