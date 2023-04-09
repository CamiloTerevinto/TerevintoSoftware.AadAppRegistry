namespace TerevintoSoftware.AadAppRegistry.Tool.Models;

public class ApiAppRegistrationResult
{
    public string Name { get; set; }
    public Guid ClientId { get; set; }
    public Guid ObjectId { get; set; }
    public string Uri { get; set; }
    public string Scope { get; set; }
}
