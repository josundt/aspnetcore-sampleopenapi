using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace AspNetCore.SampleOpenApi.Api.Transformers;

/// <summary>
/// Transforms OpenAPI schema definitions for enum types to accurately represent their serialization format in generated
/// OpenAPI documents.
/// </summary>
/// <remarks>This transformer inspects enum types and updates the OpenAPI schema to reflect whether the enum
/// values are serialized as strings or integers, based on the configured JSON serialization options. It also accounts
/// for custom enum member names specified via attributes. This ensures that the OpenAPI schema matches the actual JSON
/// representation used by the application, improving the accuracy of API documentation and client code
/// generation.</remarks>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the OpenApi library")]
internal sealed class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var type = context.JsonTypeInfo.Type;
        var isEnum = type.IsEnum;


        if (isEnum)
        {
            var jsonOptions = context.JsonTypeInfo.Options; // NB! This comes from options set through services.ConfigureHttpJsonOptions(options)

            var underLyingType = Enum.GetUnderlyingType(type);

            var hasBuiltInStringEnumConverter = jsonOptions?.Converters.Any(
                c => c.GetType() == typeof(JsonStringEnumConverter)
            ) ?? false;

            var serializeAsString = hasBuiltInStringEnumConverter;

            var attrType = typeof(JsonStringEnumMemberNameAttribute);

            var customNames = type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.IsDefined(attrType, false))
                .Aggregate(
                    new Dictionary<string, string>(),
                    (aggr, field) =>
                    {
                        var customName = field.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name;
                        if (customName != null && field.Name != customName)
                        {
                            aggr[field.Name] = customName;
                        }
                        return aggr;
                    }
                );

            if (serializeAsString)
            {
                schema.Enum = [.. Enum
                    .GetNames(type)
                    .Select(n => JsonValue.Create(GetFormattedEnumName(n, customNames)))
                ];

                schema.Type = JsonSchemaType.String;
            }
            else if (underLyingType == typeof(int))
            {
                schema.Enum = [.. ((int[])Enum
                    .GetValues(type))
                    .Select(n => JsonValue.Create(n))
                ];

                schema.Type = JsonSchemaType.Integer;
                schema.Format = "int32";
            }
            else if (underLyingType == typeof(long))
            {
                schema.Enum = [.. ((long[])Enum
                    .GetValues(type))
                    .Select(n => JsonValue.Create(n))
                ];

                schema.Type = JsonSchemaType.Integer;
                schema.Format = "int64";
            }
        }

        return Task.CompletedTask;
    }

    private static string GetFormattedEnumName(string unformatted, Dictionary<string, string> customNames /*, JsonNamingPolicy policy */)
    {
        // TO DO: Future support of different naming policies (JsonNamingPolicy)?

        if (!customNames.TryGetValue(unformatted, out var result))
        {
            result = unformatted;
        }

        return result;
    }
}
