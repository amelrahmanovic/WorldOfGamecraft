using CharacterService.DataAccessObject;
using CharacterService.Models.VM.Character;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "GameMaster")]
        [HttpGet]
        public IEnumerable<CharacterVM> getAll() //Only accessible to Game Masters.
        {
            return _characterDAO.GetAll();
        }
        [Authorize(Roles = "GameMaster,User")]
        [HttpGet("{id}")] //add to cache
        public CharacterAllVM getAll(int id)
        {
            return _characterDAO.GetById(id);
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Add(CharacterNewVM characterNewVM)
        {
            var userClaims = User.Claims;
            var userIdClaim = userClaims.FirstOrDefault(c => c.Type == "Id");
            if (userIdClaim != null)
            {
                characterNewVM.CreatedBy = userIdClaim.Value;
                _characterDAO.Save(characterNewVM);
                return Created();
            }
            else
            {
                return BadRequest("Id user not found.");
            }
        }
    }
}
