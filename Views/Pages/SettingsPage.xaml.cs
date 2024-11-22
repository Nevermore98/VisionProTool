using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using VisionPro_Tool.ViewModels.Pages;

namespace VisionPro_Tool.Views.Pages
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
