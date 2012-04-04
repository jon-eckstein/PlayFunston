using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTree
{
    public class SimpleDecisionTree
    {
        private TreeNode rootNode;
        private TreeNode currentNode;

        public SimpleDecisionTree(TreeNode root)
        {
            rootNode = root;
            currentNode = rootNode;
        }

        public SimpleDecisionTree() : this(new TreeNode(null,null,double.NaN))
        {            
        }

        public bool AddBranch(double[] newVals)
        {
            currentNode = rootNode;  
            foreach (var val in newVals)
            {
                var childCount = currentNode.Children.Count;
                if (childCount == 0)
                {
                    var newChild = new TreeNode(currentNode, null, val);
                    currentNode.Children.Add(newChild);
                    currentNode = newChild;
                }
                else
                {
                    //if there exist a child with the same value then just move to the next value..
                    var sameValNode = currentNode.Children.FirstOrDefault(n => n.Value == val);
                    if (sameValNode != null)
                    {
                        currentNode = sameValNode;
                    }
                    else
                    {
                        var newChild = new TreeNode(currentNode, null, val);
                        currentNode.Children.Add(newChild);
                        currentNode = newChild;
                    }                                                         
                }               
                
            }

            return true;
        }

        public double Compute(double[] inputVals)
        {
            currentNode = rootNode;
            
            foreach (var val in inputVals)
            {
                //if they're equal then use this child...
                var sameValNode = currentNode.Children.FirstOrDefault(n => n.Value == val);                
                if (sameValNode != null)               
                    currentNode = sameValNode;                
                else
                {
                    //if not get the child with the closest value...
                    var closestNode = (from node in currentNode.Children
                                       select new { Node = node, Diff = Math.Abs(node.Value - val) }).OrderBy(a => a.Diff).First();

                    currentNode = closestNode.Node;
                }
            }

            //return the child of the last node...there should be exactly one child...
            if (currentNode.Children.Count != 1)
                throw new Exception("This tree ain't right.");
            else
                return currentNode.Children.First().Value;

        }
    }
}
