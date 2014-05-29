using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfiq;
using CUDAFingerprinting.Common;
using System.Drawing;

namespace Fuzzy1
{
  internal static class QualityHelper
  {
    public static double GetLowQualityBlocksNfiq(int[,] qualityMap)
    {
      double result = 0;
      qualityMap.Select2D(x => result += (x < 2) ? 1 : 0);

      result /= (qualityMap.GetLength(0) * qualityMap.GetLength(1));
      return result;
    }

    public static double GetAverageQualityNfiq(int[,] qualityMap)
    {
      double result = 0;
      qualityMap.Select2D(x => result += x);
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
      return (numOfBlackPixels / (img.GetLength(0) * img.GetLength(1)))*100;
    }

    public static double GetBackgroundPercentage(double[,] img)
    {
      return BackgroundQalifier.GetBackgroundRercentage(img, Constants.SegmentationWindowSize, Constants.SegmentationWeight);
    }
  }
}
