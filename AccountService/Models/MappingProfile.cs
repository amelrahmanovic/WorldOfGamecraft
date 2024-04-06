using AutoMapper;
using SharingModels;

namespace AccountService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserVM>();
        }
    }
}
