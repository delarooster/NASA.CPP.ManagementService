namespace VOYG.CPP.Management.Api.Config.Options
{
    public class StorageOptions
    {
        public const string Storage = "Storage";

        public string D2CDeploymentStatusTableName { get; set; }
        public string D2CManifestDeploymentStatusTableName { get; set; }
    }
}
