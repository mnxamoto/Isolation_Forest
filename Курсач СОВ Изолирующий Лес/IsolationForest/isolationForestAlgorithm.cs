using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Курсач_СОВ_Изолирующий_Лес;

namespace Курсач_СОВ_Изолирующий_Лес.IsolationForest
{
    class isolationForestAlgorithm
    {
        public List<Tree> buildForest(List<List<Double>> X, int t, int subSampleSize)
        {

            List<Tree> forest = new List<Tree>();
            int heightLimit = (int)Math.Ceiling(Math.Log(subSampleSize) / Math.Log(2));

            List<List<Double>> Xsub = new List<List<Double>>();
            for (int i = 1; i < t; i++)
            {
                Random rand = new Random();
                int randVal;
                for (int k = 0; k < subSampleSize; k++)
                {
                    randVal = rand.Next(X.Count);
                    Xsub.Add(X[randVal]);
                }

                forest.Add(iTree(Xsub, 0, heightLimit));
            }
            return forest;
        }
        /*
         * Function takes input - random sub sample input (X), current tree height (e), and height limit (l)
         * Returns a tree for X. The external node for the tree holds the size of remaining input.
         * */
        public Tree iTree(List<List<Double>> X, int e, int l)
        {

            if (e > l || X.Count <= 1)
            {
                Tree exNode = new Tree(0, 0);
                exNode.setLeft(null);
                exNode.setRight(null);
                exNode.setSize(X.Count);
                return exNode;
            }
            else
            {
                Random rand = new Random();
                int q = rand.Next(X[0].Count);
                List<Double> columnData = new List<Double>();
                foreach (List<Double> s in X)
                {
                    columnData.Add(s[q]);
                }
                double minVal = columnData.Min();
                double maxVal = columnData.Max();

                Random random = new Random();
                double p = (random.NextDouble() * ((maxVal - minVal) + 1)) + minVal;

                List<List<Double>> X_left = new List<List<Double>>();
                List<List<Double>> X_right = new List<List<Double>>();

                foreach (List<Double> row in X)
                {
                    if (row[q] < p)
                    {
                        X_left.Add(row);
                    }
                    else
                    {
                        X_right.Add(row);
                    }
                }
                Tree inNode = new Tree(q, p);
                inNode.setLeft(iTree(X_left, e + 1, l));
                inNode.setRight(iTree(X_right, e + 1, l));

                return inNode;
            }
        }
    }
}
