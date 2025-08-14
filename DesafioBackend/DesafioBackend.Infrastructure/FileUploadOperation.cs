
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DesafioBackend.Infrastructure;

public class FileUploadOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo.GetParameters()
                            .Where(p => p.ParameterType == typeof(IFormFile))
                            .ToList();

        if (!fileParams.Any())
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = fileParams.ToDictionary(
                            p => p.Name,
                            p => new OpenApiSchema { Type = "string", Format = "binary" }
                        ),
                        Required = (ISet<string>)fileParams.Select(p => p.Name).ToList()
                    }
                }
            }
        };
    }
}
