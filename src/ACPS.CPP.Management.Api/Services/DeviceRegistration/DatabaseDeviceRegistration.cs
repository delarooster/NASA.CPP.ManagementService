using VOYG.CPP.Management.Api.Models.Requests.Registration;
using VOYG.CPP.Models;
using System;
using System.Threading.Tasks;
using Device = VOYG.CPP.Models.Entities.Device;
using VOYG.CPP.Models.Entities;
using VOYG.CPP.Management.Api.Models;

namespace VOYG.CPP.Management.Api.Services
{
    public class DatabaseDeviceRegistration : IDeviceRegistration
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public DatabaseDeviceRegistration(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Register(string registrationId, string deviceId, RegistrationRequest registrationRequest)
        {
            var unitOfWork = _unitOfWorkFactory.Invoke();

            var device = new Device
            {
                Id = deviceId,
                Operator = registrationRequest.Operator,
                PartNumberId = registrationRequest.PartNumber
            };

            await unitOfWork.DeviceRepository.AddAsync(device);

            await unitOfWork.DeviceLifecycleRepository.AddAsync(new DeviceLifecycle
            {
                DeviceId = deviceId,
                State = DeviceRegistrationState.Registered.ToString()
            });

            await unitOfWork.DeviceDetailsRepository.AddAsync(new DeviceDetails
            {
                DeviceId = deviceId,
                Imei = registrationRequest.Imei,
                SerialNumber = registrationRequest.SerialNumber,
                Ccid = registrationRequest.Ccid,
                Imsi = registrationRequest.Imsi,
                Description = registrationRequest.Desciption,
                PppIp = registrationRequest.Ppp_Ip,
            });

            await unitOfWork.SaveChangesAsync();
        }
    }
}