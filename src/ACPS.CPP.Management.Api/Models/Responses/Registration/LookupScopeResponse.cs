using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Responses.Registration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LookupScopeResponse
    {
        [JsonProperty("scope-id")]
        public string ScopeId { get; set; }
    }
}
