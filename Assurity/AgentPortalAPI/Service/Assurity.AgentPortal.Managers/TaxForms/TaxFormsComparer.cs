namespace Assurity.AgentPortal.Managers.TaxForms;

public class TaxFormsComparer : IComparer<string>
{
    public static string ExtractYear(string displayName)
    {
        // Assuming the year is the first part of the displayName
        var parts = displayName.Split(' ');
        return parts.Length > 0 ? parts[0] : displayName;
    }

    public int Compare(string x, string y)
    {
        // ex: yearX = 2022, yearY = 2022 NY (per mockup we want 2022 to come before 2022 NY)
        string yearX = ExtractYear(x);
        string yearY = ExtractYear(y);

        // Compare the years first
        int yearComparison = string.Compare(yearX, yearY, StringComparison.Ordinal);
        if (yearComparison != 0)
        {
            return yearComparison;
        }

        if (x == yearX && y.StartsWith(yearY))
        {
            return 1;
        }

        if (y == yearY && x.StartsWith(yearX))
        {
            return -1;
        }

        // Default alphabetical comparison
        return string.Compare(x, y, StringComparison.Ordinal);
    }
}
