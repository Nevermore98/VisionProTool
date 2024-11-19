using System.Windows;
using System.Windows.Controls;
using WPF_VisionPro_Demo.Models;

namespace WPF_VisionPro_Demo.TemplateSelectors
{
    public class InputItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is InputItem inputItem)
            {
                return inputItem.ValueType switch
                {
                    "Int32" => IntTemplate,
                    "Double" => DoubleTemplate,
                    "String" => TextTemplate,
                    _ => TextTemplate
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
