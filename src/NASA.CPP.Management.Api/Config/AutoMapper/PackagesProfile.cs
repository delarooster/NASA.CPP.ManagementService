using AutoMapper;
using VOYG.CPP.Models.Entities.Enums;
using System.Linq;

namespace VOYG.CPP.Management.Api.Config.AutoMapper
{
    public class PackagesProfile : Profile
    {
        public PackagesProfile()
        {
            CreateMap<VOYG.CPP.Models.Entities.Package, Models.DeviceTwin.Desired.Package>()
                .ForMember(x => x.Name, s => s.MapFrom(src => src.Name))
                .ForMember(x => x.Version, s => s.MapFrom(src => src.Version))
                .ForMember(x => x.RestartSw, s => s.MapFrom(src => src.RestartSw))
                .ForMember(x => x.RestartHw, s => s.MapFrom(src => src.RestartHw))
                .ForMember(x => x.DownloadLocation, s => s.MapFrom(src => src.DownloadLocation))
                .ForMember(x => x.PreExecCommands, s => s.MapFrom(src => src.ExecCommands.Where(x => x.CommandType == CommandType.Pre).Select(x => x.Command)))
                .ForMember(x => x.PostExecCommands, s => s.MapFrom(src => src.ExecCommands.Where(x => x.CommandType == CommandType.Post).Select(x => x.Command)))
                .ForMember(x => x.DownloadFiles, s => s.Ignore());

            CreateMap<Models.DeviceTwin.Desired.Package, Models.DeviceTwin.Reported.Package>()
                .ReverseMap();
        }
    }
}
