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

    public static long FuzzySameTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "FuzzySameTest.txt", strFirst);

      DecisionMaker decisionMaker = new DecisionMaker();

      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numIdk = 0;
        int numAll = 0;
        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);

          foreach (var item in fpForMatch)
          {
            var input = InputHelper.GetMultyFuzzyInput(item.Item1, item.Item2, item.Item3, fileNameEt);

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

    public static long FuzzyDifTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "FuzzyDifTest.txt", strFirst);

      DecisionMaker decisionMaker = new DecisionMaker();

      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numIdk = 0;
        int numAll = 0;
        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintDiff(j);

          foreach (var item in fpForMatch)
          {
            var input = InputHelper.GetMultyFuzzyInput(item.Item1, item.Item2, item.Item3, fileNameEt);

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
        
    public static long MultymodalVotSameTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "MultymodalVotSameTest.txt", strFirst);
      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numAll = 0;
        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);
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

            if (yes>no)
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

    public static long MultymodalVotDifTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdn\n");
      File.AppendAllText(Constants.resultsPath + "MultymodalVotDifTest.txt", strFirst);
      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numAll = 0;
        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";

          var fpForMatch = DbHelper.GetTestingFingerprintDiff(j);
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

    public static long MultymodalAllSameTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "MultymodalAllSameTest.txt", strFirst);
      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numAll = 0;
        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);
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

    public static long MultymodalAllDifTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "MultymodalAllDifTest.txt", strFirst);
      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numAll = 0;
        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintDiff(j);
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

    public static long MonomodalSameTest(int numOfFps, double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "MonomodalSameTest.txt", strFirst);

      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numAll = 0;

        for (int j = 1; j < numOfFps; j++)
        {          
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintSame(j, bestFp);

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
        string str = String.Format("{0} {1} {2} {3} {4}\n", threshold, numAll,numYes,numNo, 0);
        File.AppendAllText(Constants.resultsPath + "MonomodalSameTest.txt", str);

      }
      w.Stop();
      return w.ElapsedMilliseconds;
    }

    public static long MonomodalDifTest(int numOfFps,double thresholdStep)
    {
      numOfFps++;
      Stopwatch w = Stopwatch.StartNew();
      string strFirst = String.Format("Threshold NumFull NumYes NumNo NumIdk\n");
      File.AppendAllText(Constants.resultsPath + "MonomodalSameTest.txt", strFirst);

      for (double threshold = 0.01; threshold < 1; threshold += thresholdStep)
      {
        int numYes = 0;
        int numNo = 0;
        int numAll = 0;

        for (int j = 1; j < numOfFps; j++)
        {
          int bestFp = DbHelper.GetBestFingerprint(j);
          string fileNameEt = j.ToString() + "_" + bestFp.ToString() + ".tif";
          var fpForMatch = DbHelper.GetTestingFingerprintDiff(j);

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
