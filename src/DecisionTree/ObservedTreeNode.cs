using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DecisionTree
{
    public class ObservedTreeNode : TreeNode
    {
        public ObservedTreeNode(TreeNode parent, List<TreeNode> children, double val) : base(parent, children, val) { }
    }
}