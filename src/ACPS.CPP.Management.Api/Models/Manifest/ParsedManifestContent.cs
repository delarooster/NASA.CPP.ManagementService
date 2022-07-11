using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Manifest
{
    public class ParsedManifestContent
    {
        public ParsedManifestContent(SetFileContent setFileContent)
        {
            Globals = setFileContent.Globals;
            Metadata = setFileContent.Metadata;
            Shiftsave = setFileContent.Shiftsave;
            Comments = setFileContent.Comments;
            Canlink = setFileContent.Canlink;
            Ecu = setFileContent.Ecu;
            Ad = setFileContent.Ad;
            StoreControl = setFileContent.StoreControl;
            Tables = setFileContent.Tables;
        }

        public dynamic Globals { get; set; }
        public dynamic Metadata { get; set; }
        public dynamic Shiftsave { get; set; }
        public dynamic Comments { get; set; }
        public dynamic Canlink { get; set; }
        public dynamic Ecu { get; set; }
        [JsonProperty("a/d")]
        public dynamic Ad { get; set; }
        [JsonProperty("store_control")]
        public dynamic StoreControl { get; set; }
        public dynamic Tables { get; set; }
        public Geo? Geo { get; set; }
    }

    public class Geo
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public Mode[] Modes { get; set; }
        public Zones Zones { get; set; }
    }

    public class Zones
    {
        public string Type { get; set; }
        public Feature[] Features { get; set; }
    }

    public class Feature
    {
        public string Type { get; set; }
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; }
        public string[] Polylines { get; set; }
    }

    public class Properties
    {
        public string Description { get; set; }
        [JsonProperty("zone-id")]
        public string Zoneid { get; set; }
        [JsonProperty("zone-type")]
        public string Zonetype { get; set; }
        [JsonProperty("zone-data")]
        public ZoneData Zonedata { get; set; }
    }

    public class ZoneData
    {
        public string Mode { get; set; }
        [JsonProperty("zone-id-ref")]
        public string[] Zoneidref { get; set; }
    }

    public class Mode
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public Output Output { get; set; }
    }

    public class Output
    {
        public string Link { get; set; }
        public string Msg { get; set; }
        public string Rate { get; set; }
    }
}