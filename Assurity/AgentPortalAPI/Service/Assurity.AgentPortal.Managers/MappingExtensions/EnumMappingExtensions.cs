namespace Assurity.AgentPortal.Managers.MappingExtensions;

using System.ComponentModel;

public static class EnumMappingExtensions
{
    public static string? GetDescription(this Enum value)
    {
        Type type = value.GetType();
        var name = Enum.GetName(type, value);

        if (name != null)
        {
            var field = type.GetField(name);
            if (field != null)
            {
                if (Attribute.GetCustomAttribute(
                           field,
                           typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                {
                    return attr.Description;
                }
            }
        }

        return null;
    }
}
