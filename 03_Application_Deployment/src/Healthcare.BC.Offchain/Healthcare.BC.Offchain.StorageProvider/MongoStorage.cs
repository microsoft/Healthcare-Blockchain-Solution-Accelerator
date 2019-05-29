using Microsoft.Extensions.Configuration;
using System;

namespace Healthcare.BC.Offchain.StorageProvider
{
    public class MongoStorage
    {
        protected string mongoconnString;

        public MongoStorage()
        {
            readConnectionString();
        }

        private void readConnectionString() {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            try
            {
               this.mongoconnString = config["offichain_connectionstring"];
            }
            catch (Exception)
            {
                throw new NullReferenceException("'offichain_connectionstring' was not assigned in appsettings.config file");
            }
     
        }

    }
}
