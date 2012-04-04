using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTree
{
    public class TreeNode : IComparable<TreeNode>
    {

        public TreeNode Parent { get; set; }
        public SortedSet<TreeNode> Children { get; set; }
        public double Value { get; set; }
        
        public TreeNode(TreeNode parent, List<TreeNode> children, double val)
        {
            Parent = parent;
            if (children != null)
                Children = new SortedSet<TreeNode>(children);
            else
                Children = new SortedSet<TreeNode>();
            Value = val;
        }

        public int CompareTo(TreeNode other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
