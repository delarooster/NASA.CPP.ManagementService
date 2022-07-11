using AutoMapper;
using VOYG.CPP.Management.Api.Models.Responses.Deployment;
using VOYG.CPP.Models.Entities;
using VOYG.CPP.Models.Entities.Enums;
using System;

namespace VOYG.CPP.Management.Api.Config.AutoMapper
{
    public class DeploymentsProfile : Profile
    {
        public DeploymentsProfile()
        {
            CreateMap<Deployment, DeploymentResponse>()
                .ForMember(x => x.Created, s => s.MapFrom(src => src.CreatedUtc))
                .ForMember(x => x.DeviceId, s => s.MapFrom(src => src.DeviceId))
                .ForMember(x => x.Package, s => s.MapFrom(src => new PackageResponse
                {
                    Name = src.Package.Name,
                    Version = src.Package.Version
                }))
                .ForMember(x => x.Status, s => s.MapFrom(src => Enum.GetName(typeof(DeploymentStatus), src.DeploymentStatus)));

            CreateMap<Models.DeviceTwin.Desired.Deployment, Models.DeviceTwin.Reported.Deployment>()
                .ForMember(x => x.Status, s => s.Ignore())
                .ReverseMap();
        }
    }
}
