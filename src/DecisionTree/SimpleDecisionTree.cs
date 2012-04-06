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

        public bool AddBranch(double[] newVals, bool isObserved)
        {
            currentNode = rootNode;
            foreach (var val in newVals)
            {
                var childCount = currentNode.Children.Count;
                bool addNewNode = false;
                //if there are no current children in the branch then add the current branch as a child.
                if (childCount == 0)
                {
                    addNewNode = true;
                }
                else
                {
                    //if there exist a child with the same value then just move to the next value..
                    var sameValNode = currentNode.Children.FirstOrDefault(n => n.Value == val);
                    if (sameValNode != null)
                        currentNode = sameValNode;
                    else //otherwise add the new branch as a child...                    
                        addNewNode = true;
                }

                //add the node
                if (addNewNode)
                {
                    TreeNode newChild = null;
                    if (isObserved)
                        newChild = new ObservedTreeNode(currentNode, null, val);
                    else
                        newChild = new TrainedTreeNode(currentNode, null, val);

                    currentNode.Children.Add(newChild);
                    currentNode = newChild;
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
                    //order the set by difference first, then whether or not the tree node is observed.
                    var closestNode = (from node in currentNode.Children
                                       select new { Node = node, Diff = Math.Abs(node.Value - val), IsObserved = node is ObservedTreeNode })
                                       .OrderBy(a => a.Diff)
                                       .OrderByDescending(a=>a.IsObserved) 
                                       .First();

                    currentNode = closestNode.Node;
                }
            }

            //return the child of the last node...there should be exactly one child...
            if (currentNode.Children.Count == 1)
                return currentNode.Children.First().Value;                
            else
                throw new Exception("This tree ain't right.");
                
        }
        
    }
}
