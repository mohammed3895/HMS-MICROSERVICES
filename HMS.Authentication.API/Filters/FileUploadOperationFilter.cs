using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HMS.Authentication.API.Filters
{
    /// <summary>
    /// Operation filter to handle file upload parameters in Swagger documentation
    /// </summary>
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.ApiDescription.ParameterDescriptions
                .Where(p => p.ModelMetadata?.ModelType == typeof(IFormFile))
                .ToList();

            if (!fileParameters.Any())
                return;

            // Clear existing parameters
            operation.Parameters?.Clear();

            // Set request body for multipart/form-data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "Profile picture file (image format)"
                                }
                            },
                            Required = new HashSet<string> { "file" }
                        }
                    }
                }
            };
        }
    }
}