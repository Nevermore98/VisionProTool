using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WPF_VisionPro_Demo.Models;

namespace WPF_VisionPro_Demo.ViewModels.Pages
{
    public partial class RunningPageVM : ObservableObject
    {
        public CogRecordDisplay RecordDisplayControl { get; set; }


        public CogToolBlock ToolBlock { get; set; } = new();
        public CogImageFileTool ImageFileTool { get; set; } = new();


        [ObservableProperty]
        private List<InputItem> _inputList = new();
        [ObservableProperty]
        private List<OutputItem> _outputList = new();


        [ObservableProperty]
        private string _name = "运行";
        [ObservableProperty]
        private string _result = "无结果";
        [ObservableProperty]
        private double _inputThreshold = 0.8;

        public RunningPageVM()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VppData");
            string vppName = "零件瑕疵检测（支持输入阈值、PMCount）TB.vpp";

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

            void InitControls()
            {
                RecordDisplayControl.AutoFit = true;
                InputList = ToolBlock.Inputs
                    .Where(x => x.Name != "InputImage")
                    .Select((x, index) => new InputItem(index + 1, x.ValueType.Name, x.Name, x.Value))
                    .ToList();

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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
            openFileDialog.Title = "打开图像";
            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string imagePath = openFileDialog.FileName;

                    //加载图像到 ImageFileTool
                    ImageFileTool.Operator.Open(imagePath, CogImageFileModeConstants.Read);
                    ImageFileTool.Run();

                    RecordDisplayControl.Record = null;
                    RecordDisplayControl.Image = ImageFileTool.OutputImage;
                    ToolBlock.Inputs["InputImage"].Value = ImageFileTool.OutputImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("打开图像失败：" + ex.Message);
                    return;
                }
            }
        }

        [RelayCommand]
        public void LoadFolder()
        {

        }

        [RelayCommand]
        public void Run()
        {
            //ToolBlock.Inputs["Threshold"].Value = InputList.
            foreach (CogToolBlockTerminal item in ToolBlock.Inputs)
            {
                if (item.Name == "InputImage") continue;
                item.Value = InputList.First(x => x.Name == item.Name).Value;
            }


            ToolBlock.Run();

            // TODO 修改 找瑕疵，脚本异常，pma 找不到
            RecordDisplayControl.Record = ToolBlock.CreateLastRunRecord().SubRecords[0];
            OutputList = ToolBlock.Outputs
                .Select((x, index) => new OutputItem(index + 1, x.Name, x.Value))
                .ToList();
        }
    }
}
