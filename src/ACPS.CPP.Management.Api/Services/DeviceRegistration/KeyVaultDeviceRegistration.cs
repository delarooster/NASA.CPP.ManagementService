using VOYG.CPP.Management.Api.Models.Requests.Registration;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using VOYG.CPP.Management.Api.Extensions;

namespace VOYG.CPP.Management.Api.Services
{
    public class KeyVaultDeviceRegistration : IDeviceRegistration
    {
        private readonly KeyVaultClient _secretClient;
        private readonly ILogger<KeyVaultDeviceRegistration> _logger;
        private readonly string _keyVaultUrl;

        public KeyVaultDeviceRegistration(
            KeyVaultClient secretClient,
            ILogger<KeyVaultDeviceRegistration> logger,
            string keyVaultUrl)
        {
            _secretClient = secretClient;
            _logger = logger;
            _keyVaultUrl = keyVaultUrl;
        }

        public async Task Register(string id, string deviceId, RegistrationRequest registrationRequest)
        {
            if (registrationRequest.Secrets!.IsNullOrEmpty())
            {
                return;
            }

            var jsonSecrets = JsonConvert.SerializeObject(registrationRequest.Secrets);
            var keyVaultKey = deviceId.Replace("_", "-");
            var result = await _secretClient.SetSecretAsync(_keyVaultUrl, keyVaultKey, jsonSecrets);
            _logger.LogTrace($"SetSecret completed: {result}\n");
        }
    }
}