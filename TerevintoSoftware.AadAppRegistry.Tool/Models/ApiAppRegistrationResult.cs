using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.AadAppRegistry.Tool.Models;

[ExcludeFromCodeCoverage]
public class ApiAppRegistrationResult
{
    public string Name { get; set; }
    public Guid ClientId { get; set; }
    public Guid ObjectId { get; set; }
    public string Uri { get; set; }
    public string Scope { get; set; }
}
