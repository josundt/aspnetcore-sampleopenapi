using Asp.Versioning;
using Asp.Versioning.Conventions;
using AspNetCore.SampleOpenApi;
using AspNetCore.SampleOpenApi.Transformers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Net.Mime;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

internal sealed class Options
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool DefaultVersionAssumedWhenUnspecified { get; set; }
    public IEnumerable<(int, int)>? Versions { get; set; }
    public (int, int)? DefaultVersion { get; set; }
}

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services, Options options)
    {

        if (options.Versions?.Any() ?? false)
        {
            /* var versioningBuilder = */
            services
            .AddApiVersioning(o =>
            {
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader("api-version")
                );
                o.AssumeDefaultVersionWhenUnspecified = options.DefaultVersionAssumedWhenUnspecified;
                o.ReportApiVersions = true;
                //if (options.DefaultVersion == null)
                //{
                //    o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
                //}
                //else
                //{
                //    var (major, minor) = options.DefaultVersion.Value;
                //    o.DefaultApiVersion = new ApiVersion(major, minor);
                //}
            })
            .AddApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'V'.'v";
                //o.SubstituteApiVersionInUrl = true;
                //o.SubstitutionFormat = "V'.'v";
            })
            .AddMvc(o =>
            {
                o.Conventions.Add(new VersionByNamespaceConvention());
            });



            foreach (var v in options.Versions)
            {
                var (major, minor) = v;
                var version = $"v{major}.{minor}";

                services.AddOpenApi(version, o =>
                {
                    o.ApplyApiVersionInfo(options.Title, options.Description);
                    o.AddOperationTransformer<ProblemDetailsOperationTransformer>();
                    //o.ApplyAuthorizationChecks([.. scopes.Keys]);
                    //o.ApplySecuritySchemeDefinitions();
                    //o.ApplyOperationDefaultValues();
                });
            }
        }

        return services;
    }

    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(o =>
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            )
            .AddMvcOptions(o =>
            {
                // Remove redundant output formatters ("text/plain" and "text/json")
                o.RemoveRedundantOutputFormatters();

                // Ensure validation errors (ValidationProblemDetails from ModelState dictionary) are
                // serialized to JSON with correct serialization options (e.g. camelCase conversion).
                o.ModelMetadataDetailsProviders.Add(
                    new SystemTextJsonValidationMetadataProvider());
            });

        return services;
    }

    private static MvcOptions RemoveRedundantOutputFormatters(this MvcOptions mvcOptions)
    {
        // Remove string output formatter
        mvcOptions
            .OutputFormatters
            .RemoveType<StringOutputFormatter>();

        // Remove redundant text/json media type from SystemTextJsonOutputFormatter
        mvcOptions.OutputFormatters
            .OfType<SystemTextJsonOutputFormatter>()
            .FirstOrDefault()?
            .SupportedMediaTypes
            .Remove("text/json");

        // By now, only "applicaion/json" (and "application/*+json") should be supported

        return mvcOptions;
    }
}
