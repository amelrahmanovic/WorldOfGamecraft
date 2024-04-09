using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQHelper;
using SharingModels.ModelsVM;
using System.Text;

namespace CombatService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private List<ApplicationUserVM>? applicationUsers;

        public ChallengeController()
        {
        }
        [HttpGet]
        public ActionResult GetAll() 
        {
            #region RabbitMQ
            RabbitMQCRUD rabbitMQCRUD = new RabbitMQCRUD("localhost", "rabbitmq", "rabbitmq");
            var message = rabbitMQCRUD.GetQueues("users", false, true);
            applicationUsers = JsonConvert.DeserializeObject<List<ApplicationUserVM>>(message);
            #endregion

            return Ok(applicationUsers);
        }
    }
}
