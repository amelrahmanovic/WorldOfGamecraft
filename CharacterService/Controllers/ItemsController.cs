using CharacterService.DataAccessObject;
using CharacterService.Models.VM.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

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
                ConnectionMultiplexer redis;
                ItemVM itemVM;
                try//try to connect from local machine
                {
                    redis = ConnectionMultiplexer.Connect("localhost,connectTimeout=10000,responseTimeout=10000");
                }
                catch (Exception)//connect to docker
                {
                    redis = ConnectionMultiplexer.Connect("redis:6379,connectTimeout=10000,responseTimeout=10000");
                }

                IDatabase db = redis.GetDatabase();

                var cache = db.StringGet(id.ToString());
                if (cache.HasValue)
                {
                    itemVM = JsonConvert.DeserializeObject<ItemVM>(cache);
                }
                else
                {
                    itemVM = _itemDAO.GetById(id);
                    db.StringSet(id.ToString(), JsonConvert.SerializeObject(itemVM));
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
