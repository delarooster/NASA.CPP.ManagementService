using VOYG.CPP.Management.Api.Models.Requests.Commands;
using VOYG.CPP.Management.Api.Models.Responses.Commands;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Controllers
{
    public class CommandsController : BaseController
    {
        private readonly ICommandsService _commandsService;

        public CommandsController(ICommandsService commandsService)
        {
            _commandsService = commandsService;
        }

        [HttpPost("/commands/")]
        [ProducesResponseType(typeof(InvokeCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InvokeCommand(InvokeCommandRequest invokeCommandRequest, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _commandsService.InvokeSetFileUpload(invokeCommandRequest, cancellationToken));
        }
    }
}
