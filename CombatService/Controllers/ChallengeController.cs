using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharingModels;
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
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // Docker host
                UserName = "rabbitmq", // RabbitMQ username
                Password = "rabbitmq" // RabbitMQ password
            };

            IConnection connection;
            try//try to connect from local machine
            {
                factory.HostName = "localhost";
                connection = factory.CreateConnection();
            }
            catch (Exception)//connect to docker
            {
                factory.HostName = "rabbitmq";
                connection = factory.CreateConnection();
            }
            using (var channel = connection.CreateModel())
            {
                // Declare the queue
                channel.QueueDeclare(queue: "users",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Create a consumer
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    applicationUsers = JsonConvert.DeserializeObject<List<ApplicationUserVM>>(message);
                };

                // Start consuming messages
                channel.BasicConsume(queue: "users",
                                     autoAck: false,
                                     consumer: consumer);
            }

            return Ok(applicationUsers);
        }
    }
}
