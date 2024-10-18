namespace Noizera.Shared.Contracts.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AuthorizeAttribute : Attribute
{
    public string? Permissions { get; set; }
    public string? Roles { get; set; }
    public string? Policies { get; set; }
}