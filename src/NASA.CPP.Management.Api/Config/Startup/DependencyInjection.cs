using Azure.Data.Tables;
using Azure.Storage.Blobs;
using VOYG.CPP.Management.Api.Config.Options;
using VOYG.CPP.Management.Api.Services;
using VOYG.CPP.Management.Api.Services.Interfaces;
using VOYG.CPP.Models;
using VOYG.CPP.Models.Context;
using VOYG.CPP.Models.Entities;
using VOYG.CPP.Models.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using PolylineEncoder.Net.Utility;
using Microsoft.Azure.Devices.Provisioning.Service;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using VOYG.CPP.Management.Api.Services.DeviceRegistration;
using VOYG.CPP.Management.Api.Services.DeviceRegistration.Interfaces;

namespace VOYG.CPP.Management.Api.Config.Startup
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(factory =>
            {
                var urlHelperFactory = factory.GetService<IUrlHelperFactory>();
                var actionContext = factory.GetService<IActionContextAccessor>()
                    ?.ActionContext;

                return urlHelperFactory.GetUrlHelper(actionContext);
            });
            services.AddScoped<IUrlService, UrlService>();

            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddSingleton<Func<IUnitOfWork>>(serviceProvider => () => GetUnitOfWork(configuration.GetConnectionString("DbConnection")));
            services.AddSingleton(GetRegistryManager(configuration.GetConnectionString("IoTHub")));
            services.AddSingleton(GetBlobServiceClient(configuration.GetConnectionString("C2D")));
            services.AddSingleton(GetTableServiceClient(configuration.GetConnectionString("D2C")));
            services.AddSingleton(GetServiceClient(configuration.GetConnectionString("IoTHub")));
            services.AddSingleton(GetPolylineUtility());

            services.AddScoped<IBlobServiceProvider, BlobServiceProvider>();
            services.AddScoped<IDeploymentService, DeploymentService>();
            services.AddScoped<ISetFileUploadsService, SetFileUploadsService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IManifestDeploymentService, ManifestDeploymentService>();
            services.AddScoped<ICommandsService, CommandsService>();
            services.AddScoped<IRegistrationService, RegistrationService>();

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IScopeIdProvider>(_ => new ScopeIdProvider(configuration["DpsScopeId"]));

            ConfigureDeviceRegistration(services, configuration);

            services.AddScoped(_ => ProvisioningServiceClient.CreateFromConnectionString(configuration.GetConnectionString("DPS")));
            services.AddScoped(_ => new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback)));

            return services;
        }

        private static void ConfigureDeviceRegistration(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDeviceRegistration, DpsDeviceRegistration>();
            
            services.AddScoped<IDeviceRegistration>(x => new KeyVaultDeviceRegistration(
                x.GetRequiredService<KeyVaultClient>(),
                x.GetRequiredService<ILogger<KeyVaultDeviceRegistration>>(),
                configuration["KeyVaultUrl"]));

            services.AddScoped<IDeviceRegistration, DatabaseDeviceRegistration>();
            services.AddScoped<IDpsClient, DpsDeviceRegistration>();
        }

        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownProxies.Add(IPAddress.Parse(configuration["ApiManagementPublicIp"]));
            });

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = true;
            });

            services.Configure<ContainerNamesOptions>(configuration.GetSection(ContainerNamesOptions.ContainerNames));
            services.Configure<ApisOptions>(configuration.GetSection(ApisOptions.Apis));
            services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.Storage));

            return services;
        }

        private static IUnitOfWork GetUnitOfWork(string connectionString)
        {
            var dbContext = new NasaDbContext(new DbContextOptionsBuilder<NasaDbContext>()
                .UseSqlServer(connectionString)
                .Options);

            return new UnitOfWork(
                dbContext,
                new Repository<DownloadFile, int>(dbContext),
                new Repository<ExecCommand, int>(dbContext),
                new Repository<Package, int>(dbContext),
                new Repository<Deployment, int>(dbContext),
                new Repository<Manifest, DateTime>(dbContext),
                new SetFileRepository(dbContext),
                new Repository<ManifestDeployment, int>(dbContext),
                new DeviceRepsitory(dbContext),
                new DeviceLifecycleRepository(dbContext),
                new Repository<DeviceDetails, int>(dbContext)
                );
        }

        private static RegistryManager GetRegistryManager(string connectionString)
        {
            return RegistryManager.CreateFromConnectionString(connectionString);
        }

        private static BlobServiceClient GetBlobServiceClient(string connectionString)
        {
            return new BlobServiceClient(connectionString);
        }

        private static TableServiceClient GetTableServiceClient(string connectionString)
        {
            return new TableServiceClient(connectionString);
        }

        private static ServiceClient GetServiceClient(string connectionString)
        {
            return ServiceClient.CreateFromConnectionString(connectionString);
        }

        private static PolylineUtility GetPolylineUtility()
        {
            return new PolylineUtility();
        }
    }
}
