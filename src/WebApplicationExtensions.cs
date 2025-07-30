using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Scalar.AspNetCore;

namespace Microsoft.AspNetCore.Builder;

internal static class WebApplicationExtensions
{
    public static WebApplication MapCustomOpenApi(this WebApplication app)
    {
        app.MapOpenApi("/openapi/{documentName}.json");
        //app.MapScalarApiReference();

        return app;
    }

    public static WebApplication UseCustomHttps(this WebApplication app)
    {
        app.UseHttpsRedirection();

        return app;
    }

    public static WebApplication UseCustomExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler("/error");

        return app;
    }

    public static WebApplication UseCustomAuthorization(this WebApplication app)
    {
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapCustomControllers(this WebApplication app)
    {
        app.MapControllers();

        return app;
    }

    public static WebApplication UseCustomScalar(this WebApplication app)
    {
        var versionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.MapScalarApiReference("/documentation", options =>
        {
            options.WithTitle("OpenApi Sample API");

            // Add all versions to a single Scalar UI with proper version names
            foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
            {
                var versionTitle = $"Version {description.ApiVersion}";
                options.AddDocument(description.GroupName, versionTitle, $"/openapi/{description.GroupName}.json");
            }
        });

        return app;
    }
}
