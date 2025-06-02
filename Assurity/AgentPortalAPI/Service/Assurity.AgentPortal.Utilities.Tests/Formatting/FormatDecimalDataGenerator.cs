namespace Assurity.AgentPortal.Utilities.Tests.Formatting;

using Xunit;

public class FormatDecimalDataGenerator : TheoryData<decimal?, string>
{
    public FormatDecimalDataGenerator()
    {
        Add(null, null);
        Add(2.043m, "2.043%");
        Add(50.00m, "50%");
        Add(0.0m, "0%");
    }
}
