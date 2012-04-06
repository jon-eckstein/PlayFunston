using System;
using ShouldITakeMyDogToFortFunstonNow.Models;
namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public interface IDecisionService
    {
        double GetDecision(CurrentObservation obs);
        void AddUserObservation(CurrentObservation obs);
    }
}
