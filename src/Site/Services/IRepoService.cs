using System;
namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public interface IRepoService
    {
        void AddObservation(ShouldITakeMyDogToFortFunstonNow.Models.CurrentObservation obs);
        System.Collections.Generic.IEnumerable<ShouldITakeMyDogToFortFunstonNow.Models.CurrentObservation> GetAllObservations();
    }
}
