using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

using Курсач_СОВ_Изолирующий_Лес.IsolationForest;

using weka.filters.supervised.attribute;
using weka.core;
using weka.filters;
using weka.classifiers.trees;
using weka.core.converters;

namespace Курсач_СОВ_Изолирующий_Лес
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Packet> packets = new List<Packet>();
        string[][] dataString;
        //double[][] dataDouble;
        List<List<Double>> dataDouble = new List<List<double>>();

        bool[] trueClass;
        double[] ocenkaAnomalii;

        List<List<Double>> dataDoubleTrain = new List<List<double>>();
        List<List<Double>> dataDoubleTest = new List<List<double>>();
        Instances dataInstacesTrain;
        Instances dataInstacesTest;

        struct InfaDlyaObnaryjeniyaAnomaliiARFF
        {
            public List<List<Double>> dataTrain;
            public List<List<Double>> dataTest;

            public double porogOcenki;
        }

        struct InfaDlyaExperimentaCIzmenenPoroga
        {
            public double min;
            public double max;
            public double shag;
        }

        struct InfaDlyaZapolnenieTablIzARFF
        {
            public Instances dataInstaces;
            public DataGridView dataGridView;
            public bool dataTrain;
        }

        struct InfaDlyaZapolnenieTablIzCSV
        {
            public string[] linesIzCSV;
            public double[][] data;
            public DataGridView dataGridView;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView9.Rows.Add(3);

            dataGridView9.Rows[0].Cells[0].Value = "Мин";
            dataGridView9.Rows[1].Cells[0].Value = "Макс";
            dataGridView9.Rows[2].Cells[0].Value = "Шаг";

        }

        public void ZapolnenieTabl(object a)
        {
            InfaDlyaZapolnenieTablIzARFF infa = (InfaDlyaZapolnenieTablIzARFF)a;
            Instances dataSet = infa.dataInstaces;
            DataGridView dataGridViewFormat = infa.dataGridView;
            //DataGridView dataGridViewFormat = new DataGridView();

            int countAttribute = dataSet.numAttributes();
            int countRow = dataSet.numInstances();

            if (infa.dataTrain)
            {
                dataDoubleTrain.Clear();
            }
            else
            {
                dataDoubleTest.Clear();
            }

            dataGridViewFormat.Invoke((Action)(() =>
            {
                dataGridViewFormat.Columns.Clear();
            }));
            //Добавление столбцов = атрибутам
            /*
            for (int i = 0; i < countAttribute; i++)
            {
                dataGridViewFormat.Invoke((Action)(() =>
                {
                    dataGridViewFormat.Columns.Add("column1_" + i, dataSet.attribute(i).name());
                }));
            }
            */
            dataGridViewFormat.Invoke((Action)(() =>
            {
                dataGridViewFormat.Columns.Add("column1_class", "class");
            }));

            for (int k = 0; k < countRow; k++)
            {
                List<double> rowDouble = new List<double>();

                String[] row = new String[1];

                for (int i = 0; i < countAttribute; i++)
                {
                    if (i < countAttribute - 1)  //Ограничение: чтобы не добавлялась метка класса как атрибут
                    {
                        rowDouble.Add(dataSet.get(k).value(i));
                    }
                    else
                    {
                        row[0] = dataSet.get(k).value(i).ToString();
                    }

                    //row[i] = dataSet.get(k).value(i).ToString();
                    //dataGridViewFormat.Rows[k].Cells[i].Value = rowDouble[i];
                }

                //Добавление строки в таблицу
                dataGridViewFormat.Invoke((Action)(() =>
                {
                    dataGridViewFormat.Rows.Add(row);
                    dataGridViewFormat.Rows[k].HeaderCell.Value = (k + 1).ToString();
                }));

                if (infa.dataTrain)
                {
                    dataDoubleTrain.Add(rowDouble);
                }
                else
                {
                    dataDoubleTest.Add(rowDouble);
                }

                label1.Invoke((Action)(() => { label1.Text = "Загружено данных:" + k + "/" + countRow; }));
                progressBar1.Invoke((Action)(() => { progressBar1.Value = k; }));
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //Загрузка из arff
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;

                dataInstacesTrain = new Instances(new java.io.FileReader(textBox3.Text));

                progressBar1.Maximum = dataInstacesTrain.numInstances();
                //Очистить столбцы и вывести данные в таблицу
                InfaDlyaZapolnenieTablIzARFF infa = new InfaDlyaZapolnenieTablIzARFF();
                infa.dataInstaces = dataInstacesTrain;
                infa.dataGridView = dataGridView7;
                infa.dataTrain = true;

                Task.Factory.StartNew(() => { ZapolnenieTabl(infa); }); //Создание и запуск нового потока
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //Загрузка из arff
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;

                dataInstacesTest = new Instances(new java.io.FileReader(textBox4.Text));

                progressBar1.Maximum = dataInstacesTest.numInstances();
                //Очистить столбцы и вывести данные в таблицу
                InfaDlyaZapolnenieTablIzARFF infa = new InfaDlyaZapolnenieTablIzARFF();
                infa.dataInstaces = dataInstacesTest;
                infa.dataGridView = dataGridView6;
                infa.dataTrain = false;

                Task.Factory.StartNew(() => { ZapolnenieTabl(infa); }); //Создание и запуск нового потока
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = dataDoubleTest.Count;

            InfaDlyaObnaryjeniyaAnomaliiARFF infa = new InfaDlyaObnaryjeniyaAnomaliiARFF();
            infa.dataTrain = dataDoubleTrain;
            infa.dataTest = dataDoubleTest;
            infa.porogOcenki = 90;
            

            Task.Factory.StartNew(() => { obnaryjenieAnomaliiARFF(infa); }); //Создание и запуск нового потока
        }

        public void obnaryjenieAnomaliiARFF(object a)
        {
            InfaDlyaObnaryjeniyaAnomaliiARFF infa = (InfaDlyaObnaryjeniyaAnomaliiARFF)a;
            List<List<Double>> dataTrain = infa.dataTrain; //обучающие данные
            List<List<Double>> dataTest = infa.dataTest; //обучающие данные

            DateTime dateTime;  //время

            /*
			Calling isolationForestAlgorithm with input data, number of trees which is roughly taken to be 
			20% of number of data points and sub sampling size which is taken as 10% of number of data points.
            (входные (обучающие) данные, количество деревьев, размер подвыборки для ДР)
			*/
            isolationForestAlgorithm isolationForestAlgorithm = new isolationForestAlgorithm();

            label1.Invoke((Action)(() => { label1.Text = "Обучение леса..."; }));

            dateTime = DateTime.Now;

            //Можно поиграть с параметрами
            List<Tree> forest = isolationForestAlgorithm.buildForest(dataTrain, 100, Convert.ToInt32(dataTrain.Count * 0.66));  //(data, data.size() / 5, data.size() / 10); //оригинал

            textBoxLog.Invoke((Action)(() =>
            {
                textBoxLog.Text += "Количество элементов на обучение" + dataTrain.Count;
                textBoxLog.Text += "Время обучения" + (DateTime.Now - dateTime);
            })); //Вывод времени обучения

            /*
			Calculating anomalyScore for each data point in input data. This is calculated for each data point
			by calculating the average of the path length for each point.
			*/
            anomalyScore anomalyScore = new anomalyScore();

            dataGridView6.Invoke((Action)(() =>
            {
                dataGridView6.Columns.Add("columns6_Score", "Оценка аномалии");
                //dataGridView6.Columns.Add("columns6_flagAnomalii", "Флаг аномалии");
            })); //Добавление строки в таблицу

            int countDataTest = dataTest.Count;

            ocenkaAnomalii = new double[countDataTest];

            dateTime = DateTime.Now;

            for (int i = 0; i < countDataTest; i++)
            {
                ocenkaAnomalii[i] = anomalyScore.Calculating(forest, dataTest[i], countDataTest); //Оценка аномалии

                dataGridView6.Invoke((Action)(() =>
                {
                    dataGridView6.Rows[i].Cells[dataGridView6.Columns.Count - 1].Value = ocenkaAnomalii[i];
                })); //Добавление строки в таблицу

                label1.Invoke((Action)(() => { label1.Text = "Обработано данных:" + i + "/" + countDataTest; }));
                progressBar1.Invoke((Action)(() => { progressBar1.Value = i; }));
            }

            textBoxLog.Invoke((Action)(() =>
            {
                textBoxLog.Text += "Количесвто элементов на тестирование" + countDataTest;
                textBoxLog.Text += "Время тестирования" + (DateTime.Now - dateTime);
            })); //Вывод времени тестирования
        }

        private void button14_Click(object sender, EventArgs e)
        {

            InfaDlyaExperimentaCIzmenenPoroga infa = new InfaDlyaExperimentaCIzmenenPoroga();
            infa.min = Convert.ToDouble(dataGridView9.Rows[0].Cells[1].Value);
            infa.max = Convert.ToDouble(dataGridView9.Rows[1].Cells[1].Value);
            infa.shag = Convert.ToDouble(dataGridView9.Rows[2].Cells[1].Value);

            progressBar1.Value = 0;
            progressBar1.Maximum = Convert.ToInt32((infa.max - infa.min) / infa.shag) + 2;

            Task.Factory.StartNew(() => { ExperimentCIzmenenPoroga(infa); }); //Создание и запуск нового потока
        }

        public void ExperimentCIzmenenPoroga(object a)
        {
            InfaDlyaExperimentaCIzmenenPoroga infa = (InfaDlyaExperimentaCIzmenenPoroga)a;
            int countRow = ocenkaAnomalii.Length;

            for (double i = infa.min; i < infa.max; i += infa.shag)
            {
                int TP = 0;
                int TN = 0;
                int FP = 0;
                int FN = 0;

                bool[] predictedClass = new bool[countRow];

                //Определение класса на основе порога
                for (int k = 0; k < countRow; k++)
                {
                    if (ocenkaAnomalii[k] > i)
                    {
                        predictedClass[k] = true;
                    }
                    else
                    {
                        predictedClass[k] = false;
                    }
                }

                for (int k = 0; k < countRow; k++)
                {
                    if ((trueClass[k] == true) && (trueClass[k] == predictedClass[k]))
                    {
                        TP++;
                    }

                    if ((trueClass[k] == false) && (trueClass[k] == predictedClass[k]))
                    {
                        TN++;
                    }

                    if ((trueClass[k] == true) && (trueClass[k] != predictedClass[k]))
                    {
                        FP++;
                    }

                    if ((trueClass[k] == false) && (trueClass[k] != predictedClass[k]))
                    {
                        FN++;
                    }
                }

                string[] row = calculatingMetriks(i, TP, TN, FP, FN);

                dataGridView10.Invoke((Action)(() =>
                {
                    dataGridView10.Rows.Add(row);
                }));

                int obmen = TP;
                TP = TN;
                TN = obmen;

                obmen = FP;
                FP = FN;
                FN = obmen;

                row = calculatingMetriks(i, TP, TN, FP, FN);

                dataGridView11.Invoke((Action)(() =>
                {
                    dataGridView11.Rows.Add(row);
                }));

                label1.Invoke((Action)(() => { label1.Text = "Порог:" + i + "/" + infa.max; }));
                progressBar1.Invoke((Action)(() => { progressBar1.Value++; }));

            }
        }

        private string[] calculatingMetriks(double i, int TP, int TN, int FP, int FN)
        {
            double precision = (TP / (double)(TP + FP));
            double recall = (TP / (double)(TP + FN));
            double accuracy = ((TP + TN) / (double)(TP + TN + FP + FN));
            double F_mera = 2 * ((precision * recall) / (precision + recall));

            double TPR = recall;
            double FPR = (FP / (double)(FP + TN));

            string[] row = new string[7];
            row[0] = i.ToString();
            row[1] = precision.ToString();
            row[2] = recall.ToString();
            row[3] = F_mera.ToString();
            row[4] = accuracy.ToString();
            row[5] = TPR.ToString();
            row[6] = FPR.ToString();

            return row;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = openFileDialog1.FileName;

                InfaDlyaZapolnenieTablIzCSV infa = new InfaDlyaZapolnenieTablIzCSV();
                infa.dataGridView = dataGridView8;
                infa.linesIzCSV = File.ReadAllLines(openFileDialog1.FileName);

                progressBar1.Maximum = infa.linesIzCSV.Length;

                Task.Factory.StartNew(() => { ZapolnenieTablIzCSV(infa); }); //Создание и запуск нового потока
            }
        }

        public void ZapolnenieTablIzCSV(object a)
        {
            InfaDlyaZapolnenieTablIzCSV infa = (InfaDlyaZapolnenieTablIzCSV)a;
            DataGridView dataGridViewFormat = infa.dataGridView;
            string[] linesCSV = infa.linesIzCSV;

            int countRow = linesCSV.Length;

            trueClass = new bool[countRow];
            ocenkaAnomalii = new double[countRow];

            for (int k = 0; k < countRow; k++)
            {
                string[] row = linesCSV[k].Split(';');

                if (row[0] == "1")
                {
                    trueClass[k] = true;
                }
                else
                {
                    trueClass[k] = false;
                }

                ocenkaAnomalii[k] = Convert.ToDouble(row[1]);

                //Добавление строки в таблицу
                dataGridViewFormat.Invoke((Action)(() =>
                {
                    dataGridViewFormat.Rows.Add(row);
                }));

                label1.Invoke((Action)(() => { label1.Text = "Загружено данных:" + k + "/" + countRow; }));
                progressBar1.Invoke((Action)(() => { progressBar1.Value = k; }));
            }

            dataGridView9.Invoke((Action)(() =>
            {
                dataGridView9.Rows[0].Cells[1].Value = ocenkaAnomalii.Min();
                dataGridView9.Rows[1].Cells[1].Value = ocenkaAnomalii.Max();
                dataGridView9.Rows[2].Cells[1].Value = (ocenkaAnomalii.Max() - ocenkaAnomalii.Min()) / 1000;
            }));
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }
    }
}

