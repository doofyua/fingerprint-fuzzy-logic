using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
  public static class DbHelper
  {
    public static void GetQualityDb()
    {
      for (int i = 41; i < 101; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          int[,] nfiqMask = QualityHelper.GetQualityMap(Constants.PathToDb + i + "_" + j + ".tif");
          double nfiqQuality = QualityHelper.GetAverageQualityNfiq(nfiqMask);
          string str = String.Format("{0} {1} {2}\n", i, j, nfiqQuality);
          File.AppendAllText(Constants.qualityDb + "qualityAll.txt", str);
        }
      }
    }

    public static void SaveBestQualityFingerprint()
    {
      if (!File.Exists(Constants.qualityDb + "qualityAll.txt"))
      {
        GetQualityDb();
      }
      var qualityDb = File.ReadAllLines(Constants.qualityDb + "qualityAll.txt");
      for (int i = 1; i < 101; i++)
      {
        double maxQuality = Double.MinValue;
        int maxFingerNum = 1;
        for (int j = 1; j < 9; j++)
        {
          var strs = qualityDb[(i - 1) * 8 + j - 1].Split();
          double quality = Double.Parse(strs[2]);
          if (quality > maxQuality)
          {
            maxQuality = quality;
            maxFingerNum = j;
          }
        }
        string str = String.Format("{0} {1}\n", i, maxFingerNum);
        File.AppendAllText(Constants.qualityDb + "qualityBest.txt", str);
      }
    }

    public static int GetBestFingerprint(int fingerNum)
    {
      var qualityBest = File.ReadAllLines(Constants.qualityDb + "qualityBest.txt");
      int res = Int32.Parse(qualityBest[fingerNum-1].Split()[1]);
      return res;
    }

    public static List<Tuple<string, string, string>> GetTestingFingerprintSame(int fingerNumber, int bestFingerprint)
    {
      List<Tuple<string, string, string>> res = new List<Tuple<string, string, string>>();
      List<Tuple<int, int, int>> resNums = GetTestingFingerprintNumSame(bestFingerprint);
      foreach (Tuple<int, int, int> item in resNums)
      {
        Tuple<string, string, string> newTuple
          = new Tuple<string, string, string>(
            fingerNumber.ToString() + "_" + item.Item1.ToString()+".tif",
            fingerNumber.ToString() + "_" + item.Item2.ToString() + ".tif",
            fingerNumber.ToString() + "_" + item.Item3.ToString() + ".tif");
        res.Add(newTuple);
      }
      return res;
    }

    public static List<Tuple<int, int, int>> GetTestingFingerprintNumSame(int bestFingerprint)
    {
      List<Tuple<int, int, int>> res = new List<Tuple<int, int, int>>();
      for (int i = 1; i < 8; i++)
      {
        for (int j = i + 1; j < 8; j++)
        {
          for (int k = j + 1; k < 8; k++)
          {
            Tuple<int, int, int> newTouple = new Tuple<int, int, int>(i, j, k);
            if (i == bestFingerprint)
            {
              newTouple = new Tuple<int, int, int>(8, j, k);              
            }
            if (j == bestFingerprint)
            {
              newTouple = new Tuple<int, int, int>(i, 8, k);
            }
            if (k == bestFingerprint)
            {
              newTouple = new Tuple<int, int, int>(i, j, 8);
            }
            res.Add(newTouple);
          }
        }
      }
      return res;
    }

    public static List<Tuple<string, string, string>> GetTestingFingerprintDiff(int fingerNum)
    {
      List<Tuple<string, string, string>> res = new List<Tuple<string, string, string>>();

      Random rand = new Random();
      while (res.Count < 35)
      {
        int i1 = rand.Next(1, 101);
        int j1 = rand.Next(1, 9);

        int i2 = rand.Next(1, 101);
        int j2 = rand.Next(1, 9);

        int i3 = rand.Next(1, 101);
        int j3 = rand.Next(1, 9);

        if (i1 != fingerNum && i2 != fingerNum && i3 != fingerNum)
        {
          Tuple<string, string, string> newTouple =
            new Tuple<string, string, string>(i1.ToString() + "_" + j1.ToString() + ".tif",
              i2.ToString() + "_" + j2.ToString() + ".tif",
              i3.ToString() + "_" + j3.ToString() + ".tif");
          res.Add(newTouple);
        }        
      }
      return res;
    }
  }
}
