using VOYG.CPP.Management.Api.Models.Requests.Registration;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services
{
    public interface IDeviceRegistration
    {
        Task Register(string registrationId, string deviceId, RegistrationRequest registrationRequest);
    }
}