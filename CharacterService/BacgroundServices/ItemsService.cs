using AutoMapper;
using CharacterService.DataAccessObject;
using CharacterService.Models.VM.Item;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RedisHelper;

namespace CharacterService.BacgroundServices
{
    public class ItemsService : BackgroundService
    {
        private AppDbContex _appDbContex;
        private ItemDAO _itemDAO;
        private MapperConfiguration config;

        public ItemsService(ILogger<ItemDAO> logger)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContex>();
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MSSQLDBConnection"));
            DbContextOptions<AppDbContex> options = optionsBuilder.Options;
            _appDbContex = new AppDbContex(options);
            _itemDAO = new ItemDAO(_appDbContex, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var ItemsDB = _itemDAO.GetAll();
                RedisCRUD redisCRUD = new RedisCRUD("localhost");
                bool addedNew = false;
                foreach (var item in ItemsDB)
                {
                    if (!redisCRUD.ExistKey("Item-" + item.Id.ToString()))
                    {
                        addedNew=true;
                        redisCRUD.Save("Item-" + item.Id.ToString(), JsonConvert.SerializeObject(item));
                    }
                }

                Console.WriteLine("Bacground proccess----------------------->" + DateTime.Now.ToString()+" Added: "+ addedNew.ToString());
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
