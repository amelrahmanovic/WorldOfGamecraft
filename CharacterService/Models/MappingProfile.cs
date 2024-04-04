using AutoMapper;
using CharacterService.Models.VM.Character;
using CharacterService.Models.VM.Item;

namespace CharacterService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Character
            CreateMap<Character, CharacterVM>();
            CreateMap<Character, CharacterAllVM>()
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class.Name));
            CreateMap<CharacterNewVM, Character>();
            #endregion
            #region Item
            CreateMap<Item, ItemVM>();
            CreateMap<ItemNewVM, Item>();
            #endregion
        }
    }
}
