using CharacterService.DataAccessObject;
using CharacterService.Models.VM.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedisHelper;

namespace CharacterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private ItemDAO _itemDAO;
        private readonly ILogger<ItemDAO> _logger;

        public ItemsController(ItemDAO itemDAO, ILogger<ItemDAO> logger)
        {
            _itemDAO = itemDAO;

            _logger = logger;
        }

        [Authorize(Roles = "GameMaster")]
        [HttpGet]
        public List<ItemVM> GetAll() 
        {
            return _itemDAO.GetAll();
        }
        [Authorize(Roles = "User")]
        [HttpGet("{id}")] //add to cache
        public ItemVM getAll(int id)
        {
            try
            {
                #region caching
                string connectionString = "localhost";
                RedisCRUD redisCRUD = new RedisCRUD(connectionString);
                string result = redisCRUD.Get("Item-" + id.ToString());
                ItemVM itemVM;
                if (result!="")
                {
                    itemVM = JsonConvert.DeserializeObject<ItemVM>(result);
                }
                else
                {
                    itemVM = _itemDAO.GetById(id);
                    redisCRUD.Save("Item-" + id.ToString(), JsonConvert.SerializeObject(itemVM));
                }
                #endregion
                return itemVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ItemVM() {Description="", Name="" };
            }
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Add(ItemNewVM itemNewVM)
        {
            _itemDAO.Save(itemNewVM);
            return Created();
        }
        [Authorize(Roles = "User")]
        [HttpPost("grant")]
        public IActionResult GrantItem()
        {
            int? character = null;
            int? item = null;

            if (character == null || item == null)
            {
                return NotFound("Character or item not found.");
            }

            // Grant the item to the character (this logic depends on your application requirements)
            // For example, you might add the item to the character's inventory or update the database records accordingly.

            return Ok("Item granted successfully.");
        }
        [Authorize(Roles = "User")]
        [HttpPost("gift")]
        public IActionResult GiftItem(ItemGiftVM itemGiftVM)
        {
            _itemDAO.GiftItem(itemGiftVM);
            return Created();
        }

    }
}
