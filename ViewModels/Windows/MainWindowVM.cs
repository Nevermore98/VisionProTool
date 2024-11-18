using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WPF_VisionPro_Demo.ViewModels.Windows
{
    public partial class MainWindowVM :ObservableObject
    {
        [ObservableProperty]
        private string _name = "Main";


        public MainWindowVM()
        {
            //RootNavigation.Navigate(typeof(MyDashboardClass));
        }



        [RelayCommand]
        private void Loaded()
        {
        }
    }
}
