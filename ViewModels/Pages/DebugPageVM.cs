using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WPF_VisionPro_Demo.ViewModels.Pages
{
    public partial class DebugPageVM : ObservableObject
    {
        [ObservableProperty]
        private string _name = "调试";
        [ObservableProperty]
        private bool _isSizeChanging = false;


        public CogToolBlockEditV2 ToolBlockEditV2Control { get; set; }


        [RelayCommand]
        public void Loaded()
        {
            //ToolBlockEditV2Instance.Subject?.Dispose();
        }

  
        [RelayCommand]
        public void UnLoaded()
        {
            //ToolBlockEditV2Instance.Dispose();
            // ToolBlockEditV2Instance.Dispose() 似乎没有释放资源，需要手动调用 GC.Collect()
            //GC.Collect();
        }
    }
}
