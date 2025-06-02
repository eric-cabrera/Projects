namespace Assurity.AgentPortal.Utilities.Formatting;

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

public static class DataFormatter
{
    public static string? FormatDecimalAsAPercentage(decimal? value)
    {
        if (value.HasValue)
        {
            return $"{string.Format("{0:G29}", value)}%";
        }

        return null;
    }

    public static string? FormatDecimalAsAMonetaryValue(decimal? value)
    {
        if (value.HasValue)
        {
            return $"${string.Format("{0:N2}", value)}";
        }

        return null;
    }

    public static string? FormatDecimal(decimal? value)
    {
        if (value.HasValue)
        {
            return $"{string.Format("{0:G29}", value)}";
        }

        return null;
    }

    public static string FormatPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length != 10 || !long.TryParse(phoneNumber, out _))
        {
            return string.Empty;
        }

        return $"({phoneNumber[..3]}) {phoneNumber.Substring(3, 3)}-{phoneNumber[6..]}";
    }

    public static string? GetEnumDisplayName(Enum? enumValue)
    {
        var field = enumValue?.GetType().GetField(enumValue.ToString());

        if (field == null)
        {
            return null;
        }

        var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;

        return attribute?.Name ?? enumValue?.ToString();
    }

    public static string? FormatDate(DateTime? date, string dateFormat)
    {
        if (!date.HasValue || (date.HasValue && date == DateTime.MinValue))
        {
            return null;  // Ensures a blank cell in Excel to avoid #VALUE!
        }

        return date.Value.ToString(dateFormat);
    }

    public static string? FormatStringToTitleCase(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(value.ToLower());
    }

    /// <summary>
    /// Truncating the zipcode to 5 digits protects us from inconsistent data returned from LifePro or Global data.
    /// </summary>
    /// <param name="zipcode">The original zipcode.</param>
    /// <returns>The first 5 digits of the zipcode.</returns>
    public static string? FormatZipCode(string? zipcode)
    {
        if (string.IsNullOrWhiteSpace(zipcode))
        {
            return null;
        }
        else if (zipcode.Length <= 5)
        {
            return zipcode;
        }

        return zipcode[..5];
    }
}
