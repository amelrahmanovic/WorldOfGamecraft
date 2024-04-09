using AutoMapper;
using SharingModels.ModelsVM;

namespace CombatService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<ApplicationUserVM, ApplicationUser>();
        }
    }
}
