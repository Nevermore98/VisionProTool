using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Timers;
using System.Windows.Controls;
using WPF_VisionPro_Demo.ViewModels.Pages;
using Timer = System.Timers.Timer;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_VisionPro_Demo.Views.Pages
{
    /// <summary>
    /// DebugPage.xaml 的交互逻辑
    /// </summary>
    public partial class DebugPage : Page
    {
        public DebugPageVM ViewModel { get; }


        public DebugPage()
        {
            Loaded += DebugPage_Loaded;
            ViewModel = App.Current.Services.GetRequiredService<DebugPageVM>();
            DataContext = ViewModel;
            InitializeComponent();
        }



        private void DebugPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.ToolBlockEditV2Control = cogToolBlockEditV2;
        }
    }
}
