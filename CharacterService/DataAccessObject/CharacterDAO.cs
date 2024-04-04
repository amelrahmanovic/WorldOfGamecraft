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
        private readonly ILogger<ItemDAO> _logger;

        public CharacterDAO(AppDbContex contex, ILogger<ItemDAO> logger) 
        {
            _contex = contex;

            config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = new Mapper(config);

            _logger = logger;
        }

        public List<CharacterVM> GetAll()
        {
            try
            {
                return mapper.Map<List<CharacterVM>>(_contex.Character.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<CharacterVM>();
            }
            
        }
        public CharacterAllVM GetById(int id)
        {
            try
            {
                CharacterAllVM characterAllVM = mapper.Map<CharacterAllVM>(_contex.Character.Include(x => x.Class).SingleOrDefault(x => x.Id == id));
                List<CharacterItem> characterItems = _contex.CharacterItem.Include(x => x.Item).Where(x => x.CharacterId == id).ToList();
                foreach (var item in characterItems)
                {
                    characterAllVM.statsBonus += item.Item.BonusFaith + item.Item.BonusStrength + item.Item.BonusAgility + item.Item.BonusIntelligence;
                }

                return characterAllVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new CharacterAllVM();
            }
        }
        public void Save(CharacterNewVM characterNewVM)
        {
            try
            {
                Character character = mapper.Map<Character>(characterNewVM);
                _contex.Character.Add(character);
                _contex.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
