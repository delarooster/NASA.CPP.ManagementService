using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VOYG.CPP.Management.Api.Services
{
    public class UrlService : IUrlService
    {
        private const string SuffixHeader = "X-Suffix";
        private const string Slash = "/";

        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlService(IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateUrl(string action, string controller, object values)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return _urlHelper.Action(action, controller, values);
            }

            var fullScheme = $"{_httpContextAccessor.HttpContext.Request.Scheme}:{Slash}{Slash}";
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(SuffixHeader, out var suffix);
            var host = BuildHostString(_httpContextAccessor.HttpContext.Request.Host.Value, fullScheme, suffix);
            return _urlHelper.Action(action, controller, values, _httpContextAccessor.HttpContext.Request.Scheme, host);
        }

        private static string BuildHostString(string host, string scheme, string? suffix)
        {
            if (host.EndsWith(Slash))
            {
                host = host.Remove(host.Length - 1, 1);
            }

            if (host.StartsWith(scheme))
            {
                host = host.Replace(scheme, string.Empty);
            }

            if (suffix == null) return host;

            if (suffix.StartsWith(Slash))
            {
                suffix = suffix.Remove(0, 1);
            }

            if (suffix.EndsWith(Slash))
            {
                suffix = suffix.Remove(suffix.Length - 1, 1);
            }

            host = $"{host}/{suffix}";

            return host;
        }
    }
}