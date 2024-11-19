using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WPF_VisionPro_Demo.ViewModels.Pages;

namespace WPF_VisionPro_Demo.Views.Pages
{
    /// <summary>
    /// CameraPage.xaml 的交互逻辑
    /// </summary>
    public partial class CameraPage : Page
    {
        public CameraPageVM ViewModel { get; }

        public CameraPage()
        {
            ViewModel = App.Current.Services.GetRequiredService<CameraPageVM>();
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
