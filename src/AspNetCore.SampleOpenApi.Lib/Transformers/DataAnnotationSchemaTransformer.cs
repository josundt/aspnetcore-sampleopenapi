using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.SampleOpenApi.Lib.Transformers;

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
            if (TransformerUtil.TryGetOpenApiPropertySchema(schema, propertyInfo, jsonOptions?.PropertyNamingPolicy, out OpenApiSchema propertySchema) &&
                Attribute.IsDefined(propertyInfo, typeof(EmailAddressAttribute))
            )
            {
                propertySchema.Format = "email";
            }

            // PS! Tested and found to be supported "out of the box":
            // * RangeAttribute,
            // * LengthAttribute/MinLengthAttribute/MaxLengthAttribute
            // * UrlAttribute
        }

        return Task.CompletedTask;
    }
}
