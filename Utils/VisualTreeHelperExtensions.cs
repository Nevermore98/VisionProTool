using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace WPF_VisionPro_Demo.Utils
{
    public static class VisualTreeHelperExtensions
    {
        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            // 如果parent是空的，返回null
            if (parent == null) return null;

            T foundChild = null;

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
    }

}
