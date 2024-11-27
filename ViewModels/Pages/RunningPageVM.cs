using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using Serilog;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using Wpf.Ui.Controls;
using VisionPro_Tool.Models;
using MessageBox = System.Windows.MessageBox;

namespace VisionPro_Tool.ViewModels.Pages;

public partial class RunningPageVM : ObservableRecipient, IRecipient<PropertyChangedMessage<string>>
{
    public void Receive(PropertyChangedMessage<string> message)
    {
        if (message.Sender is DebugPageVM debugPage)
        {
            ImagePathList = new();
            OutputList = new();
            RunCount = 0;
            SaveBmpCount = 0;
            KeepRunningTime = "00:00";
            CurrentHandleTime = 0;
            AverageHandleTime = 0;

            VppFilePath = debugPage.VppFilePath;
            VppFileName = Path.GetFileNameWithoutExtension(VppFilePath);
            if (File.Exists(VppFilePath)) ToolBlock = (CogToolBlock)CogSerializer.LoadObjectFromFile(VppFilePath);
            if (RecordDisplayControl != null) RecordDisplayControl.Record = null;
        }
    }

    private ILogger _logger;

    public CogRecordDisplay RecordDisplayControl { get; set; }
    public CogToolBlock ToolBlock { get; set; } = new();
    public CogImageFileTool ImageFileTool { get; set; } = new();

    private ICogImage _currentImage;
    private object _lockObj = new();


