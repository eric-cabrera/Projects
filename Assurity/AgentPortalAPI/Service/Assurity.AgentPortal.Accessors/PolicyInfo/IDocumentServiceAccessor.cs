namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using System.Threading.Tasks;

public interface IDocumentServiceAccessor
{
    Task<byte[]> GetImageByIdAsync(string policyNumber, string objectClass);
}