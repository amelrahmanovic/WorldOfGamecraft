using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQHelper
{
    public class RabbitMQCRUD
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private string HostName, UserName, Password;

        public RabbitMQCRUD(string HostName, string UserName, string Password)
        {
            this.HostName = HostName;
            this.UserName = UserName;
            this.Password = Password;
            //OpenConnection();
        }

        private void OpenConnection()
        {
            factory = new ConnectionFactory()
            {
                //HostName = HostName, // Docker host
                UserName = UserName, // RabbitMQ username
                Password = Password // RabbitMQ password
            };
            try//try to connect from local machine
            {
                factory.HostName = HostName;
                connection = factory.CreateConnection();
            }
            catch (Exception)//connect to docker
            {
                factory.HostName = "rabbitmq";
                connection = factory.CreateConnection();
            }
        }

        public void NewQueues(string nameQueue, string json)
        {
            OpenConnection();
            var channel = connection.CreateModel();

            //channel.QueueDelete(queue: nameQueue);

            channel.QueueDeclare(queue: nameQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            channel.BasicPublish(exchange: "",
                                 routingKey: nameQueue,
                                 basicProperties: null,
                                 body: Encoding.UTF8.GetBytes(json));
            channel.Close();
        }
        public int CountQueuesMessages(string nameQueues)
        {
            OpenConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: nameQueues,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var result = channel.QueueDeclarePassive(nameQueues);

                connection.Close();

                return (int)result.MessageCount;
            }
        }
        public void DeleteQueue(string nameQueue)
        {
            OpenConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDelete(queue: nameQueue);
            }
            connection.Close();
        }
        public string GetQueues(string nameQueue, bool autoAck, bool AckMessage)
        {
            string message = "";
            try
            {
                OpenConnection();
                using (var channel = connection.CreateModel())
                {
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
                        message = Encoding.UTF8.GetString(body.ToArray());
                        try
                        {
                            if (AckMessage)
                                channel.BasicAck(ea.DeliveryTag, false);

                        }
                        catch (Exception) { }
                    };

                    // Start consuming messages
                    string consumerTag = channel.BasicConsume(queue: nameQueue,
                                         autoAck: autoAck,
                                         consumer: consumer);
                }
                connection.Close();
            }
            catch (Exception) { }
            return message;
        }
        public bool ExistQueue(string nameQueue)
        {
            OpenConnection();
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclarePassive(nameQueue);
                    connection.Close();
                    return true;
                }
                catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
                {
                    connection.Close();
                    return false;
                }
            }

        }
    }
}