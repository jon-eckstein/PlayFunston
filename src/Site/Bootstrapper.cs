using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Conventions;
using TinyIoC;
using ShouldITakeMyDogToFortFunstonNow.Services;

namespace ShouldITakeMyDogToFortFunstonNow
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("scripts"));
        }
       
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            //container.Register<DecisionService>().AsSingleton();
            container.Register<DecisionService>().AsSingleton();
            container.Register<MongoDbRepoService>().AsSingleton();
            
            /*
            var connString = ConfigurationManager.AppSettings["MONGOHQ_URL"];
            var databaseName = connString.Split('/').Last();
            var server = MongoServer.Create(connString);
            var database = server.GetDatabase(databaseName);

            if (!database.CollectionExists("Messages"))
                database.CreateCollection("Messages");

            container.Register<MongoServer>(server);
            container.Register<MongoDatabase>(database);
            container.Register<MongoCollection<Message>>(database.GetCollection<Message>("Messages"));*/
        }

    }
}