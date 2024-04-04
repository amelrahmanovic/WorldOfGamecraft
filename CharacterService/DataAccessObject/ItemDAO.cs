using AutoMapper;
using CharacterService.Models;
using CharacterService.Models.VM.Item;

namespace CharacterService.DataAccessObject
{
    public class ItemDAO
    {
        private MapperConfiguration config;
        private Mapper mapper;
        private AppDbContex _contex;
        private readonly ILogger<ItemDAO> _logger;

        public ItemDAO(AppDbContex contex, ILogger<ItemDAO> logger)
        {
            _contex = contex;

            config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = new Mapper(config);

            _logger = logger;
        }
        public List<ItemVM> GetAll() 
        {
            try
            {
                return mapper.Map<List<ItemVM>>(_contex.Item.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<ItemVM>();
            }
            
        }
        public ItemVM GetById(int id)
        {
            try
            {
                ItemVM itemVM = mapper.Map<ItemVM>(_contex.Item.FirstOrDefault(x => x.Id == id));
                if (itemVM.BonusAgility > itemVM.BonusStrength
                    && itemVM.BonusAgility > itemVM.BonusIntelligence
                    && itemVM.BonusAgility > itemVM.BonusFaith)
                {
                    itemVM.Name += "  Of The Bear";
                }
                if (itemVM.BonusStrength > itemVM.BonusAgility
                    && itemVM.BonusFaith > itemVM.BonusIntelligence
                    && itemVM.BonusAgility > itemVM.BonusFaith)
                {
                    itemVM.Name += "  Of The Owl";
                }
                if (itemVM.BonusFaith > itemVM.BonusAgility
                    && itemVM.BonusFaith > itemVM.BonusIntelligence
                    && itemVM.BonusFaith > itemVM.BonusStrength)
                {
                    itemVM.Name += "  Of The Unicorn";
                }
                return itemVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ItemVM() {Description="", Name="" };
            }

            
        }
        public void Save(ItemNewVM itemNewVM)
        {
            try
            {
                _contex.Item.Add(mapper.Map<Item>(itemNewVM));
                _contex.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
           
        }
        public void GiftItem(ItemGiftVM itemGiftVM)
        {
            try
            {
                var characterItemFrom = _contex.CharacterItem.SingleOrDefault(x => x.ItemId == itemGiftVM.ItemId && x.CharacterId == itemGiftVM.FromCharacterId);
                if (characterItemFrom != null)
                {
                    characterItemFrom.CharacterId = itemGiftVM.ToCharacterId;
                    _contex.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            } 
        }
    }
}
