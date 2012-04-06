using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DecisionTree
{
    public class TrainedTreeNode : TreeNode
    {
        public TrainedTreeNode(TreeNode parent, List<TreeNode> children, double val) : base(parent, children, val) { }
    }
}