using AutoMapper;
using Newtonsoft.Json;
using SharedKernel.Libraries;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth
            CreateMap<User, TokenUser>().ForMember("Item", opt => opt.Ignore());

            // Config
            CreateMap<UserConfig, UserConfigDto>()
                .ForMember(des => des.ConfigValue, options => options.MapFrom(src => JsonConvert.DeserializeObject<ConfigValue>(src.Json)));

            // User
            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserDto>();

            // Process
            CreateMap<ProcessDto, Process>().ReverseMap();

            // Asset
            CreateMap<SpendingDto, Spending>().ReverseMap();
            CreateMap<SpendingDto, SpendingTree>().ReverseMap();
            CreateMap<SpendingTree, Spending>().ReverseMap();

            // Notebooke
            CreateMap<NoteDto, Note>().ReverseMap();
            CreateMap<NoteCategoryDto, NoteCategory>().ReverseMap();

            // Cloud
            CreateMap<DirectoryDto, Directory>();
            CreateMap<Directory, DirectoryDto>().ForMember(des => des.Name, options => options.MapFrom(src => src.Name.ToBase64Decode()));
            CreateMap<CloudFileDto, CloudFile>().ReverseMap();

            // Feedback
            CreateMap<FeedbackDto, Feedback>().ReverseMap();
        }
    }
}
