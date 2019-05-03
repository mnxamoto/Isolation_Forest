using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач_СОВ_Изолирующий_Лес
{
    class Tree
    {
        private Tree left;
        private Tree right;
        private int attribute;
        private double splitVal;
        private int size;

        public Tree(int attribute, double splitVal)
        {
            this.attribute = attribute;
            this.splitVal = splitVal;
        }

        public Tree getLeft()
        {
            return left;
        }

        public void setLeft(Tree left)
        {
            this.left = left;
        }

        public int getAttribute()
        {
            return attribute;
        }

        public void setAttribute(int attribute)
        {
            this.attribute = attribute;
        }

        public double getSplitVal()
        {
            return splitVal;
        }

        public void setSplitVal(double splitVal)
        {
            this.splitVal = splitVal;
        }

        public int getSize()
        {
            return size;
        }

        public void setSize(int size)
        {
            this.size = size;
        }

        public Tree getRight()
        {
            return right;
        }

        public void setRight(Tree right)
        {
            this.right = right;
        }
    }
}
