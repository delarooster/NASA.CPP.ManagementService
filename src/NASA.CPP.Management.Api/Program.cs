using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace VOYG.CPP.Management.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, cfg) =>
                {
                    var builtConfig = cfg.Build();

                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                    cfg.AddAzureKeyVault(builtConfig["KeyVaultUrl"], keyVaultClient, new DefaultKeyVaultSecretManager());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}