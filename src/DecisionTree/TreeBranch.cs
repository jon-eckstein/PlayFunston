using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTree
{
    public class TreeBranch<T> where T : IComparable<T>
    {
        private T[] values;
        private T answer;
        private bool isObserved;

        public TreeBranch(T[] vals, T answer, bool isObserved)
        {
            this.values = vals;
            this.answer = answer;
            this.isObserved = isObserved;
        }

    }
}
