using VOYG.CPP.Management.Api.Helpers;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.Commands;
using VOYG.CPP.Management.Api.Models.Responses.Commands;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services
{
    public class CommandsService : ICommandsService
    {
        private readonly ServiceClient _serviceClient;
        private readonly HashSet<string> AllowedMethods = new HashSet<string>
        {
            "SOFTWARE_RESTART", "Start Collection", "STOP", "RESTART", "ECHO"
        };

        public CommandsService(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task<IServiceResult<InvokeCommandResponse>> InvokeSetFileUpload(InvokeCommandRequest invokeCommandRequest, CancellationToken cancellationToken)
        {
            AllowedMethods.TryGetValue(invokeCommandRequest.Command, out var allowedMethod);

            if (string.IsNullOrEmpty(allowedMethod))
            {
                return ResponseHelper.UnsuccessfulResult<InvokeCommandResponse>(new Dictionary<string, string>() { { "action", $"{invokeCommandRequest.Command} is not a valid choice." } },
                    StatusCodes.Status400BadRequest);
            }

            try
            {
                var methodInvocation = new CloudToDeviceMethod(invokeCommandRequest.Command) { ResponseTimeout = TimeSpan.FromSeconds(30) };
                await _serviceClient.InvokeDeviceMethodAsync(invokeCommandRequest.DeviceUid, methodInvocation, cancellationToken);
            }
            catch
            {
                return ResponseHelper.SuccessfulResult(new InvokeCommandResponse(invokeCommandRequest.Command, invokeCommandRequest.DeviceUid), StatusCodes.Status201Created);
            }

            return ResponseHelper.SuccessfulResult(new InvokeCommandResponse(invokeCommandRequest.Command, invokeCommandRequest.DeviceUid), StatusCodes.Status201Created);
        }
    }
}
