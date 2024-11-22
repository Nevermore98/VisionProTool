using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VisionPro_Tool.ViewModels.Windows
{
    public partial class MainWindowVM :ObservableObject
    {
        [ObservableProperty]
        private string _name = "Main";


        public MainWindowVM()
        {
        }



        [RelayCommand]
        private void Loaded()
        {
        }
    }
}
