using AutoMapper;
using CombatService.DataAccessObject;
using CombatService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQHelper;
using SharingModels.ModelsVM;
using ApplicationUser = CombatService.Models.ApplicationUser;
using MappingProfile = CombatService.Models.MappingProfile;

namespace CombatService.BacgroundServices
{
    public class UserService : BackgroundService
    {
        private AppDbContex _appDbContex;
        private ApplicationUserDAO _applicationUserDAO;
        private MapperConfiguration config;
        private Mapper mapper;

        //public UserService(ApplicationUserDAO applicationUserDAO)
        //{
        //    _applicationUserDAO = applicationUserDAO;

        //    config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        //    mapper = new Mapper(config);
        //}
        public UserService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContex>();
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MSSQLDBConnection"));
            DbContextOptions<AppDbContex> options = optionsBuilder.Options;
            _appDbContex = new AppDbContex(options);
            _applicationUserDAO = new ApplicationUserDAO(_appDbContex);

            config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = new Mapper(config);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                RabbitMQCRUD rabbitMQCRUD = new RabbitMQCRUD("localhost", "rabbitmq", "rabbitmq");
                if(rabbitMQCRUD.ExistQueue("users"))
                {
                    var message = rabbitMQCRUD.GetQueues("users", false, true);
                    List<ApplicationUserVM>? applicationUsersVM = JsonConvert.DeserializeObject<List<ApplicationUserVM>>(message);
                    if (applicationUsersVM != null)
                    {
                        List<ApplicationUser> applicationUsers =  mapper.Map<List<ApplicationUser>>(applicationUsersVM);
                        foreach (var item in applicationUsers)
                        {
                            _applicationUserDAO.Save(item);
                        }
                    }
                    
                    Console.WriteLine("Bacground proccess----------------------->" + DateTime.Now.ToString() + " " + message);
                }
                else
                    Console.WriteLine("Bacground proccess----------------------->" + DateTime.Now.ToString());
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
