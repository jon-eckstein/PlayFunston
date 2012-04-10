using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DecisionTree;

namespace DecisionTreeTests
{
    [TestClass]
    public class TreeTests
    {
        [TestMethod]
        public void NewNodeShouldBeAdded()
        {
            var head = new TreeNode(null, null, double.NaN);
            var tree = new SimpleDecisionTree(head);
            tree.AddBranch(new double[] { 0, 1 }, 3, false);
            Assert.AreEqual(0, head.Children.First().Value);
            Assert.AreEqual(1, head.Children.First().Children.First().Value);
        }

        [TestMethod]
        public void MultipleNodesWithDiscreteValuesShouldBeAdded()
        {
            var head = new TreeNode(null, null, double.NaN);
            var tree = new SimpleDecisionTree(head);
            tree.AddBranch(new double[] { 0, 1 }, 3, false);
            tree.AddBranch(new double[] { 1, 5 }, 6, false);

            Assert.AreEqual(0, head.Children.First().Value);
            Assert.AreEqual(1, head.Children.First().Children.First().Value);
            Assert.AreEqual(3, head.Children.First().Children.First().Children.First().Value);

            Assert.AreEqual(1, head.Children.ElementAt(1).Value);
            Assert.AreEqual(5, head.Children.ElementAt(1).Children.First().Value);
            Assert.AreEqual(6, head.Children.ElementAt(1).Children.First().Children.First().Value);
        }


        [TestMethod]
        public void NodesWithSameValueShouldBranchOffParent()
        {
            var head = new TreeNode(null, null, double.NaN);
            var tree = new SimpleDecisionTree(head);
            tree.AddBranch(new double[] { 0, 1, 3 }, 1, false);
            tree.AddBranch(new double[] { 1, 5, 6 }, 0, false);
            tree.AddBranch(new double[] { 1, 9, 8 }, -1, false);
            tree.AddBranch(new double[] { 2, 5, 4 }, -1, false);

            Assert.AreEqual(0, head.Children.First().Value);
            Assert.AreEqual(1, head.Children.First().Children.First().Value);
            Assert.AreEqual(3, head.Children.First().Children.First().Children.First().Value);

            Assert.AreEqual(1, head.Children.ElementAt(1).Value);
            Assert.AreEqual(5, head.Children.ElementAt(1).Children.First().Value);
            Assert.AreEqual(6, head.Children.ElementAt(1).Children.First().Children.First().Value);

            Assert.AreEqual(2, head.Children.ElementAt(1).Children.Count);
            Assert.AreEqual(9, head.Children.ElementAt(1).Children.ElementAt(1).Value);
            Assert.AreEqual(8, head.Children.ElementAt(1).Children.ElementAt(1).Children.First().Value);
        }

        [TestMethod]
        public void ComputeShouldGiveCorrectAnswer()
        {
            var head = new TreeNode(null, null, double.NaN);
            var tree = new SimpleDecisionTree(head);
            tree.AddBranch(new double[] { 0, 1, 3 }, 1, false);
            tree.AddBranch(new double[] { 1, 5, 6 }, 0, false);
            tree.AddBranch(new double[] { 1, 9, 8 }, -1, false);
            tree.AddBranch(new double[] { 2, 5, 4 }, -1, false);

            var answer = tree.Compute(new double[] { 0, 1, 2 });
            Assert.AreEqual(1, answer);

            answer = tree.Compute(new double[] { 1, 6, 2 });
            Assert.AreEqual(0, answer);

            answer = tree.Compute(new double[] { 1, 8, 2 });
            Assert.AreEqual(-1, answer);

            answer = tree.Compute(new double[] { 2, 0, 0 });
            Assert.AreEqual(-1, answer);
        }


        [TestMethod]
        public void ComputeShouldFavorAnObservedBranch()
        {
            var head = new TreeNode(null, null, double.NaN);
            var tree = new SimpleDecisionTree(head);
            tree.AddBranch(new double[] { 0, 1, 3 }, 1, false);
            tree.AddBranch(new double[] { 1, 5, 6 }, 0, false);
            tree.AddBranch(new double[] { 1, 9, 8 }, -1, false);
            tree.AddBranch(new double[] { 1, 9, 5 }, 0, true);
            tree.AddBranch(new double[] { 2, 5, 4 }, -1, false);

            var answer = tree.Compute(new double[] { 1, 8, 3 });
            Assert.AreEqual(0, answer);
        }

        [TestMethod]
        public void TreeShouldRemoveOneAnswerIfMultipleExist()
        {
            var head = new TreeNode(null, null, double.NaN);
            var tree = new SimpleDecisionTree(head);
            tree.AddBranch(new double[] { 0, 1, 3 }, 1, false);
            tree.AddBranch(new double[] { 1, 5, 6 }, 0, false);
            tree.AddBranch(new double[] { 1, 8, 5 }, -1, false);
            tree.AddBranch(new double[] { 1, 8, 5 }, 0, true);
            tree.AddBranch(new double[] { 2, 5, 4 }, -1, false);

            var answer = tree.Compute(new double[] { 1, 8, 5 });
            Assert.AreEqual(0, answer);

        }




    }
}
