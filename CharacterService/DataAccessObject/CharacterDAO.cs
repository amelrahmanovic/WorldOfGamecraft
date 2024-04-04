using AutoMapper;
using CharacterService.Models;
using CharacterService.Models.VM.Character;
using Microsoft.EntityFrameworkCore;

namespace CharacterService.DataAccessObject
{
    public class CharacterDAO
    {
        private MapperConfiguration config;
        private Mapper mapper;
        private AppDbContex _contex;

        public CharacterDAO(AppDbContex contex) {
            _contex = contex;

            config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = new Mapper(config);
        }

        public List<CharacterVM> GetAll()
        {
            return mapper.Map<List<CharacterVM>>(_contex.Character.ToList());
        }
        public CharacterAllVM GetById(int id)
        {
            CharacterAllVM characterAllVM = mapper.Map<CharacterAllVM>(_contex.Character.Include(x=>x.Class).SingleOrDefault(x=>x.Id==id));
            List<CharacterItem> characterItems = _contex.CharacterItem.Include(x=>x.Item).Where(x=>x.CharacterId==id).ToList();
            foreach (var item in characterItems)
            {
                characterAllVM.statsBonus += item.Item.BonusFaith + item.Item.BonusStrength + item.Item.BonusAgility + item.Item.BonusIntelligence;
            }

            return characterAllVM;
        }
        public void Save(CharacterNewVM characterNewVM)
        {
            Character character = mapper.Map<Character>(characterNewVM);
            _contex.Character.Add(character);
            _contex.SaveChanges();
        }
    }
}
