#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class WebApplicationExtensions
{
    public static WebApplication MapConfiguredOpenApi(this WebApplication app)
    {
        app.MapOpenApi("/openapi/{documentName}.json");
        app.MapOpenApi("/openapi/{documentName}.yaml");

        return app;
    }

    public static WebApplication UseConfiguredHttps(this WebApplication app)
    {
        app.UseHttpsRedirection();

        return app;
    }

    public static WebApplication UseConfiguredExceptionHandler(this WebApplication app)
    {
        app
            .UseExceptionHandler()
            .UseStatusCodePages(); // Ensures ProblemDetails response behavior for minimal api endpoints on non-success results

        return app;
    }

    public static WebApplication UseConfiguredAuthorization(this WebApplication app)
    {
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapConfiguredControllers(this WebApplication app)
    {
        app.MapControllers();

        return app;
    }
}
