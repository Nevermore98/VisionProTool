﻿using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using VisionPro_Tool.ViewModels.Pages;
using VisionPro_Tool.Views.Windows;

namespace VisionPro_Tool.Views.Pages
{
    /// <summary>
    /// CameraPage.xaml 的交互逻辑
    /// </summary>
    public partial class CameraPage : Page
    {
        public CameraPageVM ViewModel { get; }

        public CameraPage()
        {
            Loaded += CameraPage_Loaded;
            ViewModel = App.Current.Services.GetRequiredService<CameraPageVM>();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void CameraPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 设置高度，避免滚动。ScrollViewer.CanContentScroll = False 似乎不起作用
            Height = App.Current.Services.GetRequiredService<MainWindow>().ActualHeight - 48;
            ViewModel.AcqFifoEditV2Control = cogAcqFifoEditV2;
        }
    }
}
