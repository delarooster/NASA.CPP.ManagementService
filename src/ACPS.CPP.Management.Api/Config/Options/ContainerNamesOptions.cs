namespace VOYG.CPP.Management.Api.Config.Options
{
    public class ContainerNamesOptions
    {
        public const string ContainerNames = "ContainerNames";

        public string Packages { get; set; }

        public string Manifests { get; set; }

        public string ParsedManifests { get; set; }
    }
}
