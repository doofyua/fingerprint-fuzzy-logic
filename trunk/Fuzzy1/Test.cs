using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
    public static class Test
    {

        public static long FuzzySameTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "FuzzySameTest.txt", strFirst);

            DecisionMaker decisionMaker = new DecisionMaker();
            QualityHelper qhelper = new QualityHelper();

            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);

                var newFpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);
                fpForMatchDictionary.Add(j, newFpForMatch);
            }


            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numIdk = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];

                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";

                    foreach (var item in fpForMatch)
                    {
                        var input = InputHelper.GetMultyFuzzyInput(item.Item1, item.Item2, item.Item3, fileNameEt, qhelper);

                        var answer = decisionMaker.GetAnswer(input, threshold);
                        switch (answer.ValueName)
                        {
                            case "yes":
                                numYes++;
                                break;
                            case "no":
                                numNo++;
                                break;
                            case "idk":
                                numIdk++;
                                break;
                            default:
                                break;
                        }
                        numAll++;
                    }
                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, numIdk);
                File.AppendAllText(Constants.resultsPath + "FuzzySameTest.txt", str);
            }
            w.Stop();
            return w.ElapsedMilliseconds;
        }

        public static long FuzzyDifTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "FuzzyDifTest.txt", strFirst);

            DecisionMaker decisionMaker = new DecisionMaker();
            QualityHelper qhelper = new QualityHelper();
            DbHelper dbHelper = new DbHelper();
            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                var newFpForMatch = dbHelper.GetTestingFingerprintDiff(j);
                fpForMatchDictionary.Add(j, newFpForMatch);

                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);
            }

            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numIdk = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";

                    foreach (var item in fpForMatch)
                    {
                        var input = InputHelper.GetMultyFuzzyInput(item.Item1, item.Item2, item.Item3, fileNameEt, qhelper);

                        var answer = decisionMaker.GetAnswer(input, threshold);
                        switch (answer.ValueName)
                        {
                            case "yes":
                                numYes++;
                                break;
                            case "no":
                                numNo++;
                                break;
                            case "idk":
                                numIdk++;
                                break;
                            default:
                                break;
                        }
                        numAll++;
                    }
                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, numIdk);
                File.AppendAllText(Constants.resultsPath + "FuzzyDifTest.txt", str);
            }
            w.Stop();
            return w.ElapsedMilliseconds;
        }

        public static long MultymodalVotSameTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "MultymodalVotSameTest.txt", strFirst);


            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);

                var newFpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);
                fpForMatchDictionary.Add(j, newFpForMatch);
            }


            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
                    foreach (var item in fpForMatch)
                    {
                        var input = InputHelper.GetMultymodalInput(item.Item1, item.Item2, item.Item3, fileNameEt);
                        int yes = 0;
                        int no = 0;
                        for (int i = 0; i < input.Count; i++)
                        {
                            if (input[i] > threshold)
                                yes++;
                            else
                                no++;
                        }

                        if (yes > no)
                        {
                            numYes++;
                        }
                        else
                        {
                            numNo++;
                        }
                        numAll++;
                    }
                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, 0);
                File.AppendAllText(Constants.resultsPath + "MultymodalVotSameTest.txt", str);
            }
            w.Stop();
            return w.ElapsedMilliseconds;

        }

        public static long MultymodalVotDifTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdn\n");
            File.AppendAllText(Constants.resultsPath + "MultymodalVotDifTest.txt", strFirst);

            DecisionMaker decisionMaker = new DecisionMaker();
            QualityHelper qhelper = new QualityHelper();
            DbHelper dbHelper = new DbHelper();
            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                var newFpForMatch = dbHelper.GetTestingFingerprintDiff(j);
                fpForMatchDictionary.Add(j, newFpForMatch);

                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);
            }

            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numIdk = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";

                    foreach (var item in fpForMatch)
                    {
                        var input = InputHelper.GetMultymodalInput(item.Item1, item.Item2, item.Item3, fileNameEt);
                        int yes = 0;
                        int no = 0;
                        for (int i = 0; i < input.Count; i++)
                        {
                            if (input[i] > threshold)
                                yes++;
                            else
                                no++;
                        }

                        if (yes > no)
                        {
                            numYes++;
                        }
                        else
                        {
                            numNo++;
                        }
                        numAll++;
                    }
                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, 0);
                File.AppendAllText(Constants.resultsPath + "MultymodalVotDifTest.txt", str);
            }
            w.Stop();
            return w.ElapsedMilliseconds;
        }

        public static long MultymodalAllSameTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "MultymodalAllSameTest.txt", strFirst);

            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);

                var newFpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);
                fpForMatchDictionary.Add(j, newFpForMatch);
            }


            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";

                    foreach (var item in fpForMatch)
                    {
                        var input = InputHelper.GetMultymodalInput(item.Item1, item.Item2, item.Item3, fileNameEt);
                        if (input[0] > threshold && input[1] > threshold && input[2] > threshold)
                        {
                            numYes++;
                        }
                        else
                        {
                            numNo++;
                        }
                        numAll++;
                    }
                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, 0);
                File.AppendAllText(Constants.resultsPath + "MultymodalAllSameTest.txt", str);
            }
            w.Stop();
            return w.ElapsedMilliseconds;

        }

        public static long MultymodalAllDifTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "MultymodalAllDifTest.txt", strFirst);

            DecisionMaker decisionMaker = new DecisionMaker();
            QualityHelper qhelper = new QualityHelper();
            DbHelper dbHelper = new DbHelper();
            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                var newFpForMatch = dbHelper.GetTestingFingerprintDiff(j);
                fpForMatchDictionary.Add(j, newFpForMatch);

                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);
            }

            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numIdk = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
                    foreach (var item in fpForMatch)
                    {
                        var input = InputHelper.GetMultymodalInput(item.Item1, item.Item2, item.Item3, fileNameEt);
                        if (input[0] > threshold && input[1] > threshold && input[2] > threshold)
                        {
                            numYes++;
                        }
                        else
                        {
                            numNo++;
                        }
                        numAll++;
                    }
                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, 0);
                File.AppendAllText(Constants.resultsPath + "MultymodalAllDifTest.txt", str);
            }
            w.Stop();
            return w.ElapsedMilliseconds;
        }

        public static long MonomodalSameTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "MonomodalSameTest.txt", strFirst);

            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);

                var newFpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);
                fpForMatchDictionary.Add(j, newFpForMatch);
            }


            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
                    foreach (var item in fpForMatch)
                    {
                        var input1 = InputHelper.GetMonomodalInput(item.Item1, fileNameEt);
                        if (input1 < threshold)
                        {
                            numNo++;
                        }
                        else
                        {
                            numYes++;
                        }
                        numAll++;

                        var input2 = InputHelper.GetMonomodalInput(item.Item2, fileNameEt);
                        if (input2 < threshold)
                        {
                            numNo++;
                        }
                        else
                        {
                            numYes++;
                        }
                        numAll++;

                        var input3 = InputHelper.GetMonomodalInput(item.Item3, fileNameEt);
                        if (input3 < threshold)
                        {
                            numNo++;
                        }
                        else
                        {
                            numYes++;
                        }
                        numAll++;
                    }

                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, 0);
                File.AppendAllText(Constants.resultsPath + "MonomodalSameTest.txt", str);

            }
            w.Stop();
            return w.ElapsedMilliseconds;
        }

        public static long MonomodalDifTest(int step, double thresholdStep, double startThreshold, double endThreshold)
        {

            Stopwatch w = Stopwatch.StartNew();
            string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
            File.AppendAllText(Constants.resultsPath + "MonomodalSameTest.txt", strFirst);

            DecisionMaker decisionMaker = new DecisionMaker();
            QualityHelper qhelper = new QualityHelper();
            DbHelper dbHelper = new DbHelper();
            Dictionary<int, List<Tuple<string, string, string>>> fpForMatchDictionary = new Dictionary<int, List<Tuple<string, string, string>>>();
            Dictionary<int, int> BestFp = new Dictionary<int, int>();

            for (int j = 1; j < 101; j += step)
            {
                var newFpForMatch = dbHelper.GetTestingFingerprintDiff(j);
                fpForMatchDictionary.Add(j, newFpForMatch);

                int bestFp = DbHelper.GetBestFingerprint(j);
                BestFp.Add(j, bestFp);
            }

            for (double threshold = startThreshold; threshold < endThreshold + 0.001; threshold += thresholdStep)
            {
                int numYes = 0;
                int numNo = 0;
                int numIdk = 0;
                int numAll = 0;
                for (int j = 1; j < 101; j += step)
                {
                    int bestFp = BestFp[j];
                    var fpForMatch = fpForMatchDictionary[j];

                    string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
                    foreach (var item in fpForMatch)
                    {
                        var input1 = InputHelper.GetMonomodalInput(item.Item1, fileNameEt);
                        if (input1 < threshold)
                        {
                            numNo++;
                        }
                        else
                        {
                            numYes++;
                        }
                        numAll++;

                        var input2 = InputHelper.GetMonomodalInput(item.Item2, fileNameEt);
                        if (input2 < threshold)
                        {
                            numNo++;
                        }
                        else
                        {
                            numYes++;
                        }
                        numAll++;

                        var input3 = InputHelper.GetMonomodalInput(item.Item3, fileNameEt);
                        if (input3 < threshold)
                        {
                            numNo++;
                        }
                        else
                        {
                            numYes++;
                        }
                        numAll++;
                    }

                }
                string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll, numYes, numNo, 0);
                File.AppendAllText(Constants.resultsPath + "MonomodalDifTest.txt", str);

            }
            w.Stop();
            return w.ElapsedMilliseconds;
        }

    }
}
