using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WPF_VisionPro_Demo.ViewModels.Pages;

namespace WPF_VisionPro_Demo.Views.Pages
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPageVM ViewModel { get; }


        public SettingsPage()
        {
            var viewModel = App.Current.Services.GetRequiredService<SettingsPageVM>();
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

       
    }
}
