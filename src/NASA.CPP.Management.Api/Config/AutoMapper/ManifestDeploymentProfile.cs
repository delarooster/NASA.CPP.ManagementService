using AutoMapper;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.DeviceTwin.Desired;
using VOYG.CPP.Management.Api.Models.Responses.ManifestDeployment;
using VOYG.CPP.Models.Entities.Enums;
using System;

namespace VOYG.CPP.Management.Api.Config.AutoMapper
{
    public class ManifestDeploymentProfile : Profile
    {
        public ManifestDeploymentProfile()
        {
            CreateMap<BlobFile, Manifest>()
                .ForMember(x => x.BlobFileMd5Hash, s => s.MapFrom(src => src.BlobFileMd5Hash))
                .ForMember(x => x.BlobFileSasUrl, s => s.MapFrom(src => src.BlobFileSasUrl))
                .ForMember(x => x.ContentLength, s => s.MapFrom(src => src.ContentLength));

            CreateMap<CPP.Models.Entities.ManifestDeployment, ManifestDeploymentResponse>()
                .ForMember(x => x.Id, s => s.MapFrom(src => src.Id))
                .ForMember(x => x.Strategy, s => s.MapFrom(src => Enum.GetName(typeof(ManifestDeploymentStrategy), src.Strategy).ToLower()))
                .ForMember(x => x.CreatedUtc, s => s.MapFrom(src => src.CreatedUtc))
                .ForMember(x => x.DeviceId, s => s.MapFrom(src => src.DeviceId))
                .ForMember(x => x.ManifestId, s => s.MapFrom(src => src.ManifestId))
                .ForMember(x => x.Tag, s => s.MapFrom(src => src.Tag));
        }
    }
}
