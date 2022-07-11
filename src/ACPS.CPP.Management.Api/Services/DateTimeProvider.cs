using VOYG.CPP.Management.Api.Services.Interfaces;
using System;

namespace VOYG.CPP.Management.Api.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UnixEpoch => DateTime.UnixEpoch;
    }
}
