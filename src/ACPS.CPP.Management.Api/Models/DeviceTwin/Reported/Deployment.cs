﻿using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin.Reported
{
    public class Deployment
    {
        public Deployment(int deploymentId, Package package)
        {
            DeploymentId = deploymentId;
            Package = package;
        }

        [JsonProperty("deploymentId")]
        public int DeploymentId { get; set; }

        [JsonProperty("package")]
        public Package Package { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
