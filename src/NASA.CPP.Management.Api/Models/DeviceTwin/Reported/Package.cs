using Newtonsoft.Json;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin.Reported
{
    public class Package
    {
        public Package()
        {
            DownloadFiles = new List<DownloadFile>();
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("restartSw")]
        public bool RestartSw { get; set; }

        [JsonProperty("restartHw")]
        public bool RestartHw { get; set; }

        [JsonProperty("downloadLocation")]
        public string DownloadLocation { get; set; }

        [JsonProperty("preExec")]
        public IEnumerable<string> PreExecCommands { get; set; }

        [JsonProperty("postExec")]
        public IEnumerable<string> PostExecCommands { get; set; }

        [JsonProperty("downloadFiles")]
        public IEnumerable<DownloadFile> DownloadFiles { get; set; }
    }
}
