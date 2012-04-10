using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DecisionTree;
using ShouldITakeMyDogToFortFunstonNow.Models;

namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public class DecisionService : IDecisionService
    {
        private SimpleDecisionTree tree;
        private IRepoService repoService;
        private static object treeLocker = new object();

        public DecisionService(IRepoService repo)
        {
            repoService = repo;
            InitDecisionTree();
        }

        private void InitDecisionTree()
        {
            lock (treeLocker)
            {
                tree = new SimpleDecisionTree();
                IEnumerable<CurrentObservation> data = null;
                data = repoService.GetAllObservations();

                if (data.Count() == 0) //if no data currently in database, insert training data
                {
                    data = GetTrainingData();
                    foreach (var obs in data)
                        repoService.AddObservation(obs);
                }

                TrainTree(data);
            }
        }


        public double GetDecision(CurrentObservation obs)
        {
            try
            {                
                var input = new double[] { obs.ConditionCode, obs.Temp, obs.WindChill, obs.WindMph, obs.WindGustMph };
                return tree.Compute(input);
            }
            catch (Exception ex)
            {
                //if this situation occurs if something is wrong with the tree.              
                //TODO: come up with an answer somehow...
                return -5;
            }
        }

        public void AddUserObservation(CurrentObservation obs)
        {
            //get rid of any potentially ridiculous entries.
            //there could be extreme cases where it's sunny but the wind is blowing too hard to go, but I'm simplifying things for now.
            //TODO: look into this more carefully.
            if (obs.ConditionCode == 0 && obs.GoFunston == -1) return;
            if (obs.ConditionCode == 2 && obs.GoFunston == 1) return;
            //TODO: need to get rid of any duplicate observations...or maybe figure out a way to add weight to a given observation

            obs.IsObservedByUser = true;
            //repoService.AddObservation(obs);
            //re-initialize the tree...
            //TODO: this should happen offline as a background job.
            InitDecisionTree();
        }

        private void TrainTree(IEnumerable<CurrentObservation> trainingData)
        {            
            foreach (var obs in trainingData)                
                tree.AddBranch(obs.ToDoubleArray(), obs.IsObservedByUser);                            
        }

        private IEnumerable<CurrentObservation> GetTrainingData()
        {
            yield return new CurrentObservation(0, 70, 0, 2, 2, 1);
            yield return new CurrentObservation(0, 60, 0, 5, 5, 1);
            yield return new CurrentObservation(0, 50, 0, 13, 10, 1);
            yield return new CurrentObservation(0, 55, 50, 15, 10, 0);
            yield return new CurrentObservation(0, 45, 0, 20, 30, -1);
            yield return new CurrentObservation(1, 60, 0, 2, 2, 1);
            yield return new CurrentObservation(1, 65, 55, 15, 20, 0);
            yield return new CurrentObservation(1, 50, 0, 10, 5, 0);
            yield return new CurrentObservation(1, 55, 0, 5, 2, 1);
            yield return new CurrentObservation(2, 60, 50, 15, 20, -1);
            yield return new CurrentObservation(2, 0, 0, 0, 0, -1);
            yield return new CurrentObservation(2, 50, 40, 10, 15, -1);
        }

        private void InsertTrainingData()
        {
            foreach (var obs in GetTrainingData())
                repoService.AddObservation(obs);
        }


    }
}