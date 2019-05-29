using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Healthcare.BC.Offchain.Repository.Models;
using Healthcare.BC.Offchain.Repository.ModelBase;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace Healthcare.BC.Offchain.Repository
{
    public class BusinessTransactionRepository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier> where TEntity : class, IEntityModel<TIdentifier>
    {

        private readonly IMongoDatabase _database;

        public BusinessTransactionRepository(IMongoClient client)
        {
            _database = client.GetDatabase("ContractTransaction");

            if (!BsonClassMap.IsClassMapRegistered(typeof(Profile))) 
                BsonClassMap.RegisterClassMap<Profile>();

        }

        public TEntity Get(TIdentifier id)
        {
            return _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant()).Find(x => x.Id.Equals(id)).FirstOrDefault();
        }

        public TEntity Find(ISpecification<TEntity> specification)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            return collection.Find(specification.Predicate).FirstOrDefaultAsync().Result;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant()).Find(new BsonDocument()).ToList();
        }

        /// <summary>
        /// Not actually used or part of the IRepository interface.
        /// </summary>
        /// <param name="builders"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll(FilterDefinition<TEntity> builders)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            var list = collection.Find(builders).ToList();
            return list;
        }

        public IEnumerable<TEntity> FindAll(ISpecification<TEntity> specification)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            var list = collection.Find(specification.Predicate).ToList();
            return list;
        }

        /// <summary>
        /// Gets all entities for the passed <param name="identifiers"></param> variable.
        /// </summary>
        /// <param name="identifiers">Guids of type TIdentifier to find."</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll(IEnumerable<TIdentifier> identifiers)
        {
            List<TEntity> results = new List<TEntity>();
            IMongoCollection<TEntity> collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());
            foreach (var i in identifiers)
                results.Add(collection.Find(x => x.Id.Equals(i)).FirstOrDefault());
            return results;
        }

        public TEntity Save(TEntity entity)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            collection.ReplaceOne(x => x.Id.Equals(entity.Id), entity, new UpdateOptions
            {
                IsUpsert = true
            });

            return entity;
        }

        public async Task<TEntity> SaveAsync(TEntity entity)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            await collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity, new UpdateOptions
            {
                IsUpsert = true
            });

            return entity;
        }

        public void Delete(TIdentifier address)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            collection.DeleteOne(x => x.Id.Equals(address));
        }

        public void Delete(TEntity entity)
        {
            var collection = _database.GetCollection<TEntity>(typeof(TEntity).Name.ToLowerInvariant());

            collection.DeleteOne(x => x.Id.Equals(entity.Id));
        }
    }
}
