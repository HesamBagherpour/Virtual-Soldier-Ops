using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class EnumNameAttribute : Attribute
{
    public string NameValue { get; }

    public EnumNameAttribute(string nameValue)
    {
        NameValue = nameValue;
    }
}


public static class EnumExtensions
{
    public static string GetName(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        EnumNameAttribute attr = 
            (EnumNameAttribute)Attribute.GetCustomAttribute(field, typeof(EnumNameAttribute));
        return attr == null ? value.ToString() : attr.NameValue;
    }
}
