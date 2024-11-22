using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionPro_Tool.ViewModels.Pages
{
    public partial class SettingsPageVM : ObservableObject
    {
        [ObservableProperty]
        private string _name = "设置";
    }
}
