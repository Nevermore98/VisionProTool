using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WPF_VisionPro_Demo.ViewModels.Pages;

namespace WPF_VisionPro_Demo.Views.Pages
{
    /// <summary>
    /// RunningPage.xaml 的交互逻辑
    /// </summary>
    public partial class RunningPage : Page
    {
        public RunningPageVM ViewModel { get; }
        public RunningPage()
        {
            Loaded += RunningPage_Loaded;

            ViewModel = App.Current.Services.GetRequiredService<RunningPageVM>();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void RunningPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.RecordDisplayControl = cogRecordDisplay;

        }
    }
}
