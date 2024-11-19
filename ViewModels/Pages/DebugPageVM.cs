using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using Serilog;
using System.IO;
using WPF_VisionPro_Demo.Models;
using MessageBox = System.Windows.MessageBox;

namespace WPF_VisionPro_Demo.ViewModels.Pages
{
    public partial class DebugPageVM : ObservableObject
    {
        public CogRecordDisplay RecordDisplayControl { get; set; }


        public CogToolBlock ToolBlock { get; set; } = new();
        public CogImageFileTool ImageFileTool { get; set; } = new();


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
        [NotifyPropertyChangedFor(nameof(IsImageCountMoreThanOne), nameof(ImageCount))]
        [NotifyCanExecuteChangedFor(nameof(NextImageCommand), nameof(PreviousImageCommand))]
        private List<string> _imagePathList = new(); // 文件夹下的图像路径列表


        public bool IsImageCountMoreThanOne => ImagePathList.Count > 1;
        public int ImageCount => ImagePathList.Count;
        public bool IsExistInput => InputList.Count > 0;

        private ILogger _logger;

        public DebugPageVM()
        {
            _logger = App.Current.Services.GetRequiredService<ILogger>();
            _logger.Information($"RunningPageVM");


            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData");
            string vppName = "零件瑕疵检测（支持输入阈值、查找数量，并显示在图像上）TB.vpp";

            string path = Path.Combine(dir, vppName);

            // 加载 vpp，比较耗时，放到构造函数中，而不是 Loaded，导航切换会触发 Loaded
            ToolBlock = (CogToolBlock)CogSerializer.LoadObjectFromFile(path);
        }

        [RelayCommand]
        public void Loaded()
        {
            //string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData");
            //string vppName = "零件瑕疵检测TB.vpp";

            //string path = Path.Combine(dir, vppName);

            // 加载 vpp，比较耗时
            //ToolBlock = (CogToolBlock)CogSerializer.LoadObjectFromFile(path);

            InitControls();
            // TODO watch toolblock 变化再重新加载，否则就不加载，避免重新加载耗时
            void InitControls()
            {
                RecordDisplayControl.AutoFit = true;
                // TODO FindCount 绑定不上
                InputList = ToolBlock.Inputs
                    .Where(x => x.Name != "InputImage")
                    .Select((x, index) => new InputItem(index + 1, x.ValueType.Name, x.Name, x.Value)).ToList();
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
            // 清空 ImagePathList
            ImagePathList = new();
            CurrentIndex = 1;
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
                    ImagePathList = new List<string> { imagePath };

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
                    System.Windows.MessageBox.Show("加载文件夹下的图像失败：" + ex.Message);
                    return;
                }

            }
        }

        [RelayCommand]
        public void Run()
        {
            try
            {
                RunToolBlock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("运行失败：" + ex.Message);
                return;
            }
        }

        private void RunToolBlock()
        {
            if (ImagePathList.Count > 0)
            {
                ImageFileTool.Operator.Open(ImagePathList[CurrentIndex - 1], CogImageFileModeConstants.Read);
                ImageFileTool.Run();
                RecordDisplayControl.Record = null;
                RecordDisplayControl.Image = ImageFileTool.OutputImage;
                ToolBlock.Inputs["InputImage"].Value = ImageFileTool.OutputImage;
            }

            // 将输入参数赋值给 ToolBlock
            foreach (CogToolBlockTerminal item in ToolBlock.Inputs)
            {
                if (item.Name == "InputImage") continue;
                item.Value = InputList.First(x => x.Name == item.Name).Value;
            }

            ToolBlock.Run();

            RecordDisplayControl.Record = ToolBlock.CreateLastRunRecord().SubRecords[0];
            OutputList = ToolBlock.Outputs
                .Select((x, index) => new OutputItem(index + 1, x.Name, x.Value))
                .ToList();
        }

        [RelayCommand(CanExecute = nameof(IsImageCountMoreThanOne))]
        public void NextImage()
        {
            try
            {
                CurrentIndex++;
                if (CurrentIndex >= ImagePathList.Count)
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
    }
}
