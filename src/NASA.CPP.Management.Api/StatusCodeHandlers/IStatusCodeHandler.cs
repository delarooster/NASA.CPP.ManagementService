using Microsoft.AspNetCore.Mvc;

namespace VOYG.CPP.Management.Api.Interfaces
{
    public interface IStatusCodeHandler
    {
        IActionResult HandleReponse(object message);
    }
}