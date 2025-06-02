namespace Assurity.AgentPortal.Utilities.Tests.Formatting;

using Xunit;

public class FormatDateDataGenerator : TheoryData<DateTime?, string, string>
{
    public FormatDateDataGenerator()
    {
        Add(null, "MM/dd/yyyy", null);
        Add(new DateTime(2025, 01, 15), "MM/dd/yyyy", "01/15/2025");
        Add(new DateTime(2025, 01, 15), "dd/MM/yyyy", "15/01/2025");
        Add(new DateTime(2025, 01, 15), string.Empty, "1/15/2025 12:00:00 AM");
        Add(default(DateTime), string.Empty, null);
    }
}
