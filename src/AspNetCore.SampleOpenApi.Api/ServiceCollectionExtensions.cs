using Asp.Versioning;
using AspNetCore.SampleOpenApi.Api;
using AspNetCore.SampleOpenApi.Api.Transformers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
    public static IServiceCollection AddConfiguredOpenApi(this IServiceCollection services, Options options)
    {

        if (options.Versions?.Any() ?? false)
        {
            /* var versioningBuilder = */
            services
                .AddApiVersioning(o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = options.DefaultVersionAssumedWhenUnspecified;

                    if (options.DefaultVersion == null)
                    {
                        o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
                    }
                    else
                    {
                        var (major, minor) = options.DefaultVersion.Value;
                        o.DefaultApiVersion = new ApiVersion(major, minor);
                    }
                })
                .AddApiExplorer(o =>
                {
                    o.GroupNameFormat = "'v'V'.'v";
                    o.SubstituteApiVersionInUrl = true;
                    o.SubstitutionFormat = "V'.'v";
                });
            // .AddMvc(o => { //o.Conventions });


            //var inferredVersions = services
            //    .BuildServiceProvider()
            //    .GetRequiredService<IApiVersionDescriptionProvider>()
            //    .ApiVersionDescriptions
            //    .Select(v => (v.ApiVersion.MajorVersion!.Value, v.ApiVersion.MinorVersion!.Value));


            foreach (var v in options.Versions /* inferredVersions */)
            {
                var (major, minor) = v;
                var version = $"v{major}.{minor}";

                services.AddOpenApi(version, o =>
                {
                    o.OpenApiVersion = OpenApi.OpenApiSpecVersion.OpenApi3_1;
                    o.ApplyApiVersionInfo(options.Title, options.Description);
                    o.AddOperationTransformer<ProblemDetailsResponseOperationTransformer>();
                    o.AddSchemaTransformer<EnumSchemaTransformer>();
                    o.AddSchemaTransformer<DataAnnotationSchemaTransformer>();
                    o.AddSchemaTransformer<NullableRequiredFixSchemaTransformer>();
                    //o.ApplyAuthorizationChecks([.. scopes.Keys]);
                    //o.ApplySecuritySchemeDefinitions();
                    //o.ApplyOperationDefaultValues();
                });
            }
        }

        return services;
    }

    public static IServiceCollection AddConfiguredControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(o => ConfigureJsonSerialization(o.JsonSerializerOptions))
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

    public static IServiceCollection AddConfiguredMinimalApi(this IServiceCollection services)
    {
        return services
            // Configure Json Serialization for Minimal APIs
            .ConfigureHttpJsonOptions(options => ConfigureJsonSerialization(options.SerializerOptions))
            // Add Model validation support for minimal APIs
            .AddValidation();
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

    private static void ConfigureJsonSerialization(JsonSerializerOptions o)
    {
        o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.Converters.Add(new JsonStringEnumConverter());
        o.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        o.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
        o.NumberHandling = JsonNumberHandling.Strict;
    }
}
