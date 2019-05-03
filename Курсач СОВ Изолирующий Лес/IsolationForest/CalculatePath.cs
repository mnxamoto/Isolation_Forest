using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач_СОВ_Изолирующий_Лес
{
    class CalculatePath
    {
        public double PathLength(List<Double> instance, Tree T, int e)
        {
            if (T.getLeft() == null && T.getRight() == null)
            {
                if (T.getSize() > 1)
                {
                    return e + ((2 * (Math.Log(T.getSize() - 1) + 0.5772156649)) - ((2 * (T.getSize() - 1)) / T.getSize()));
                }
                else
                {
                    return e;
                }
            }
            int attr = T.getAttribute();
            double x_attr = instance[attr];
            if (x_attr < T.getSplitVal())
            {
                return PathLength(instance, T.getLeft(), e + 1);
            }
            else
            {
                return PathLength(instance, T.getRight(), e + 1);
            }
        }
    }
}
