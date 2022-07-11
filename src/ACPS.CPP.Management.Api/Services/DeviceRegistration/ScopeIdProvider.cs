namespace VOYG.CPP.Management.Api.Services.DeviceRegistration
{
    public interface IScopeIdProvider
    {
        string Provide(string deviceId);
    }

    public class ScopeIdProvider : IScopeIdProvider
    {
        private readonly string _scopeId;

        public ScopeIdProvider(string scopeId)
        {
            _scopeId = scopeId;
        }

        public string Provide(string deviceId)
        {
            // Because of one DPS, there is one ScopeId for each deviceId
            return _scopeId;
        }
    }
}
