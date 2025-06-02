namespace Assurity.Kafka.Utilities
{
    public static class DateTimeUtility
    {
        public static DateTime DateTimeNowCentral()
        {
            var utcDate = DateTime.UtcNow;

            var centralTime = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, centralTime);
        }
    }
}