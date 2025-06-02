using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum || Nullable.GetUnderlyingType(context.Type)?.IsEnum == true)
        {
            var enumType = context.Type.IsEnum ? context.Type : Nullable.GetUnderlyingType(context.Type);

            schema.Enum.Clear();

            if (Nullable.GetUnderlyingType(context.Type) != null)
            {
                schema.Enum.Add(new OpenApiString(string.Empty));
            }

            foreach (var name in Enum.GetNames(enumType))
            {
                schema.Enum.Add(new OpenApiString(name));
            }

            schema.Type = "string";
        }
    }
}
