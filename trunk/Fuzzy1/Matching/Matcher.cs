using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioLab.Biometrics.Mcc.Sdk;
using System.IO;

namespace Fuzzy1
{
  internal static class Matcher
  {

    private static object GetTemplate(string fileName, double[,] image)
    {
      //if this template exist alredy
      if (File.Exists(Constants.pathToTemplatesDb + fileName + "_t"))
      {
        return MccSdk.LoadMccTemplateFromTextFile(Constants.pathToTemplatesDb + fileName + "_t");
      }
      else
      {
        var minutiae = MinutiaeExtractor.GetBiolabMinutiae(image);
        object template = MccSdk.CreateMccTemplate(image.GetLength(1), image.GetLength(0), 500, minutiae.ToArray());
        MccSdk.SaveMccTemplateToTextFile(template, Constants.pathToTemplatesDb + fileName + "_t");
        return template;
      }
    }

    public static double GetIdentity(string fileName1, double[,] image1, string fileName2, double[,] image2)
    {

      object template1 = GetTemplate(fileName1,image1);
      object template2 = GetTemplate(fileName2,image2);

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