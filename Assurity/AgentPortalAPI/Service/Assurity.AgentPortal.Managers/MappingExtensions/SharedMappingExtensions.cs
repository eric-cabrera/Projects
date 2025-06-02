namespace Assurity.AgentPortal.Managers.MappingExtensions
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    public static class MappingExtensions
    {
        public static string GetEnumDisplayName(Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? enumValue.ToString();
        }
    }
}
