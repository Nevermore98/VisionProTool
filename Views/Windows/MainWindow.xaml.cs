using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Forms.Integration;
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
        }

       

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigationView.Navigate(typeof(RunningPage));
        }

    }
}