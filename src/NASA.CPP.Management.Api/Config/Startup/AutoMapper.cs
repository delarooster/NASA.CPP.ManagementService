using AutoMapper;
using VOYG.CPP.Management.Api.Config.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace VOYG.CPP.Management.Api.Config.Startup
{
    public static class AutoMapper
    {
        public static IServiceCollection AddAutoMapperWithProfiles(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DeploymentsProfile());
                mc.AddProfile(new PackagesProfile());
                mc.AddProfile(new DownloadFilesProfile());
                mc.AddProfile(new DeploymentStatusProfile());
                mc.AddProfile(new ManifestDeploymentProfile());
            });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
