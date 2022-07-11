using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin
{
    public class DeviceTwin
    {
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public class Properties
    {
        [JsonProperty("desired")]
        public DesiredProperties Desired { get; set; }

        [JsonProperty("reported")]
        public ReportedProperties Reported { get; set; }
    }

    public class DesiredProperties
    {
        [JsonProperty("deployment", NullValueHandling = NullValueHandling.Ignore)]
        public Desired.Deployment Deployment { get; set; }

        [JsonProperty("manifestDeployment", NullValueHandling = NullValueHandling.Ignore)]
        public Desired.ManifestDeployment ManifestDeployment { get; set; }
    }

    public class ReportedProperties
    {
        [JsonProperty("deployment")]
        public Reported.Deployment Deployment { get; set; }

        [JsonProperty("manifestDeployment")]
        public Reported.ManifestDeployment ManifestDeployment { get; set; }
    }
}
