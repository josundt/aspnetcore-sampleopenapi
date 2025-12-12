#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public sealed class CustomOpenApiOptions
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool DefaultVersionAssumedWhenUnspecified { get; set; }
    public IEnumerable<(int, int)>? Versions { get; set; }
    public (int, int)? DefaultVersion { get; set; }
}
