using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.DeviceRegistration.Interfaces
{
    public interface IDpsClient
    {
        Task<bool> DoesExist(string id);
    }
}
