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
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Assets"));
        }
       
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {            
            container.Register<IDecisionService,DecisionService>().AsSingleton();
            container.Register<IRepoService,MongoDbRepoService>().AsSingleton();                       
        }

    }
}