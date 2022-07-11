using AutoMapper;
using VOYG.CPP.Management.Api.Models.Responses.Status;
using VOYG.CPP.Management.Api.Models.TableStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Config.AutoMapper
{
    public class DeploymentStatusProfile : Profile
    {
        public DeploymentStatusProfile()
        {
            CreateMap<DeploymentStatus, StatusResponse>()
                .ForMember(dst => dst.TimeStamp, y => y.Ignore());
        }
    }
}
