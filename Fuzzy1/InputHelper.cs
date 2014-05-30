using CUDAFingerprinting.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
  public static class InputHelper
  {
    internal static List<double> GetMultymodalInput(string fileName1, string fileName2, string fileName3, string fileNameEt)
    {   
      List<double> res = new List<double>();
    
      double res1 = GetMonomodalInput(fileNameEt,fileName1);
      double res2 = GetMonomodalInput(fileNameEt,fileName2);
      double res3 = GetMonomodalInput(fileNameEt, fileName3);
      res.Add(res1);
      res.Add(res2);
      res.Add(res3);      
      return res;
    }

    internal static Tuple<InputVector,InputVector,InputVector> GetMultyFuzzyInput(string fileName1, string fileName2, string fileName3, string fileNameEt)
    {      
      var res1 = GetMonoFuzzyInput(fileName1, fileNameEt);
      var res2 = GetMonoFuzzyInput(fileName2, fileNameEt);
      var res3 = GetMonoFuzzyInput(fileName3, fileNameEt);
      Tuple<InputVector, InputVector, InputVector> res =
        new Tuple<InputVector, InputVector, InputVector>(res1, res2, res3);
      return res;
    }

    internal static double GetMonomodalInput(string fileName1, string fileNameEt)
    {
      var img1 = ImageHelper.LoadImage(Constants.PathToDb + fileName1);
      var imgEt = ImageHelper.LoadImage(Constants.PathToDb + fileNameEt);

      double identity = Matcher.GetIdentity(fileNameEt, imgEt, fileName1, img1);
      return identity;
    }

    internal static InputVector GetMonoFuzzyInput(string fileName1, string fileNameEt)
    {
      var img1 = ImageHelper.LoadImage(Constants.PathToDb + fileName1);
      var imgEt = ImageHelper.LoadImage(Constants.PathToDb + fileNameEt);

      double identity1 = Matcher.GetIdentity(fileNameEt, imgEt, fileName1, img1);

      var map = QualityHelper.GetQualityMap(Constants.PathToDb + fileName1);
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
      return input;
    }
  }
}
