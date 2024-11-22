using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using VisionPro_Tool.ViewModels.Pages;
using VisionPro_Tool.Views.Windows;

namespace VisionPro_Tool.Views.Pages
{
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
            // 设置高度，避免滚动。ScrollViewer.CanContentScroll = False 似乎不起作用
            Height = App.Current.Services.GetRequiredService<MainWindow>().ActualHeight - 48;

            ViewModel.ToolBlockEditV2Control = cogToolBlockEditV2;
        }
    }
}
