using System.ComponentModel;
using System.Reflection;

namespace Act.Avalonia.UI.Common;

public class DescriptiveEnum<T> where T : Enum
{
    public string Description { get; set; }
    public T Value { get; set; }

    public DescriptiveEnum(T value)
    {
        Value = value;

        FieldInfo? fi = value.GetType().GetField(value.ToString());
        object[]? attributes = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        Description = (attributes != null && attributes.Length != 0) ?
            ((DescriptionAttribute)attributes[0]).Description :
            value.ToString();
    }


}