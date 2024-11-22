using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace VisionPro_Tool.ViewModels.Pages
{
    public partial class SettingsPageVM : ObservableObject
    {
        [ObservableProperty]
        private string _name = "设置";

        [ObservableProperty]
        private string _version = "";

        public SettingsPageVM()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
