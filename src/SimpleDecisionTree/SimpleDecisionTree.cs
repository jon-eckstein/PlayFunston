using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDecisionTree
{
    public class SimpleTree
    {
        private TreeNode RootNode { get; set; }
        private TreeNode currentNode;

        public SimpleTree(TreeNode rootNode)
        {
            RootNode = rootNode;
            currentNode = rootNode;
        }

        public SimpleTree() : this(new TreeNode(null,null,double.NaN))
        {            
        }

        public bool AddNode(double[] newVals)
        {
            currentNode = RootNode;  
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
                    //if there exist a child with the same value then just movbe to the next value..
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
            currentNode = RootNode;
            
            foreach (var val in inputVals)
            {
                //if there exist a child with the same value then just movbe to the next value..
                var sameValNode = currentNode.Children.FirstOrDefault(n => n.Value == val);
                //if they're equal then use this child...
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
