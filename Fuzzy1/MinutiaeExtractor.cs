using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUDAFingerprinting.Common;
using CUDAFingerprinting.Common.Segmentation;
using CUDAFingerprinting.Common.OrientationField;
using CUDAFingerprinting.ImageEnhancement.LinearSymmetry;
using CUDAFingerprinting.TemplateBuilding.Minutiae.BinarizationThinning;

namespace Fuzzy1
{
  internal static class MinutiaeExtractor
  {

    public static List<Minutia> ExtractCUDAFpMinutiae(double[,] image, int rows, int columns)
    {
     
      int[,] mask;

      var doubleImage = SegmentImage(image, rows, columns, out mask);

      var orField = MakeOrientationField(doubleImage, rows, columns, Constants.OrFieldWindowSize,
                                         Constants.OrFieldOverlap);

      doubleImage = BinarizeImage(doubleImage, rows, columns);

      doubleImage = ThinImage(doubleImage, rows, columns);

      int orFieldWidth = columns / (Constants.OrFieldWindowSize - Constants.OrFieldOverlap);
      int orFieldHeight = rows / (Constants.OrFieldWindowSize - Constants.OrFieldOverlap);

      var minutiae = FindMinutiae(doubleImage, rows, columns, orField, orFieldHeight, orFieldWidth, mask);

      return minutiae;
      
     }

   private  static List<Minutia> FindMinutiae(double[,] startImg, int rows, int columns, double[,] orField2D, int orFieldRows, int orFieldColumns, int[,] mask)
    {
      
      var minutiae = MinutiaeDetection.FindMinutiae(startImg);
      //--------------------------------
      var trueMinutiae = new List<Minutia>();
      for (int i = 0; i < minutiae.Count; i++)
      {
        if (mask[minutiae[i].Y / Constants.SegmentationWindowSize, minutiae[i].X / Constants.SegmentationWindowSize] != 0)
        {
          trueMinutiae.Add(minutiae[i]);
        }
      }
      minutiae = trueMinutiae;

      //--------------------------------
      minutiae = MinutiaeDetection.FindBigMinutiae(minutiae);

      MinutiaeDirection.FindDirection(orField2D.Select2D(x => Math.PI - x),
                                      Constants.OrFieldWindowSize - Constants.OrFieldOverlap, minutiae,
                                      startImg.Select2D(x => (int)x), 4);

      return minutiae;
    }

    public static double[,] MakeOrientationField(double[,] image, int rows, int columns, int regionSize, int overlap)
    {
      
      return OrientationFieldGenerator.GenerateOrientationField(image.Select2D(x => (int)x));
    }

    public static double[,] ThinImage(double[,] image, int rows, int columns)
    {     
      return Thining.ThinPicture(image);
    }

    public static double[,] BinarizeImage(double[,] image, int rows, int columns)
    {      
      var newImage = ImageEnhancementHelper.EnhanceImage(image);

      return GlobalBinarization.Binarization(newImage, Constants.BinarizationThreshold);
    }

    public static double[,] SegmentImage(double[,] image, int rows, int columns, out int[,] mask)
    {      
      mask = Segmentator.Segmetator(image,Constants.SegmentationWindowSize, Constants.SegmentationWeight,Constants.SegmentationThreshold);
      return Segmentator.ColorImage(image,Segmentator.GetBigMask(mask, image.GetLength(0), image.GetLength(1),Constants.SegmentationWindowSize));
    }
  }
}
