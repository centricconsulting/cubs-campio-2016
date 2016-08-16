using Microsoft.AspNetCore.Http;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace webapi.Filters
{
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            var formFileParams = context.ApiDescription.ActionDescriptor.Parameters
                                    .Where(x => x.ParameterType.IsAssignableFrom(typeof(IFormFile)))
                                    .Select(x => x.Name)
                                    .ToList(); ;

            var formFileSubParams = context.ApiDescription.ActionDescriptor.Parameters
                .Where(x => x.ParameterType.IsAssignableFrom(typeof(IFormFile)))
                .SelectMany(x => x.ParameterType.GetProperties())
                .Select(x => x.Name)
                .ToList();

            var allFileParamNames = formFileParams.Union(formFileSubParams);

            if (!allFileParamNames.Any())
                return;

            var paramsToRemove = new List<IParameter>();
            paramsToRemove.AddRange(operation.Parameters.Where(p => allFileParamNames.Contains(p.Name)));
            paramsToRemove.ForEach(x => operation.Parameters.Remove(x));
            foreach (var paramName in formFileParams)
            {
                var fileParam = new NonBodyParameter
                {
                    Type = "file",
                    Name = paramName,
                    In = "formData"
                };
                operation.Parameters.Add(fileParam);
            }
            foreach (IParameter param in operation.Parameters)
            {
                param.In = "formData";
            }

            operation.Consumes = new List<string>() { "multipart/form-data" };
        }

    }

}
