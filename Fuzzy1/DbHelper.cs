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
      for (int i = 1; i < 101; i++)
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

    public static void FindBestQualityFingerprint()
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
          var strs = qualityDb[(i - 1) * 8 + j-1].Split();
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
  }
}
