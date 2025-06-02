namespace Assurity.AgentPortal.Accessors.GlobalData.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// An entity representing the GlobalData.dbo.ATTRIBUTES table.
/// </summary>
public class Attributes
{
    public string ObjectId { get; set; }

    public int ObjectType { get; set; }

    public string ObjectClass { get; set; }

    public string PolicyNumber { get; set; }

    public string DocType { get; set; }

    public string PrintInclude { get; set; }
}