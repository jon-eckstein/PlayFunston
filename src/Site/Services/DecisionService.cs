using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleDecisionTree;
using ShouldITakeMyDogToFortFunstonNow.Models;

namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public class DecisionService : IDecisionService
    {
        private SimpleTree tree;
        private IRepoService repoService;
        private static object treeLocker = new object();

        public DecisionService(IRepoService repo)
        {
            repoService = repo;
            InitDecisionTree();
        }

        private void InitDecisionTree()
        {
            tree = new SimpleTree();
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


        public double GetDecision(CurrentObservation obs)
        {
            try
            {                
                var input = new double[] { obs.ConditionCode, obs.Temp, obs.WindChill, obs.WindMph, obs.WindGustMph };
                return tree.Compute(input);
            }
            catch (Exception ex)
            {
                //if this situation occurs then the tree didn't have enough info to process...                
                //TODO: come up with an answer somehow...
                return -5;
            }
        }

        private void TrainTree(IEnumerable<CurrentObservation> trainingData)
        {
            //let the tree learn...
            lock (treeLocker)
            {
                foreach (var obs in trainingData)                
                    tree.AddNode(obs.ToDoubleArray());                
            }
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