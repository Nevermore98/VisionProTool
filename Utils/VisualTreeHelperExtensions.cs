using System.Windows;
using System.Windows.Media;

namespace VisionPro_Tool.Utils
{
    public static class VisualTreeHelperExtensions
    {
        public static T? FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T? foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // 如果子元素就是要找的类型
                if (child is T typedChild)
                {
                    if (child is FrameworkElement element && element.Name == childName)
                    {
                        foundChild = typedChild;
                        break;
                    }
                }

                // 递归查找
                foundChild = FindChild<T>(child, childName);
                if (foundChild != null) break;
            }
            return foundChild;
        }

        public static T? FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    return typedChild;

                T? recursiveResult = FindChild<T>(child);
                if (recursiveResult != null)
                    return recursiveResult;
            }
            return null;
        }
    }

}
