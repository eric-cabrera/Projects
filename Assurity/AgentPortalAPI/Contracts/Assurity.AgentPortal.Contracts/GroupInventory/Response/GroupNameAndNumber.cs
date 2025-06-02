namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public record GroupNameAndNumber
{
    public string? DisplayValue { get; init; }

    public string? Name { get; init; }

    public string? Number { get; init; }
}
