using Swashbuckle.Swagger;
using System.Collections.Generic;

public class SwaggerHateoasFilter : ISchemaFilter
{
    public void Apply(Schema schema, SchemaRegistry schemaRegistry, System.Type type)
    {
        if (schema.properties != null && schema.properties.ContainsKey("_links"))
        {
            schema.properties["_links"].description = "Enlaces HATEOAS disponibles para este recurso.";
        }
    }
}
