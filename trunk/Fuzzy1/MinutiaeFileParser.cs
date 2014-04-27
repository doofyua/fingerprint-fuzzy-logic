using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioLab.Biometrics.Mcc.Sdk;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Fuzzy1
{
  internal static class MinutiaeFileParser
  {
    internal static  List<Minutia> ParseFile(string path)
    {
      List<String> stringsFromFile = File.ReadAllLines(path).ToList();
      return stringsFromFile.Select(x => ParseOneString(x)).ToList();
    }

    private static Minutia ParseOneString(string str)
    {
      Minutia newMinutia = new Minutia();
      int x;
      int y;
      double angle;
      Int32.TryParse(str.Split()[0], out x);
      Int32.TryParse(str.Split()[1], out y);
      Double.TryParse(str.Split()[0], out angle);

      newMinutia.X = x;
      newMinutia.Y = y;
      newMinutia.Direction = angle;

      return newMinutia;
    }
  }
}
