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
    private static string pathToTemplatesDb = @"D:\TemplateDB\";
    private static string pathToRules = @"..\..\Rules.txt";

    static void Main(string[] args)
    {
      //DbHelper.GetQualityDb();
      //DbHelper.SaveBestQualityFingerprint();

        var a = Test.FuzzySameTest(1, 0.005, 0.51, 0.55);
        File.AppendAllText(Constants.resultsPath + "time.txt", "\n FuzzySameTest(1, 0.005, 0.51, 0.55) " + a.ToString());
        var b = Test.FuzzyDifTest(1, 0.005, 0.51, 0.55);
        File.AppendAllText(Constants.resultsPath + "time.txt", "\n FuzzyDifTest(1, 0.005, 0.51, 0.55);  " + b.ToString());

        //var c = Test.MonomodalSameTest(1, 0.005, 0.51, 0.55);
        //File.AppendAllText(Constants.resultsPath + "time.txt", "\n MonomodalSameTest(1, 0.005, 0.51, 0.55);  " + c.ToString());
        //var d = Test.MonomodalDifTest(1, 0.005, 0.51, 0.55);
        //File.AppendAllText(Constants.resultsPath + "time.txt", "\n MonomodalDifTest(1, 0.005, 0.51, 0.55); " + d.ToString());

        //var e = Test.MultymodalAllSameTest(1, 0.005, 0.51, 0.55);
        //File.AppendAllText(Constants.resultsPath + "time.txt", "\n MultymodalAllSameTest(1, 0.005, 0.51, 0.55); " + e.ToString());
        //var f = Test.MultymodalAllDifTest(1, 0.005, 0.51, 0.55);
        //File.AppendAllText(Constants.resultsPath + "time.txt", "\n MultymodalAllDifTest(1, 0.005, 0.51, 0.55); " + f.ToString());

        //var j = Test.MultymodalVotSameTest(1, 0.005, 0.51, 0.55);
        //File.AppendAllText(Constants.resultsPath + "time.txt", "\n MultymodalVotSameTest(1, 0.005, 0.51, 0.55); " + j.ToString());
        //var h = Test.MultymodalVotDifTest(1, 0.005, 0.51, 0.55);
        //File.AppendAllText(Constants.resultsPath + "time.txt", "\n MultymodalVotDifTest(1, 0.005, 0.51, 0.55); " + h.ToString());

      int t = 3;
    }

    private static void OneCycleTest()
    {
      int i = 5;
      int a = DbHelper.GetBestFingerprint(i);
      var b = DbHelper.GetTestingFingerprintSame(i, a);
      OneFullCycle(b[0].Item1, b[0].Item2, b[0].Item3, i.ToString() + "_" + a.ToString()+".tif");
    }

    private static void DBTest()
    {
      int a = DbHelper.GetBestFingerprint(5);
      var b = DbHelper.GetTestingFingerprintSame(100, DbHelper.GetBestFingerprint(100));
      var c = DbHelper.GetTestingFingerprintDiff(100);
      //DbHelper.FindBestQualityFingerprint();
    }

    private static void OneFingerTest(string fileName1, string fileName2)
    {
      var img1 = ImageHelper.LoadImage(pathToDb + fileName1);
      var img2 = ImageHelper.LoadImage(pathToDb + fileName2);
      double identity1 = Matcher.GetIdentity(pathToDb + fileName2, pathToDb + fileName1);

      var map = QualityHelper.GetQualityMap(pathToDb + fileName1);
      double awerageQuality = QualityHelper.GetAverageQualityNfiq(map);
      double badBlocks = QualityHelper.GetLowQualityBlocksNfiq(map);
      double darkness = QualityHelper.GetDarkness(img1);
      double background = QualityHelper.GetBackgroundPercentage(img1);
      InputVector input = new InputVector();
      input.AverageQuality = awerageQuality;
      input.Background = background;
      input.BadBlocks = badBlocks;
      input.Darkness = darkness;
      input.Identity = identity1;

      DecisionMaker m = new DecisionMaker();
      m.GetAnswerForFinger(input,0.5);

    }

    private static void OneFullCycle(string fileName1, string fileName2, string fileName3, string fileNameEt)
    {
      //var fuzzyInput =  InputHelper.GetMultyFuzzyInput(fileName1, fileName2, fileName3, fileNameEt);

      //DecisionMaker m = new DecisionMaker();
      //double threshold = 0.5;
      //var answer = m.GetAnswer(fuzzyInput, threshold);
      //string a = answer.ValueName;
      //double i = answer.Membership;
     

    }

    private static void SaveTemplateDB()
    {
      for (int i = 1  ; i < 101; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          string fileName = i + "_" + j + ".tif";
          double[,] img = ImageHelper.LoadImage(pathToDb + fileName);
          Matcher.SaveTemplate(img, pathToTemplatesDb + fileName);

        }
        
      }
 
    }

    private static void GetFingerprint(string path1, string path2 )
    {
      //take 
      double[,] image1 = ImageHelper.LoadImage(path1);
      double[,] image2 = ImageHelper.LoadImage(path2);
      double a = Matcher.GetIdentity(path1, path2);
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
