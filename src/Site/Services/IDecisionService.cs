using System;
namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public interface IDecisionService
    {
        double GetDecision(ShouldITakeMyDogToFortFunstonNow.Models.CurrentObservation obs);
    }
}
