using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioLab.Biometrics.Mcc.Sdk;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using CUDAFingerprinting.Common;

namespace Fuzzy1
{
  class Program
  {
    private static string pathToDb = @"D:\Учеба\Было\fingerprint\Handbook II Ed\FVC2000\Dbs\Db2_a\";
    private static string pathToRules = @"..\..\Rules.txt";
    static void Main(string[] args)
    {
    int t = 3;
    }

    private static void RuleParserTest()
    {
      List<Rule> rules = RuleParser.Parse(pathToRules);
      int i = 1;
    }

    private static void DarknessTest()
    {
      List<string> darkFingerprins = new List<string>(new[] { "57_6", "19_4", "45_6", "86_1", "38_4", "38_2", "4_5", "7_2", "14_2", "19_7" });
      List<string> normalFingerprins = new List<string>(new[] { "1_7", "3_3", "5_3", "6_2", "8_8", "11_1", "21_5", "30_4", "30_8", "32_5" });

      
      for (int threshold = 80; threshold < 120; threshold +=5)
      {
        double darkAverage = 0;
        double normalAverage = 0;
        for (int i = 0; i < darkFingerprins.Count; i++)
        {
          var image = ImageHelper.LoadImage(pathToDb + darkFingerprins[i] + ".tif");
          darkAverage += QualityHelper.GetDarkness(image);
        }
        darkAverage /= darkFingerprins.Count;
        darkAverage *= 100;
        for (int j = 0; j < normalFingerprins.Count; j++)
        {
          var image = ImageHelper.LoadImage(pathToDb + normalFingerprins[j] + ".tif");
          normalAverage += QualityHelper.GetDarkness(image);
        }
        normalAverage /= normalFingerprins.Count;
        normalAverage *= 100;
        string str = String.Format("{0} {1} {2} {3}\n", threshold,normalAverage,darkAverage, darkAverage - normalAverage);
        File.AppendAllText(@"D:\darkness1.txt", str);
      }
    }

    private static void GetAndMatchTemplates()
    {

      //object template =BioLab.Biometrics.Mcc.Sdk.MccSdk.CreateMccTemplate(256, 364, 500, lst.ToArray());
      //double answ = BioLab.Biometrics.Mcc.Sdk.MccSdk.MatchMccTemplates(template, template);

      //object template2 = BioLab.Biometrics.Mcc.Sdk.MccSdk.CreateMccTemplate(256, 364, 500, lst2.ToArray());
      //double answ2 = BioLab.Biometrics.Mcc.Sdk.MccSdk.MatchMccTemplates(template2, template2);

      //double answ3 = BioLab.Biometrics.Mcc.Sdk.MccSdk.MatchMccTemplates(template, template2);

      //object template3 = BioLab.Biometrics.Mcc.Sdk.MccSdk.CreateMccTemplate(256, 364, 500, lst2.ToArray());

      //double answ4 = BioLab.Biometrics.Mcc.Sdk.MccSdk.MatchMccTemplates(template2, template3);



    }

    private static void FullQualityTest()
    {
      Stopwatch w = Stopwatch.StartNew();
      for (int i = 1; i < 101; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          var image = ImageHelper.LoadImage(pathToDb + i + "_" + j + ".tif");
          int[,] nfiqMask =
       QualityHelper.GetQualityMap(pathToDb + i + "_" + j + ".tif");
          double nfiqBadReg = QualityHelper.GetLowQualityBlocksNfiq(nfiqMask);
          double nfiqQuality = QualityHelper.GetAverageQualityNfiq(nfiqMask);
          double darkness = QualityHelper.GetDarkness(image);
          double background = QualityHelper.GetBackgroundPercentage(image);
          string str = String.Format("{0} {1} {2} {3} {4} {5}\n", i, j, nfiqBadReg, nfiqQuality, darkness, background);
          File.AppendAllText(@"D:\qualityTest1.txt", str);

        }
      }
      w.Stop();
      File.AppendAllText(@"D:\qualityTest1.txt", w.ElapsedMilliseconds.ToString());

    }
  }
}
