using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using Wpf.Ui;
using WPF_VisionPro_Demo.Views.Windows;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace WPF_VisionPro_Demo.ViewModels.Pages;

public partial class DebugPageVM : ObservableRecipient
{
    public CogToolBlockEditV2 ToolBlockEditV2Control { get; set; }

    private LoadingWindow _loadingWindow;

    [ObservableProperty]
    private string _name = "脚本";
    [ObservableProperty]
    private bool _isSizeChanging = false;

    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private string _vppFilePath = "";

    private ISnackbarService snackbarService;


    public DebugPageVM()
    {
        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData");
        string vppName = "零件瑕疵检测（支持输入阈值、查找数量，并显示在图像上）TB.vpp";

        VppFilePath = Path.Combine(dir, vppName);
        var toolBlock = CogSerializer.LoadObjectFromFile(VppFilePath) as CogToolBlock;
        App.Current.Services.GetRequiredService<RunningPageVM>().ToolBlock = toolBlock;
    }

    [RelayCommand]
    public void Loaded()
    {
        if (ToolBlockEditV2Control.Subject == null)
        {
            _loadingWindow = App.Current.Services.GetRequiredService<LoadingWindow>();
            _loadingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _loadingWindow.Owner = App.Current.Services.GetRequiredService<MainWindow>();
            _loadingWindow.Show();
            _loadingWindow.Close();
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
            ToolBlockEditV2Control.Subject = CogSerializer.LoadObjectFromFile(dialog.FileName) as CogToolBlock;
            MessageBox.Show("加载成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            VppFilePath = dialog.FileName;
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

            MessageBox.Show("保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);


            // TODO 保存时，通知 RunningPageVM 重新加载 ToolBlock
            //Messenger.Send(new PropertyChangedMessage<string>(this, VppFilePath, VppFilePath, VppFilePath));
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
            string path = dialog.FileName.EndsWith(".vpp") ? dialog.FileName : dialog.FileName + ".vpp";
            CogSerializer.SaveObjectToFile(ToolBlockEditV2Control.Subject, path);
            MessageBox.Show("另存为成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            VppFilePath = path;
        }
    }
}

