using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Responses.Commands
{
    public class InvokeCommandResponse
    {
        public InvokeCommandResponse(string command, string deviceUid)
        {
            Id = Guid.NewGuid();
            Command = command;
            DeviceUid = deviceUid;
        }

        public Guid Id { get; set; }

        [JsonProperty("action")]
        [JsonRequired]
        public string Command { get; set; }

        [JsonProperty("device")]
        [JsonRequired]
        public string DeviceUid { get; set; }
    }
}
