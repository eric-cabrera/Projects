namespace Assurity.AgentPortal.Managers.MappingExtensions
{
    public static class DateMappingExtensions
    {
        public static DateOnly? ToNullableDateOnly(this DateTime? dateTime)
        {
            return dateTime == null
                ? null
                : DateOnly.FromDateTime(dateTime.Value);
        }
    }
}
