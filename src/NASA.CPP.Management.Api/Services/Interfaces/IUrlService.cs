namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IUrlService
    {
        string GenerateUrl(string action, string controller, object values);
    }
}