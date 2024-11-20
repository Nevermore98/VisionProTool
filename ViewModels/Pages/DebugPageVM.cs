using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using Wpf.Ui;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace WPF_VisionPro_Demo.ViewModels.Pages
{
    public partial class DebugPageVM : ObservableRecipient
    {
        public CogToolBlockEditV2 ToolBlockEditV2Control { get; set; }


        [ObservableProperty]
        private string _name = "脚本";
        [ObservableProperty]
        private bool _isSizeChanging = false;

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private string _vppFilePath = "";

        partial void OnVppFilePathChanged(string value)
        {
            // 只在 VppFilePath 改变时加载，避免每次 loaded 就加载耗费性能
            var toolBlock = CogSerializer.LoadObjectFromFile(value) as CogToolBlock;
            if (ToolBlockEditV2Control != null) ToolBlockEditV2Control.Subject = toolBlock;
        }

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
                var toolBlock = CogSerializer.LoadObjectFromFile(VppFilePath) as CogToolBlock;
                ToolBlockEditV2Control.Subject = toolBlock;
            }

            //VppFilePath = App.Current.Services.GetRequiredService<DebugPageVM>().VppFilePath;
            //ToolBlockEditV2Control.Subject = App.Current.Services.GetRequiredService<DebugPageVM>().ToolBlock;

            //var toolBlock = CogSerializer.LoadObjectFromFile(VppFilePath) as CogToolBlock;
            //ToolBlockEditV2Control.Subject = toolBlock;
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
                //Messenger.Send(new ValueChangedMessage<string>(VppFilePath), "updateVppFilePath");
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
                //snackbarService.Show( 
                //                     "Don't Blame Yourself.", 
                //                     "No Witcher's Ever Died In His Bed.", 
                //                     ControlAppearance.Primary, 
                //                     new SymbolIcon(SymbolRegular.Fluent24), 
                //                     TimeSpan.FromSeconds(1) 
                //                 ); 
                MessageBox.Show("保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
}
