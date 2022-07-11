using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Linq;

namespace VOYG.CPP.Management.Api.Config.Startup
{
    public static class Swagger
    {
        private const int ApiMajorVersion = 1;
        private const string ApiTitle = "VOYG.CPP.Management.Api";

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((version, apiDescription) =>
                {
                    var values = apiDescription.RelativePath
                     .Split('/')
                     .Select(v => v.Replace("v{api-version}", version));

                    apiDescription.RelativePath = string.Join("/", values);

                    var versionParameter = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == "api-version");

                    if (versionParameter != null)
                    {
                        apiDescription.ParameterDescriptions.Remove(versionParameter);
                    }

                    return true;
                });

                c.SwaggerDoc(GetSwaggerApiVersion(), new OpenApiInfo
                {
                    Title = ApiTitle,
                    Version = GetSwaggerApiVersion(),
                    Description = "Management API"
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IApplicationBuilder UseConfiguredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{GetSwaggerApiVersion()}/swagger.json", $"{ApiTitle} {GetSwaggerApiVersion()}");
            });

            return app;
        }

        private static string GetSwaggerApiVersion()
        {
            return $"v{ApiMajorVersion}";
        }
    }
}
