using AutoMapper;
using VOYG.CPP.Management.Api.Models;

namespace VOYG.CPP.Management.Api.Config.AutoMapper
{
    public class DownloadFilesProfile : Profile
    {
        public DownloadFilesProfile()
        {
            CreateMap<Models.DeviceTwin.Desired.DownloadFile, Models.DeviceTwin.Reported.DownloadFile>()
                .ReverseMap();

            CreateMap<BlobFile, Models.DeviceTwin.Desired.DownloadFile>()
                .ForMember(x => x.BlobFileMd5Hash, s => s.MapFrom(src => src.BlobFileMd5Hash))
                .ForMember(x => x.BlobFileSasUrl, s => s.MapFrom(src => src.BlobFileSasUrl))
                .ForMember(x => x.ContentLength, s => s.MapFrom(src => src.ContentLength))
                .ForMember(x => x.FileName, s => s.MapFrom(src => src.FileName));
        }
    }
}
