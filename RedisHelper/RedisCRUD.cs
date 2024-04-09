using StackExchange.Redis;

namespace RedisHelper
{
    public class RedisCRUD
    {
        private ConnectionMultiplexer _redis;
        string _connectionString;

        public RedisCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Save(string Id, string serializedObject)
        {
            OpenConnection();

            bool result;
            var db = _redis.GetDatabase();
            result = db.StringSet(Id, serializedObject);

            _redis.Close();

            return result;
        }
        public string Get(string Id)
        {
            OpenConnection();

            string result = "";
            var db = _redis.GetDatabase();
            result = db.StringGet(Id);
            _redis.Close();

            return result;
        }
        public bool Delete(string Id)
        {
            OpenConnection();

            var db = _redis.GetDatabase();
            bool result = db.KeyDelete(Id);
            _redis.Close();
            return result;
        }
        public bool ExistKey(string Id)
        {
            OpenConnection();

            var db = _redis.GetDatabase();
            bool result = db.KeyExists(Id);
            _redis.Close();

            return result;
        }

        private void OpenConnection()
        {
            try//try to connect from local machine
            {
                _redis = ConnectionMultiplexer.Connect(_connectionString + ",connectTimeout=10000,responseTimeout=10000");//localhost
            }
            catch (Exception)//connect to docker
            {
                _redis = ConnectionMultiplexer.Connect("redis:6379,connectTimeout=10000,responseTimeout=10000");//redis is name container
            }
        }
    }
}