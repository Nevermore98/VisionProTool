using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using Serilog;
using System.IO;
using System.Windows;
using VisionPro_Tool.Services;
using VisionPro_Tool.Views.Windows;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace VisionPro_Tool.ViewModels.Pages;

public partial class DebugPageVM : ObservableRecipient
{
    public CogToolBlockEditV2 ToolBlockEditV2Control { get; set; }

    private ILoadingService _loadingService;
    private ILogger _logger;


    [ObservableProperty]
    private string _name = "脚本";
    [ObservableProperty]
    private bool _isSizeChanging = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VppFileName))]
    [NotifyPropertyChangedRecipients]
    private string _vppFilePath = "";

    public string VppFileName => Path.GetFileNameWithoutExtension(VppFilePath);

    public DebugPageVM()
    {
        _logger = App.Current.Services.GetRequiredService<ILogger>();
        _loadingService = App.Current.Services.GetRequiredService<ILoadingService>();

        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData","零件瑕疵检测", "vpp");
        string vppName = "零件瑕疵检测（支持输入阈值、查找数量）.vpp";
        string filePath = Path.Combine(dir, vppName);
        if (File.Exists(filePath))
        {
            VppFilePath = filePath;
            var toolBlock = CogSerializer.LoadObjectFromFile(VppFilePath) as CogToolBlock;
            App.Current.Services.GetRequiredService<RunningPageVM>().ToolBlock = toolBlock;
        }
    }

    [RelayCommand]
    public void Loaded()
    {
        if (ToolBlockEditV2Control.Subject == null)
        {
            _loadingService.SetOwner(App.Current.Services.GetRequiredService<MainWindow>());
            _loadingService.Show("加载中", "加载 ToolBlock 中...", WindowStartupLocation.CenterOwner);
            ToolBlockEditV2Control.Subject = App.Current.Services.GetRequiredService<RunningPageVM>().ToolBlock;
            Thread.Sleep(400);
            _loadingService?.Close();
        }

        // 需要每次加载时都重新加载 ToolBlock，这样运行页与调试页的图像才会相同
        ToolBlockEditV2Control.Subject = App.Current.Services.GetRequiredService<RunningPageVM>().ToolBlock;
    }


    [RelayCommand]
    public void UnLoaded()
    {
        //ToolBlockEditV2Instance.Dispose();
        // ToolBlockEditV2Instance.Dispose() 似乎没有释放资源，需要手动调用 GC.Collect()
        //GC.Collect();
    }

    [RelayCommand]
    public void LoadToolBlock()
    {
        VistaOpenFileDialog dialog = new VistaOpenFileDialog();
        dialog.Filter = "VPP 文件|*.vpp";
        dialog.Title = "打开";
        dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
        if (dialog.ShowDialog() == true)
        {
            _loadingService.SetOwner(App.Current.Services.GetRequiredService<MainWindow>());
            _loadingService.Show("加载中", "加载 ToolBlock 中...", WindowStartupLocation.CenterOwner);
            VppFilePath = dialog.FileName;
            ToolBlockEditV2Control.Subject = CogSerializer.LoadObjectFromFile(VppFilePath) as CogToolBlock;
            _loadingService?.Close();

            _logger.Information($"加载 {VppFilePath}");
        }
    }

    [RelayCommand]
    public void SaveToolBlock()
    {
        if (string.IsNullOrEmpty(VppFilePath))
        {
            SaveAsToolBlock();
        }
        else
        {
            CogSerializer.SaveObjectToFile(ToolBlockEditV2Control.Subject, VppFilePath);
            _logger.Information($"保存 {VppFilePath}");

            MessageBox.Show("保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            var toolBlock = CogSerializer.LoadObjectFromFile(VppFilePath) as CogToolBlock;
            App.Current.Services.GetRequiredService<RunningPageVM>().ToolBlock = toolBlock;
        }
    }

    [RelayCommand]
    public void SaveAsToolBlock()
    {
        VistaSaveFileDialog dialog = new VistaSaveFileDialog();
        dialog.Filter = "VPP 文件|*.vpp";
        dialog.Title = "另存为";
        dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
        dialog.FileName = "未命名 ToolBlock.vpp";
        if (dialog.ShowDialog() == true)
        {
            var path = dialog.FileName.EndsWith(".vpp") ? dialog.FileName : dialog.FileName + ".vpp";
            CogSerializer.SaveObjectToFile(ToolBlockEditV2Control.Subject, path);
            // 保存文件后再通知 RunningPage 重新加载
            VppFilePath = path;
            _logger.Information($"另存为 {VppFilePath}");
            //MessageBox.Show("另存为成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    public void CopyVppFilePath()
    {
        Clipboard.SetText(VppFilePath);
    }
}

