using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json;

namespace AspNetCore.SampleOpenApi.Transformers;

internal static class TransformerUtil
{
    public static bool TryGetOpenApiPropertySchema(
        OpenApiSchema schema,
        PropertyInfo pi,
        JsonNamingPolicy? propertyNamingPolicy,
        out OpenApiSchema propertySchema
    )
    {
        propertySchema = null!;
        var result = TryGetOpenApiPropertyName(schema, pi, propertyNamingPolicy, out var propertyName);
        if (result)
        {
            propertySchema = (schema.Properties![propertyName] as OpenApiSchema)!;
            result = propertySchema != null;
        }

        return result;
    }

    public static bool TryGetOpenApiPropertyName(
        OpenApiSchema schema,
        PropertyInfo pi,
        JsonNamingPolicy? propertyNamingPolicy,
        out string propertyName
    )
    {
        propertyName = null!;
        var expectedPropertyName = GetExpectedPropertyName(pi, propertyNamingPolicy);
        bool result;
        if (schema.Properties == null)
        {
            result = false;
        }
        else
        {
            result = schema.Properties.ContainsKey(expectedPropertyName);
            if (result)
            {
                propertyName = expectedPropertyName;
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
            throw new NotSupportedException("Unsupported JsonNamingPolicy");
            // TO DO: Support these as well
            //JsonNamingPolicy.KebabCaseLower
            //JsonNamingPolicy.KebabCaseUpper
            //JsonNamingPolicy.SnakeCaseLower
            //JsonNamingPolicy.SnakeCaseUpper

        }

    }
}
