﻿using CharacterService.DataAccessObject;
using CharacterService.Models.VM.Character;
using CharacterService.Models.VM.Item;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharacterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private ItemDAO _itemDAO;
        public ItemsController(ItemDAO itemDAO)
        {
            _itemDAO = itemDAO;
        }
        [HttpGet]
        public List<ItemVM> GetAll() 
        {
            return _itemDAO.GetAll();
        }
        [HttpGet("{id}")] //add to cache
        public ItemVM getAll(int id)
        {
            return _itemDAO.GetById(id);
        }
        [HttpPost]
        public IActionResult Add(ItemNewVM itemNewVM)
        {
            _itemDAO.Save(itemNewVM);
            return Created();
        }
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
        [HttpPost("gift")]
        public IActionResult GiftItem(ItemGiftVM itemGiftVM)
        {
            _itemDAO.GiftItem(itemGiftVM);
            return Created();
        }

    }
}
