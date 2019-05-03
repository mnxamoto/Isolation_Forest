using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсач_СОВ_Изолирующий_Лес.IsolationForest
{
    class anomalyScore
    {
        public double Calculating(List<Tree> Forest, List<Double> rowData, int countData)
        {
            CalculatePath path = new CalculatePath();
            double h = 0;
            double E = 0;
            double C = 0;
            double anomalyScore = 0;

            for (int i = 0; i < Forest.Count; i++)
            {
                h += path.PathLength(rowData, Forest[i], 0);
            }

            E = h / Forest.Count;
            C = ((2 * (Math.Log(countData - 1) + 0.5772156649)) - ((double)(2 * (countData - 1)) / countData));
            anomalyScore = Math.Pow(2, -1 * (E / C));
            //anomalyScore = (anomalyScore - 0.35) * 1.8;

            return anomalyScore;
        }
    }
}
