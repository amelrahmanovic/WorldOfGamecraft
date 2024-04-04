using CharacterService.DataAccessObject;
using CharacterService.Models.VM.Character;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CharacterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private CharacterDAO _characterDAO;
        public CharacterController(CharacterDAO characterDAO)
        {
            _characterDAO = characterDAO;
        }
        //Authorization
        [HttpGet]
        public IEnumerable<CharacterVM> getAll() //Only accessible to Game Masters.
        {
            return _characterDAO.GetAll();
        }
        //Authorization
        [HttpGet("{id}")] //add to cache
        public CharacterAllVM getAll(int id)
        {
            return _characterDAO.GetById(id);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Add(CharacterNewVM characterNewVM)
        {
            _characterDAO.Save(characterNewVM);
            return Created();
        }
    }
}
