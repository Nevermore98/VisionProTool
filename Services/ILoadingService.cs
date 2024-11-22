using System.Windows;

namespace VisionPro_Tool.Services
{
    public interface ILoadingService
    {
        void SetOwner(Window owner);
        void Show(string title, string message, WindowStartupLocation showLocation = WindowStartupLocation.CenterScreen, bool isShowSpinner = false);
        void Close();
    }
}
