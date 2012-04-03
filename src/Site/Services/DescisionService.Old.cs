using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Statistics.Filters;
using System.Data;
using ShouldITakeMyDogToFortFunstonNow.Framework;
using ShouldITakeMyDogToFortFunstonNow.Models;

namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public class DecisionService
    {       
        private MongoDbRepoService repoService;
        private DecisionVariable[] attributes;
        private DecisionTree tree;
        private C45Learning c45Learning;
        private List<CurrentObservation> trainingData = new List<CurrentObservation>();

        public DecisionService(MongoDbRepoService repo)
        {

            repoService = repo;
            InitDecisionTree();                                          
        }

        private void InitDecisionTree()
        {
            attributes = new[] 
            {
                new DecisionVariable("ConditionCode", 3),
                new DecisionVariable("Temp", DecisionAttributeKind.Continuous, new AForge.DoubleRange(0,100)),
                new DecisionVariable("WindChill", DecisionAttributeKind.Continuous, new AForge.DoubleRange(0,100)),
                new DecisionVariable("WindMph", DecisionAttributeKind.Continuous, new AForge.DoubleRange(0,100)),
                new DecisionVariable("WindGustMph", DecisionAttributeKind.Continuous, new AForge.DoubleRange(0,100)),
            };

           
            //var data = repoService.GetAllObservations().ToDataTable();
            var data = GetTrainingData().ToDataTable();
            //insert training data if there is none...
            if (data.Rows.Count == 0)
            {
                InsertTrainingData();
                data = repoService.GetAllObservations().ToDataTable();
            }
            /*
            DataTable data = new DataTable("My Training Data");

            //data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
            data.Columns.AddRange(new[] {
                new DataColumn("Day"),
                new DataColumn("ConditionCode"),
                new DataColumn("Temperature"),
                new DataColumn("WindChill"),
                new DataColumn("Wind"),
                new DataColumn("WindGust"),                
                new DataColumn("GoFunston")
            });

            data.Rows.Add("D1", 0, 55.2, 40, 5, 0, 1);
            data.Rows.Add("D2", 1, 66, 0, 5, 5, 0);
            data.Rows.Add("D3", 2, 50, 32, 22, 25, -1);
            */

            int classCount = 3; // 2 possible output values for going to funston; yes, no maybe

            tree = new DecisionTree(attributes, classCount);

            // Create a new instance of the ID3 algorithm
            c45Learning = new C45Learning(tree);

            // Translate our training data into integer symbols using our codebook:                        
            double[][] inputs = data.ToDoubleArray("ConditionCode", "Temp", "WindChill", "WindMph", "WindGustMph");
            //int[][] inputs = data.ToIntArray("ConditionCode", "Temp", "WindChill", "WindMph", "WindGustMph");
            int[] outputs = data.ToIntArray("GoFunston").GetColumn(0);


            // Learn the training instances!
            c45Learning.Run(inputs, outputs);  

        }


        public int GetDecision(CurrentObservation obs)
        {
            try
            {
                //repoService.AddObservation(obs);
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

        private void InsertTrainingData()
        {           
            foreach (var obs in GetTrainingData())
                repoService.AddObservation(obs);
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
            yield return new CurrentObservation(2, 60, 50, 15, 20, -1);
            //yield return new CurrentObservation(2, 0, 0, 0, 0, -1);
            yield return new CurrentObservation(2, 50, 40, 10, 15, -1);
        }

    }
}