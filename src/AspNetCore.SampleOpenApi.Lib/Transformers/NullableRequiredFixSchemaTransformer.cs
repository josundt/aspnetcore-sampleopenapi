using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AspNetCore.SampleOpenApi.Lib.Transformers;

/// <summary>
/// Provides an OpenAPI schema transformer that adjusts property nullability and required status based on JSON
/// serialization options and data annotations.
/// </summary>
/// <remarks>This transformer is intended for use with OpenAPI schema generation, ensuring that properties marked
/// as required or configured to omit null values are correctly represented in the schema. It is instantiated by the
/// OpenAPI library and should not be used directly.</remarks>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the OpenApi library")]
internal sealed class NullableRequiredFixSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var type = context.JsonTypeInfo.Type;

        (schema.Metadata ?? new Dictionary<string, object>()).TryGetValue("x-schema-id", out var typeName);

        var jsonOptions = context.JsonTypeInfo.Options;
        var jsonIgnoreCondition = jsonOptions.DefaultIgnoreCondition;

        foreach (var propertyInfo in type.GetProperties())
        {
            var hasRequiredAttrib = Attribute.IsDefined(propertyInfo, typeof(RequiredAttribute));
            if (TransformerUtil.TryGetOpenApiPropertyName(schema, propertyInfo, jsonOptions?.PropertyNamingPolicy, out var propertyName))
            {
                if (schema.Properties![propertyName] is not OpenApiSchema propertySchema)
                {
                    continue;
                }

                jsonIgnoreCondition = propertyInfo.GetCustomAttributes<JsonIgnoreAttribute>().FirstOrDefault()?.Condition
                    ?? jsonIgnoreCondition;

                var omitNulls = jsonIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault;

                if (omitNulls || hasRequiredAttrib)
                {
                    if (propertySchema.Metadata != null && propertySchema.Metadata.ContainsKey("x-is-nullable-property"))
                    {
                        propertySchema.Metadata.Remove("x-is-nullable-property");
                    }

                    if (propertySchema.Type != null && propertySchema.Type.Value.HasFlag(JsonSchemaType.Null))
                    {
                        // Remove null
                        propertySchema.Type = propertySchema.Type.Value & ~JsonSchemaType.Null;
                    }

                    var nullEnumIndex = propertySchema.Enum?.ToList().FindIndex(v => v == null) ?? -1;
                    if (nullEnumIndex >= 0)
                    {
                        propertySchema.Enum?.RemoveAt(nullEnumIndex);
                    }
                }
                else
                {
                    schema.Required ??= new HashSet<string>();
                    schema.Required.Add(propertyName);
                }

            }
        }
        return Task.CompletedTask;
    }


}
