using VOYG.CPP.Management.Api.Helpers;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.Registration;
using VOYG.CPP.Management.Api.Models.Responses.Registration;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using VOYG.CPP.Models;
using System.Linq;
using VOYG.CPP.Management.Api.Services.DeviceRegistration;
using VOYG.CPP.Management.Api.Services.DeviceRegistration.Interfaces;
using VOYG.CPP.Management.Api.Extensions;

namespace VOYG.CPP.Management.Api.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IEnumerable<IDeviceRegistration> _registrationTasks;
        private readonly IDpsClient _dpsClient;
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private readonly IScopeIdProvider _scopeIdProvider;

        public RegistrationService(
            IEnumerable<IDeviceRegistration> registrationTasks,
            Func<IUnitOfWork> unitOfWorkFactory,
            IScopeIdProvider scopeIdProvider, 
            IDpsClient dpsClient)
        {
            _registrationTasks = registrationTasks;
            _unitOfWorkFactory = unitOfWorkFactory;
            _scopeIdProvider = scopeIdProvider;
            _dpsClient = dpsClient;
        }

        public async Task<IServiceResult<RegistrationResponse>> Register(string registrationId, RegistrationRequest registrationRequest, CancellationToken cancellationToken)
        {
            var existsInDps = await _dpsClient.DoesExist(registrationId);
            if (existsInDps)
            {
                return ResponseHelper.UnsuccessfulResult<RegistrationResponse>(
                    new Dictionary<string, string>() { { "duplicated", $"Device already registered for registrationId {registrationId}" } },
                    StatusCodes.Status400BadRequest);
            }

            string deviceId = !string.IsNullOrEmpty(registrationRequest.IoTHubDeviceId) 
                ? registrationRequest.IoTHubDeviceId 
                : registrationId;

            var existsInDb = await ExistsAsync(deviceId);
            if (existsInDb)
            {
                deviceId = GetIncementedDeviceId(deviceId);
            }

            foreach (var registrationTask in _registrationTasks)
            {
                await registrationTask.Register(registrationId, deviceId, registrationRequest);
            }

            return ResponseHelper.SuccessfulResult(new RegistrationResponse
            {
                DeviceId = deviceId,
                RegistrationId = registrationId,
                Status = DeviceRegistrationState.Registered.ToString()
            },
            StatusCodes.Status201Created);
        }

        public IServiceResult<RegistrationStatusResponse> GetRegistrationStatus(string deviceId)
        {
            var unitOfWork = _unitOfWorkFactory.Invoke();

            var deviceLifecycle = unitOfWork.DeviceLifecycleRepository.GetLastForDeviceId(deviceId);

            if (deviceLifecycle == null)
            {
                return ResponseHelper.UnsuccessfulResult<RegistrationStatusResponse>(
                    new Dictionary<string, string>() { { "Not found", $"Not found registration status for device {deviceId}" } },
                    StatusCodes.Status404NotFound);
            }

            return ResponseHelper.SuccessfulResult(new RegistrationStatusResponse
            {
                DeviceId = deviceId,
                Status = deviceLifecycle.State
            },
            StatusCodes.Status200OK);
        }

        public IServiceResult<LookupScopeResponse> LookupScope(string deviceId)
        {
            var scope = _scopeIdProvider.Provide(deviceId);

            return ResponseHelper.SuccessfulResult(new LookupScopeResponse()
            {
                ScopeId = scope
            },
            StatusCodes.Status200OK);
        }

        private async Task<bool> ExistsAsync(string id)
        {
            var unitOfWork = _unitOfWorkFactory.Invoke();

            var result = await unitOfWork.DeviceRepository.GetAsync(x => x.Id == id);

            return result?.Any() ?? false;
        }

        private string GetIncementedDeviceId(string deviceId)
        {
            var unitOfWork = _unitOfWorkFactory.Invoke();
            var devicePrefix = deviceId.RemoveNumbersFromString();
            var lastDeviceWithPrefix = unitOfWork.DeviceRepository.GetFirstFilteredAndOrderedDescById(devicePrefix);

            var newDeviceNumber = lastDeviceWithPrefix?.Id.GetNumberFromString() + 1;
            return $"{devicePrefix}{newDeviceNumber:D3}";
        }
    }
}