using CommunityToolkit.Mvvm.ComponentModel;


namespace VisionPro_Tool.Models
{
    public record InputItem(int Id, string ValueType, string Name, object Value);

    //public partial class InputItem : ObservableObject
    //{
    //    [ObservableProperty]
    //    private int _id;
    //    [ObservableProperty]
    //    private string _valueType;
    //    [ObservableProperty]
    //    private string _name;
    //    [ObservableProperty]
    //    private object _value;
    //}
}
