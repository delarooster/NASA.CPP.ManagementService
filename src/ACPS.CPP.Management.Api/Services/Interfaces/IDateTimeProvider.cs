using System;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime UnixEpoch { get; }
    }
}
