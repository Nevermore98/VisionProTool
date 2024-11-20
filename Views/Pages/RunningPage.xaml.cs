﻿using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WPF_VisionPro_Demo.ViewModels.Pages;
using WPF_VisionPro_Demo.Views.Windows;

namespace WPF_VisionPro_Demo.Views.Pages
{
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
            // 设置高度，避免滚动。ScrollViewer.CanContentScroll = False 似乎不起作用
            Height = App.Current.Services.GetRequiredService<MainWindow>().ActualHeight - 48;

            ViewModel.RecordDisplayControl = cogRecordDisplay;
        }
    }
}