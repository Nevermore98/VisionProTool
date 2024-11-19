using Cognex.VisionPro.ToolBlock;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace WPF_VisionPro_Demo.ViewModels.Pages
{
    public partial class ScriptPageVM : ObservableObject
    {
        [ObservableProperty]
        private string _name = "脚本";
        [ObservableProperty]
        private bool _isSizeChanging = false;


        public CogToolBlockEditV2 ToolBlockEditV2Control { get; set; }


        [RelayCommand]
        public void Loaded()
        {
            ToolBlockEditV2Control.Subject = App.Current.Services.GetRequiredService<DebugPageVM>().ToolBlock;
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
