using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Conventions;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System.Configuration;
using ShouldITakeMyDogToFortFunstonNow.Models;

namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public class MongoDbRepoService : IRepoService
    {
        private static readonly string MODEL_NAMESPACE = "ShouldITakeMyDogToFortFunstonNow.Models";
        private static readonly string DATABASE_NAME = "fortfunstonweather";
        private static readonly string CONNECTION_STRING = ConfigurationManager.AppSettings["dbPath"];
        private static readonly string OBSERVATION_COLLECTION_NAME = "observations";

        private MongoServer server;
        private MongoDatabase db;

        static MongoDbRepoService()
        {            
            InitConventions();            
        }

        public MongoDbRepoService()
        {
            server = MongoServer.Create(CONNECTION_STRING);
            db = server.GetDatabase(DATABASE_NAME);
        }


        private static void InitConventions()
        {
            var myConventions = new ConventionProfile();
            myConventions.SetIdMemberConvention(new IsKeyConvention());
            BsonClassMap.RegisterConventions(
                myConventions,
                t => t.Namespace.StartsWith(MODEL_NAMESPACE)
            );

            BsonSerializer.RegisterIdGenerator(
                typeof(object),
                new ObjectIdGenerator()
            );
        }


        public IEnumerable<CurrentObservation> GetAllObservations()
        {
            using (server.RequestStart(db))
            {
                var collection = db.GetCollection<CurrentObservation>(OBSERVATION_COLLECTION_NAME);
                return collection.FindAll().AsEnumerable();
            }
        }


        public void AddObservation(CurrentObservation obs)
        {
            using (server.RequestStart(db))
            {
                Upsert(obs);
            }
        }

        #region Generic CRUD
        private bool Upsert<T>(T obj)
        {
           
            using (server.RequestStart(db))
            {
                var collection = db.GetCollection<T>(OBSERVATION_COLLECTION_NAME);
                var safeMode = new SafeMode(true);
                SafeModeResult result = null;
                try
                {
                    result = collection.Save(obj, safeMode);
                    return result.Ok;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        

        #endregion


    }


    public class IsKeyConvention : IIdMemberConvention
    {
        public string FindIdMember(Type type)
        {

            foreach (var property in type.GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(KeyAttribute), true))
                {
                    return property.Name;
                }
            }
            return null;
        }
    }
}