    public string VppFilePath { get; set; } = "";
    [ObservableProperty]
    private string _vppFileName = "";
    public string BmpFileDir { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");


    private Stopwatch _currentRunStopwatch = new();

    private Stopwatch _keepRunningStopwatch = new();
    private DispatcherTimer _keepRunningTimer = new();

    // 总处理时间 = _currentHandleTime 累加
    private double _totalHandleTime;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsExistInput))]
    private List<InputItem> _inputList = new();
    [ObservableProperty]
    private List<OutputItem> _outputList = new();


    [ObservableProperty]
    private string _name = "运行";
    [ObservableProperty]
    private string _result = "无结果";
    [ObservableProperty]
    private int _currentIndex = 1;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(KeepRunningButtonContent), nameof(KeepRunningButtonIcon), nameof(KeepRunningAppearance))]
    private bool _isKeepRunning = false;

    #region 设置
    [ObservableProperty]
    private bool _isSaveBmp = false;
    [ObservableProperty]
    private int _keepRunningDelayTime = 100;
    #endregion

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsImageCountMoreThanOne), nameof(ImageCount))]
    [NotifyCanExecuteChangedFor(nameof(NextImageCommand), nameof(PreviousImageCommand))]
    private List<string> _imagePathList = new(); // 文件夹下的图像路径列表



    #region 统计
    private TimeSpan _keepRunningTimeSpan;

    [ObservableProperty]
    private int _runCount = 0;
    [ObservableProperty]
    private int _saveBmpCount = 0;

    [ObservableProperty]
    private double _currentHandleTime = 0;
    [ObservableProperty]
    private double _averageHandleTime = 0;
    [ObservableProperty]
    private string _keepRunningTime = "00:00";
    #endregion


    #region 持续运行按钮状态切换
    public string KeepRunningButtonContent => IsKeepRunning ? "停止" : "持续运行";
    public SymbolIcon KeepRunningButtonIcon => IsKeepRunning ? new SymbolIcon { Symbol = SymbolRegular.Stop24, Filled = true } : new SymbolIcon { Symbol = SymbolRegular.ArrowRepeatAll24, Filled = true };
    public string KeepRunningAppearance => IsKeepRunning ? "Danger" : "Primary";
    #endregion


    public bool IsImageCountMoreThanOne => ImagePathList.Count > 1;
    public int ImageCount => ImagePathList.Count;
    public bool IsExistInput => InputList.Count > 0;


    public RunningPageVM()
    {
        // IRecipient<PropertyChangedMessage<string>> 需要设置 IsActive = true，否则无法接收
        IsActive = true;

        _logger = App.Current.Services.GetRequiredService<ILogger>();

        _keepRunningTimer.Interval = TimeSpan.FromSeconds(0.5);
        _keepRunningTimer.Tick += (sender, e) =>
        {
            KeepRunningTime = _keepRunningStopwatch.Elapsed.ToString(@"mm\:ss");
        };
    }

    [RelayCommand]
    public void Loaded()
    {
        InitControls();

        void InitControls()
        {
            try
            {
                RecordDisplayControl.AutoFit = true;
                RecordDisplayControl.Image = ToolBlock.Inputs["InputImage"]?.Value as ICogImage;
                InputList = ToolBlock.Inputs
                    .Where(x => x.Name != "InputImage")
                    .Select((x, index) => new InputItem(index + 1, x.ValueType.Name, x.Name, x.Value)).ToList();
            }
            catch { }
            // TODO 调试用
            //try
            //{
            //    string folderPath = $@"D:\DotNet-Projects\03_机器视觉项目\WPF-VisionPro-Demo\bin\x64\Debug\net48\VppData\零件瑕疵检测\img";
            //    ImagePathList = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
            //        .Where(x => x.EndsWith(".bmp") || x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".ttf") || x.EndsWith("."))
            //        .ToList();

            //    //加载图像到 ImageFileTool
            //    ImageFileTool.Operator.Open(ImagePathList[CurrentIndex - 1], CogImageFileModeConstants.Read);
            //    ImageFileTool.Run();

            //    //RecordDisplayControl.Record = null;
            //    RecordDisplayControl.Image = ImageFileTool.OutputImage;
            //    ToolBlock.Inputs["InputImage"].Value = ImageFileTool.OutputImage;

            //    InputList = ToolBlock.Inputs
            //        .Where(x => x.Name != "InputImage")
            //        .Select((x, index) => new InputItem(index + 1, x.ValueType.Name, x.Name, x.Value)).ToList();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("加载文件夹下的图像失败：" + ex.Message);
            //    return;
            //}
        }
    }


    [RelayCommand]
    public void UnLoaded()
    {
        // ToolBlockEditV2Instance.Dispose() 似乎没有释放资源，需要手动调用 GC.Collect()
        // GC.Collect();
    }

    [RelayCommand]
    public void LoadImage()
    {
        VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
        openFileDialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
        openFileDialog.Title = "选择图像";
        openFileDialog.Multiselect = false;
        openFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData");
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string imagePath = openFileDialog.FileName;
                // 重新赋值 ImagePathList 触发通知
                ImagePathList = new List<string> { imagePath };
                CurrentIndex = 1;
                //加载图像到 ImageFileTool
                ImageFileTool.Operator.Open(imagePath, CogImageFileModeConstants.Read);
                ImageFileTool.Run();

                RecordDisplayControl.Record = null;
                RecordDisplayControl.Image = ImageFileTool.OutputImage;
                ToolBlock.Inputs["InputImage"].Value = ImageFileTool.OutputImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载图像失败：" + ex.Message);
                return;
            }
        }
    }

    [RelayCommand]
    public void LoadFolder()
    {
        VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
        folderBrowserDialog.Description = "选择文件夹";
        folderBrowserDialog.Multiselect = false;
        folderBrowserDialog.ShowNewFolderButton = true;
        folderBrowserDialog.SelectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData");
        if (folderBrowserDialog.ShowDialog() == true)
        {
            string folderPath = folderBrowserDialog.SelectedPath;
            ImagePathList = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => x.EndsWith(".bmp") || x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".ttf") || x.EndsWith("."))
                .ToList();

            if (ImagePathList.Count == 0)
            {
                MessageBox.Show("文件夹中没有图像文件");
                return;
            }

            try
            {
                CurrentIndex = 1;
                //加载图像到 ImageFileTool
                ImageFileTool.Operator.Open(ImagePathList[CurrentIndex - 1], CogImageFileModeConstants.Read);
                ImageFileTool.Run();

                RecordDisplayControl.Record = null;
                RecordDisplayControl.Image = ImageFileTool.OutputImage;
                ToolBlock.Inputs["InputImage"].Value = ImageFileTool.OutputImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载文件夹下的图像失败：" + ex.Message);
                return;
            }

        }
    }

    [RelayCommand]
    public void Run()
    {
        try
        {
            var result = string.Join(", ", InputList.Select(x => $"{x.Name}: {x.Value}"));
            _logger.Information($"开始运行 ToolBlock，输入参数：【{result}】");
            RunToolBlock();
        }
        catch (Exception ex)
        {
            _logger.Error("运行 ToolBlock 失败：" + ex.Message);
            MessageBox.Show("运行失败：" + ex.Message);
        }
    }


    [RelayCommand]
    public void KeepRunning()
    {
        try
        {
            IsKeepRunning = !IsKeepRunning;

            if (IsKeepRunning)
            {
                _keepRunningStopwatch.Restart();
                _keepRunningTimer.Start();
                _keepRunningTimeSpan = TimeSpan.Zero;
                KeepRunningTime = _keepRunningTimeSpan.ToString(@"mm\:ss");

                CurrentHandleTime = 0;
                AverageHandleTime = 0;
                RunCount = 0;
                SaveBmpCount = 0;
                _totalHandleTime = 0;
            }
            else
            {
                _keepRunningTimer.Stop();
                _keepRunningStopwatch.Stop();
            }

            Task.Run(async () =>
            {
                while (IsKeepRunning)
                {
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        CurrentIndex++;
                        if (CurrentIndex > ImagePathList.Count)
                        {
                            CurrentIndex = 1;
                        }
                    });
                    RunToolBlock();
                    await Task.Delay(KeepRunningDelayTime);
                }
            });
        }
        catch (Exception ex)
        {
            IsKeepRunning = false;
            MessageBox.Show("持续运行失败：" + ex.Message);
            return;
        }
    }

    private void RunToolBlock()
    {
        if (ImagePathList.Count > 0)
        {
            // 传入原图格式
            ImageFileTool.Operator.Open(ImagePathList[CurrentIndex - 1], CogImageFileModeConstants.Read);
            ImageFileTool.Run();

            RecordDisplayControl.Record = null;
            RecordDisplayControl.Image = ImageFileTool.OutputImage;
            ToolBlock.Inputs["InputImage"].Value = ImageFileTool.OutputImage;

            // 转为 8 位图，并释放资源
            //using (Bitmap bmp = new Bitmap(ImagePathList[CurrentIndex - 1]))
            //{
            //    _currentImage = new CogImage8Grey(bmp);
            //    RecordDisplayControl.Record = null;
            //    RecordDisplayControl.Image = _currentImage;
            //    ToolBlock.Inputs["InputImage"].Value = _currentImage;
            //}
        }

        // 将输入参数赋值给 ToolBlock
        foreach (CogToolBlockTerminal item in ToolBlock.Inputs)
        {
            if (item.Name == "InputImage") continue;
            item.Value = InputList.First(x => x.Name == item.Name).Value;
        }

        _currentRunStopwatch.Restart();
        ToolBlock.Run();
        _currentRunStopwatch.Stop();

        // 使用同步 Invoke，不能使用异步 InvokeAsync，可能会出现 RunToolBlock 时，CurrentIndex 还没更新
        App.Current.Dispatcher.Invoke(() =>
        {
            CurrentHandleTime = Math.Round(_currentRunStopwatch.Elapsed.TotalMilliseconds, 2);
            _totalHandleTime += CurrentHandleTime;
            RunCount++;
            AverageHandleTime = Math.Round(_totalHandleTime / RunCount, 2);


            RecordDisplayControl.Record = ToolBlock.CreateLastRunRecord().SubRecords[0];
            OutputList = ToolBlock.Outputs
            .Select((x, index) => new OutputItem(index + 1, x.Name, x.Value))
            .ToList();
        });

        var result = string.Join(", ", OutputList.Select(x => $"{x.Name}: {x.Value}"));
        _logger.Information($"运行 ToolBlock 完成，耗时：{CurrentHandleTime} ms，输出参数：【{result}】");

        _currentRunStopwatch.Reset();
        if (IsSaveBmp)
        {
            SaveBmp();
        }
    }

    [RelayCommand(CanExecute = nameof(IsImageCountMoreThanOne))]
    public void NextImage()
    {
        try
        {
            CurrentIndex++;
            if (CurrentIndex > ImagePathList.Count)
            {
                CurrentIndex = 1;
            }

            RunToolBlock();
        }
        catch (Exception ex)
        {
            MessageBox.Show("加载下一张图像失败：" + ex.Message);
            return;
        }
    }

    [RelayCommand(CanExecute = nameof(IsImageCountMoreThanOne))]
    public void PreviousImage()
    {
        try
        {
            CurrentIndex--;
            if (CurrentIndex <= 0)
            {
                CurrentIndex = ImagePathList.Count;
            }

            RunToolBlock();
        }
        catch (Exception ex)
        {
            MessageBox.Show("加载上一张图像失败：" + ex.Message);
            return;
        }
    }

    private void SaveBmp()
    {
        Task.Run(() =>
        {
            if (!Directory.Exists(BmpFileDir))
            {
                Directory.CreateDirectory(BmpFileDir);
            }

            using (Bitmap? bmp = RecordDisplayControl.CreateContentBitmap(0) as Bitmap)
            {
                string savePath = Path.Combine(BmpFileDir, $"{DateTime.Now:yyyyMMdd-HHmmssfff}.bmp");
                bmp?.Save(savePath, ImageFormat.Jpeg);
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                lock (_lockObj)
                {
                    SaveBmpCount++;
                }
            });
        });
    }

    [RelayCommand]
    public void ClearStatistics()
    {
        RunCount = 0;
        SaveBmpCount = 0;
        KeepRunningTime = "00:00";
        CurrentHandleTime = 0;
        AverageHandleTime = 0;
        _totalHandleTime = 0;
    }
}

