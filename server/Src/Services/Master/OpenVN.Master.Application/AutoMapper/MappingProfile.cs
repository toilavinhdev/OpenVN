using AutoMapper;
using OpenVN.Master.Domain;

namespace OpenVN.Master.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<App, AppDto>().ForMember(des => des.HasLicense, opt => opt.MapFrom(src => true));
        }
    }
}
