using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DecisionTree
{
    public class ObservedTreeNode : TreeNode
    {
        //use the count to give a given observed value some weight.
        public int ObservedCount { get; set; }

        public ObservedTreeNode(TreeNode parent, List<TreeNode> children, double val) : base(parent, children, val) { }
    }
}