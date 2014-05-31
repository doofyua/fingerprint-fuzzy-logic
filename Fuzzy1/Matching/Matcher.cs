using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioLab.Biometrics.Mcc.Sdk;
using System.IO;
using CUDAFingerprinting.Common;

namespace Fuzzy1
{
  internal static class Matcher
  {

    private static object GetTemplate(string fileName)
    {
      //if this template exist alredy
      if (File.Exists(Constants.pathToTemplatesDb + fileName + "_t"))
      {
        return MccSdk.LoadMccTemplateFromTextFile(Constants.pathToTemplatesDb + fileName + "_t");
      }
      else
      {
        var image = ImageHelper.LoadImage(Constants.PathToDb + fileName);
        var minutiae = MinutiaeExtractor.GetBiolabMinutiae(image);
        object template = MccSdk.CreateMccTemplate(image.GetLength(1), image.GetLength(0), 500, minutiae.ToArray());
        MccSdk.SaveMccTemplateToTextFile(template, Constants.pathToTemplatesDb + fileName + "_t");
        return template;
      }
    }

    public static double GetIdentity(string fileName1, string fileName2)
    {

      object template1 = GetTemplate(fileName1);
      object template2 = GetTemplate(fileName2);

      MccSdk.SetMccMatchParameters(Constants.pathToMccParams);
      double answer = BioLab.Biometrics.Mcc.Sdk.MccSdk.MatchMccTemplates(template1, template2);
      
      return answer;

    }

    public static void SaveTemplate(double[,] image, string fileName)
    {
      var minutiae = MinutiaeExtractor.GetBiolabMinutiae(image);

      object template = BioLab.Biometrics.Mcc.Sdk.MccSdk.CreateMccTemplate(image.GetLength(1), image.GetLength(0), 500, minutiae.ToArray());
      MccSdk.SaveMccTemplateToTextFile(template,Constants.pathToTemplatesDb + fileName + "_t");
    }
  }
}