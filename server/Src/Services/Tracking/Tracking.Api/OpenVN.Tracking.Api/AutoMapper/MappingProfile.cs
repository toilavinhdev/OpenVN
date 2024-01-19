using AutoMapper;
using OpenVN.TrackingApi.Dto;
using SharedKernel.Domain;
using SharedKernel.Libraries;

namespace OpenVN.TrackingApi.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TrackingDto, Tracking>().ForMember(des => des.Time, opt => opt.MapFrom(src => DateHelper.ConvertMillisecondsToDate(src.Time)));
        }
    }
}
