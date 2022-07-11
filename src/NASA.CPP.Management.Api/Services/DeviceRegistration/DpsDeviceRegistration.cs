using VOYG.CPP.Management.Api.Models.Requests.Registration;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Provisioning.Service;
using VOYG.CPP.Management.Api.Services.DeviceRegistration.Interfaces;

namespace VOYG.CPP.Management.Api.Services
{
    public class DpsDeviceRegistration : IDeviceRegistration, IDpsClient
    {
        private readonly ProvisioningServiceClient _provisioningServiceClient;

        public DpsDeviceRegistration(ProvisioningServiceClient provisioningServiceClient)
        {
            _provisioningServiceClient = provisioningServiceClient;
        }

        public async Task Register(string registrationId, string deviceId, RegistrationRequest registrationRequest)
        {
            IndividualEnrollment individualEnrollment = new(registrationId, new TpmAttestation(registrationRequest.TpmEndorsementKey))
            {
                DeviceId = deviceId
            };

            _ = await _provisioningServiceClient.CreateOrUpdateIndividualEnrollmentAsync(individualEnrollment);
        }

        public async Task<bool> DoesExist(string registrationId)
        {
            try
            {
                var individualEnrollment = await _provisioningServiceClient.GetIndividualEnrollmentAsync(registrationId);
                return individualEnrollment != null;
            }
            catch (ProvisioningServiceClientException) // SDK throwns ProvisioningServiceClientException while registrationId does not exist
            {
                return false;
            }
        }
    }
}