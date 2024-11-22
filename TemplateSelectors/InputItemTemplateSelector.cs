using System.Windows;
using System.Windows.Controls;
using VisionPro_Tool.Models;

namespace VisionPro_Tool.TemplateSelectors
{
    public class InputItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
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
                    "Boolean" => BoolTemplate,
                    _ => TextTemplate
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
