using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VOYG.CPP.Management.Api.Models
{
    [ExcludeFromCodeCoverage]
    [JsonObject(MemberSerialization.OptIn)]
    public class ErrorResponse
    {
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}