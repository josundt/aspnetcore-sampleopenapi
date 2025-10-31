using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace AspNetCore.SampleOpenApi.Transformers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the OpenApi library")]
internal sealed class DataAnnotationSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var type = context.JsonTypeInfo.Type;

        var jsonOptions = context.JsonTypeInfo.Options;
        foreach (var propertyInfo in type.GetProperties())
        {
            // Ensure email address annotated properties get the string type format property set correctly
            if (
                Attribute.IsDefined(propertyInfo, typeof(EmailAddressAttribute)) &&
                TryGetOpenApiPropertySchema(schema, propertyInfo, jsonOptions?.PropertyNamingPolicy, out OpenApiSchema propertySchema)
            )
            {
                propertySchema.Format = "email";
            }

            // Properties with Required attributes may be defined as nullable to
            // allow deserialization to null but disallow null value in validation, and
            // give validation errors in a controlled fashion.
            // They should then still be documented as non-nullable in OpenAPI schemas.
            if (
                Attribute.IsDefined(propertyInfo, typeof(RequiredAttribute)) &&
                TryGetOpenApiPropertySchema(schema, propertyInfo, jsonOptions?.PropertyNamingPolicy, out propertySchema)
            )
            {
                // Remove nullable from primitive type
                if (propertySchema.Type != null && propertySchema.Type.Value.HasFlag(JsonSchemaType.Null))
                {
                    // Remove null
                    propertySchema.Type = propertySchema.Type.Value & ~JsonSchemaType.Null;
                }

                // Remove oneOf null + other type construct and only set     other type as schema for complex types
                // Do we really need this for complex types? RequiredAttribute is more relevant for primitive types
                //if (propertySchema.OneOf != null && propertySchema.OneOf.Count == 2 && propertySchema.OneOf.Any(s => s.Type == JsonSchemaType.Null))
                //{
                //    var notNullSchema = propertySchema.OneOf.First(s => s.Type != JsonSchemaType.Null);
                //    // TODO: Set property to notNullSchema properly 
                //}

                // TODO: The below lines seem to be redundant; non-nullable properties seem to automatically be added to required list. Verify!
                //propertySchema.Required ??= new HashSet<string>()
                //propertySchema.Required.Add(propertyInfo.Name)
            }
        }

        return Task.CompletedTask;
    }


    private static bool TryGetOpenApiPropertySchema(
        OpenApiSchema schema,
        PropertyInfo pi,
        JsonNamingPolicy? propertyNamingPolicy,
        out OpenApiSchema propertySchema
    )
    {
        propertySchema = null!;
        var expectedPropertyName = GetExpectedPropertyName(pi, propertyNamingPolicy);
        bool result;
        if (schema.Properties == null)
        {
            result = false;
        }
        else
        {
            IOpenApiSchema s;
            result = schema.Properties.TryGetValue(expectedPropertyName, out s!);
            if (result)
            {
                propertySchema = (s as OpenApiSchema)!;
                result = propertySchema != null;
            }
        }
        return result;
    }

    private static string GetExpectedPropertyName(PropertyInfo pi, JsonNamingPolicy? propertyNamingPolicy)
    {
        return CaseConvert(pi.Name, propertyNamingPolicy);
    }

    private static string CaseConvert(string dotnetName, JsonNamingPolicy? namingPolicy)
    {
        if (string.IsNullOrEmpty(dotnetName) || namingPolicy is null)
        {
            return dotnetName;
        }
        if (namingPolicy == JsonNamingPolicy.CamelCase)
        {
            return $"{dotnetName[0].ToString().ToLowerInvariant()}{dotnetName[1..]}";
        }
        else
        {
            return dotnetName;
            // TO DO: Support these as well
            //JsonNamingPolicy.KebabCaseLower
            //JsonNamingPolicy.KebabCaseUpper
            //JsonNamingPolicy.SnakeCaseLower
            //JsonNamingPolicy.SnakeCaseUpper

        }

    }
}
