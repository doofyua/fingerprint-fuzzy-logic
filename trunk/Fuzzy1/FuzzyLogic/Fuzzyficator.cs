using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUDAFingerprinting.Common;

namespace Fuzzy1
{
  internal static class Fuzzyficator
  {
    //identity
    internal static List<LingValue> IdentityFuzzification(double mccAnswer, double threshold)
    {
      LingValue sameValue = new LingValue("identity", "same", SameFunction(mccAnswer, threshold));
      LingValue differentValue = new LingValue("identity", "different", DifferentFunction(mccAnswer, threshold));
      return new List<LingValue>(new[] { sameValue, differentValue });
    }
    private static double SameFunction(double x, double threshold)
    {
      if (x >= threshold + 0.25)
      {
        return 1;
      }
      if (x >= threshold - 0.25 && x < threshold + 0.25)
      {
        return 1 / (1 + Math.Exp(-(20 * x - threshold*20)));
      }
      return 0;
    }
    private static double DifferentFunction(double x, double threshold)
    {
      if (x >= threshold + 0.25)
      {
        return 0;
      }
      if (x >= threshold - 0.25 && x < threshold + 0.25)
      {
        return 1 / (1 + Math.Exp(20 * x - threshold * 20));
      }
      return 1;
    }

    //darkness
    internal static List<LingValue> DarknessFuzzification(double darknessAnswer)
    {
      LingValue lowValue = new LingValue("darkness", "low", LowDarknessFunction(darknessAnswer));
      LingValue highValue = new LingValue("darkness", "high", HidhDarknessFunction(darknessAnswer));
      return new List<LingValue>(new[] { lowValue, highValue });
    }
    private static double LowDarknessFunction(double x)
    {
      if (x < 0.5)
      {
        return 1;
      }
      if (x >= 0.5 && x < 4)
      {
        return -(2.0 / 7) * x + 8.0 / 7;
      }
      return 0;
    }
    private static double HidhDarknessFunction(double x)
    {
      if (x < 0.5)
      {
        return 0;
      } if (x >= 0.5 && x < 4)
      {
        return (2.0 / 7) * x - 1.0 / 7; ;
      }
      return 1;
    }

    //background
    internal static List<LingValue> BackgroundFuzzification(double backgrouundAnswer)
    {
        LingValue largeBackground = new LingValue("background", "large", LargeBacgroundFunction(backgrouundAnswer));
        LingValue normalBackground = new LingValue("background", "normal", NormalBacgroundFunction(backgrouundAnswer));
      return new List<LingValue>(new[] { largeBackground, normalBackground });
    }
    private static double LargeBacgroundFunction(double x)
    {
      if (x > 70)
      {
        return 1;
      }
      if (x > 15 && x <= 50)
      {
        return x * (1.0 / 35) - (3.0 / 7);
      }
      return 0;
    }
    private static double NormalBacgroundFunction(double x)
    {
      if (x < 15)
      {
        return 1;
      }
      if (x >= 15 && x < 50)
      {
        return -(1.0 / 35) * x + (10.0 / 7);
      }
      return 0;
    }


    //LowQualityBlocksNfiq
    internal static List<LingValue> LowQualityBlocksNfiqFuzzification(double nfiqBlocksAnswer)
    {
      LingValue manyBlocks = new LingValue("LowQualityBlocks".ToLower(), "many", ManyLowQualityBlocksFunction(nfiqBlocksAnswer));
      LingValue littleBlocks = new LingValue("LowQualityBlocks".ToLower(), "little", LittleLowQualityBlocksFunction(nfiqBlocksAnswer));
      return new List<LingValue>(new[] { manyBlocks, littleBlocks });
    }
    private static double LittleLowQualityBlocksFunction(double x)
    {
      if (x < 22)
      {
        return 1;
      }
      if (x >= 22 && x < 26)
      {
        return 3*Math.Sqrt(Math.PI*2) * Gaussian.Gaussian1D((x - 22),3);
      }
      return 0;
    }
    private static double ManyLowQualityBlocksFunction(double x)
    {
      if (x < 22)
      {
        return 0;
      }
      if (x >= 22 && x < 25)
      {
          return 3 * Math.Sqrt(Math.PI * 2) * Gaussian.Gaussian1D(-x + 25, 3);
      }
      return 1;
    }


    internal static List<LingValue> AverageQualityNfiqFuzzification(double nfiqQualityAnswer)
    {
      LingValue highQualityNfiq = new LingValue("QualityNfiq".ToLower(), "high", HighQualityNfiqFunction(nfiqQualityAnswer));
      //LingValue middleQualityNfiq = new LingValue("QualityNfiq","Middle",MiddleQualityNfiqFunction(nfiqQualityAnswer));
      LingValue lowQualityNfiq = new LingValue("QualityNfiq".ToLower(), "low", LowQualityNfiqFunction(nfiqQualityAnswer));
      return new List<LingValue>(new[] { highQualityNfiq, lowQualityNfiq });
    }
    //private double MiddleQualityNfiqFunction(double nfiqQualityAnswer)
    //{
    //  throw new NotImplementedException();
    //}
    private static double LowQualityNfiqFunction(double x)
    {
      if (x < 1.3)
      {
        return 1;
      }
      else
      {
        return Math.Pow(3, (-x + 1.3));
      }
    }
    private static double HighQualityNfiqFunction(double x)
    {
      if (x > 2.5)
      {
        return 1;
      }
      else
      {
        return Math.Pow(3, (x - 2.5));
      }
    }

    private static double LowQualytyFunction(double x)
    {
      if (x < 15)
      {
        return 1;
      }
      if (x >= 15 && x < 35)
      {
        return (-0.05 * x + 1.75);
      }
      return 0;
    }
    private static double MiddleQualytyFunction(double x)
    {
      if (x < 15 || x >= 75)
      {
        return 0;
      }
      if (x >= 15 && x < 35)
      {
        return (0.05 * x - 1.75);
      }
      if (x >= 35 && x < 60)
      {
        return 1;
      }
      //x>=60 && x<75
      return -(1 / 15) * x + 5;
    }
    private static double HighQualytyFunction(double x)
    {
      if (x < 50)
      {
        return 1;
      }
      if (x >= 50 && x < 75)
      {
        return (0.04 * x - 2);
      }
      return 1;
    }


  }
}
