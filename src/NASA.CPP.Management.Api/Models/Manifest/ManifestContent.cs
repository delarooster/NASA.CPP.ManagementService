using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Manifest
{
    public class ManifestContent
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public Item[] Items { get; set; }
    }

    public class Item
    {
        public string ItemId { get; set; }
        public Child[] Children { get; set; }
        public string ItemType { get; set; }
        public string CreatedBy { get; set; }
        public string ProfileId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
        public string ProfileName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class Child
    {
        public string ItemId { get; set; }
        public GeofencingChild[] Children { get; set; }
        public string ItemType { get; set; }
        public SetFileContent SetFileContent { get; set; }
        public bool Enabled { get; set; }
    }

    public class SetFileContent
    {
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
    }

    public class GeofencingChild
    {
        public string ItemId { get; set; }
        public string GeoJSON { get; set; }
        public GeofencingTokenChild[] Children { get; set; }
        public string ItemType { get; set; }
        public string ZoneName { get; set; }
        public string ZoneType { get; set; }
        public string ZoneColour { get; set; }
        public Zonecoord[] ZoneCoords { get; set; }
        public bool? ZoneVisibility { get; set; }
        public int? Canlink { get; set; }
    }

    public class GeofencingTokenChild
    {
        public string ItemId { get; set; }
        public object[] Children { get; set; }
        public string ItemType { get; set; }
        public Tokencoord[] TokenCoords { get; set; }
    }

    public class Tokencoord
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Zonecoord
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}