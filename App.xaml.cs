using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;
using Wpf.Ui;
using WPF_VisionPro_Demo.Services;
using WPF_VisionPro_Demo.ViewModels.Pages;
using WPF_VisionPro_Demo.ViewModels.Windows;
using WPF_VisionPro_Demo.Views.Pages;
using WPF_VisionPro_Demo.Views.Windows;

#if NET5_0_OR_GREATER

#else
using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class IsExternalInit { }
}
#endif

namespace WPF_VisionPro_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }


        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 日志
            var year = DateTime.Now.Year;
            var month = DateTime.Now.ToString("MM");
            var day = DateTime.Now.ToString("dd");

            services.AddSingleton<ILogger>(
              new LoggerConfiguration()
               .WriteTo.File(
                    path: @$"Logs\{year}-{month}\.log",
                    rollingInterval: RollingInterval.Day, // 在上面的文件名追加日期，如 20240101.log
                    retainedFileCountLimit: null,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger());


            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPageService, PageService>();


            // 视图
            services.AddTransient<LoadingWindowVM>();
            services.AddTransient<LoadingWindow>();

            // 可能是 WPF-UI 导航的问题，使用 sp => new VM { DataContext = sp.GetService<VM>() }) 无法绑定到 VM
            // 只好去后台代码中绑定，其他 View 也要这样
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowVM>();

            services.AddSingleton<CameraPage>();
            services.AddSingleton<CameraPageVM>();

            services.AddSingleton<DebugPage>();
            services.AddSingleton<DebugPageVM>();

            services.AddSingleton<ScriptPage>();
            services.AddSingleton<ScriptPageVM>();

            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsPageVM>();


            //services.AddSingleton<VisionDebugVM>();
            //services.AddSingleton<VisionDebugView>(sp => new VisionDebugView { DataContext = sp.GetService<VisionDebugVM>() });

            //services.AddSingleton<VisionRunVM>();
            //services.AddSingleton<VisionRunView>(sp => new VisionRunView { DataContext = sp.GetService<VisionRunVM>() });

            //services.AddSingleton<VisionJobDebugVM>();
            //services.AddSingleton<VisionJobDebugView>(sp => new VisionJobDebugView { DataContext = sp.GetService<VisionJobDebugVM>() });

            //services.AddTransient<DefectDetectionVM>();
            //services.AddTransient<DefectDetectionView>(sp => new DefectDetectionView { DataContext = sp.GetService<DefectDetectionVM>() });

            //services.AddTransient<QuantityStatisticsVM>();
            //services.AddTransient<QuantityStatisticsView>(sp => new QuantityStatisticsView { DataContext = sp.GetService<QuantityStatisticsVM>() });

            //services.AddTransient<TextRecognitionVM>();
            //services.AddTransient<TextRecognitionView>(sp => new TextRecognitionView { DataContext = sp.GetService<TextRecognitionVM>() });

            //services.AddTransient<VppNameSettingWindowVM>();
            //services.AddTransient<VppNameSettingWindowView>(sp => new VppNameSettingWindowView { DataContext = sp.GetService<VppNameSettingWindowVM>() });




            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 优化 winform 控件样式
            System.Windows.Forms.Application.EnableVisualStyles();

            var loadingWindow = Services.GetRequiredService<LoadingWindow>();
            loadingWindow.Show();

            // 先预创建视图，后面切换导航就不用等待
            Services.GetRequiredService<ScriptPage>();
            Services.GetRequiredService<DebugPage>();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            loadingWindow.Close();
            mainWindow.Show();
        }
    }

}
