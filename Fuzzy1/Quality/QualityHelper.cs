using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfiq;
using CUDAFingerprinting.Common;
using System.Drawing;
using System.IO;

namespace Fuzzy1
{
  internal class QualityHelper
  {
    List<string[]> quality;

    public QualityHelper()
    {
      quality = new List<string[]>();
      if (File.Exists(Constants.qualityDb + "qualityTest1.csv"))
      {
          var qualityDb = File.ReadAllLines(Constants.qualityDb + "qualityTest1.csv");
          for (int i = 1; i < 101; i++)
          {
              for (int j = 1; j < 9; j++)
              {
                  string[] strs = qualityDb[(i - 1) * 8 + j - 1].Split();
                  // double quality = Double.Parse(strs[3]);
                  quality.Add(strs);
              }
          }
      }
      
    }

    public double GetLowQualityBlocksNfiqS(string fileName1)
    {
        if (quality.Count > 0)
        {
            var a = fileName1.Split('_');

            int i = Int32.Parse(a[0]);
            int j = Int32.Parse((a[1].Split('.'))[0]);
            return 100 * Double.Parse(quality.Find(x => Int32.Parse(x[0]) == i && Int32.Parse(x[1]) == j)[2]);
        }
        else
        {
           return GetLowQualityBlocksNfiq(GetQualityMap(Constants.PathToDb + fileName1));
        }
  
    }

    public double GetAverageQualityNfiqS(string fileName1)
    {
        if (quality.Count > 0)
        {
            var a = fileName1.Split('_');
            int i = Int32.Parse(a[0]);
            int j = Int32.Parse(a[1].Split('.')[0]);
            return Double.Parse(quality.Find(x => Int32.Parse(x[0]) == i && Int32.Parse(x[1]) == j)[3]);
        }
        else
        {
            return GetAverageQualityNfiq(GetQualityMap(Constants.PathToDb + fileName1));
        }
    }

    public double GetDarknessS(string fileName1)
    {
        if (quality.Count > 0)
        {
            var a = fileName1.Split('_');
            int i = Int32.Parse(a[0]);
            int j = Int32.Parse(a[1].Split('.')[0]);

            return 100 * Double.Parse(quality.Find(x => Int32.Parse(x[0]) == i && Int32.Parse(x[1]) == j)[4]);
        }
        else
        {
            return GetDarkness(Constants.PathToDb + fileName1);
        }
    }

    public double GetBackgroundS(string fileName1)
    {
        if (quality.Count > 0)
        {
            var a = fileName1.Split('_');
            int i = Int32.Parse(a[0]);
            int j = Int32.Parse(a[1].Split('.')[0]);
            return 100 * Double.Parse(quality.Find(x => Int32.Parse(x[0]) == i && Int32.Parse(x[1]) == j)[5]);
        }
        else
        {
            return GetBackgroundPercentage(ImageHelper.LoadImage(Constants.PathToDb + fileName1));
        }
    }
      
    public static double GetLowQualityBlocksNfiq(int[,] qualityMap)
    {
      double result = 0;
      qualityMap.Select2D(x => result += (x < 2) ? 1 : 0);

      result /= (qualityMap.GetLength(0) * qualityMap.GetLength(1));
      return result;
    }

    

    public static double GetAverageQualityNfiq(int[,] qualityMap)
    {
      // double[,] a = qualityMap.Select2D(x => (double)x);
      // ImageHelper.SaveArrayAndOpen(a,Constants.qualityDb + "q2.bmp");
      double result = 0;
      for (int i = 0; i < qualityMap.GetLength(0); i++)
      {
        for (int j = 0; j < qualityMap.GetLength(1); j++)
        {
          result += qualityMap[i, j];
        }
      }

      //qualityMap.Select2D((x,y,val) => ( result+= val ));
      result /= (qualityMap.GetLength(0) * qualityMap.GetLength(1));
      return result;
    }

    public static int[,] GetQualityMap(string path)
    {
      return Nfiq.Nfiq.GetQualityMap(new Bitmap(path), String.Empty);
    }

    public static double GetDarkness(string path)
    {
      return GetDarkness(ImageHelper.LoadImage(path));
    }

    /// <summary>
    /// Return double from 0 to 100.
    /// </summary>    
    public static double GetDarkness(double[,] img)
    {
      double numOfBlackPixels = 0;
      img.Select2D(x => numOfBlackPixels += (x < 100) ? 1 : 0);
      return (numOfBlackPixels / (img.GetLength(0) * img.GetLength(1))) * 100;
    }

    public static double GetBackgroundPercentage(double[,] img)
    {
      return BackgroundQalifier.GetBackgroundRercentage(img, Constants.SegmentationWindowSize, Constants.SegmentationWeight);
    }
  }
}
