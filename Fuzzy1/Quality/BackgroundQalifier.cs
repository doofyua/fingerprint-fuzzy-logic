using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CUDAFingerprinting.Common;
using CUDAFingerprinting.Common.OrientationField;


namespace Fuzzy1
{

  public static class BackgroundQalifier
  {
    private static int N;
    private static int M;
    
    /// <summary>
    /// Return double between 0 and 100.
    /// </summary>
    public static double GetBackgroundRercentage(double[,] img, int windowSize, double weight)
    {
      int badRegions = 0;
      //int mediumRegions = 0;
      int goodRegions = 0;
      double numberOfRegions;

      int[,] xGradients = OrientationFieldGenerator.GenerateXGradients(img.Select2D(a => (int)a));
      int[,] yGradients = OrientationFieldGenerator.GenerateYGradients(img.Select2D(a => (int)a));

      double[,] magnitudes =
            xGradients.Select2D(
                (value, x, y) => Math.Sqrt(xGradients[x, y] * xGradients[x, y] + yGradients[x, y] * yGradients[x, y]));

      double averege = KernelHelper.Average(magnitudes);

      double[,] window = new double[windowSize, windowSize];

      N = (int)Math.Ceiling(((double)img.GetLength(0)) / windowSize);
      M = (int)Math.Ceiling(((double)img.GetLength(1)) / windowSize);

      numberOfRegions = N * M;

      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < M; j++)
        {

          /////////////////////////////////////////////
          window = window.Select2D((value, x, y) =>
            {
              if (i * windowSize + x >= magnitudes.GetLength(0)
                  || j * windowSize + y >= magnitudes.GetLength(1))
              {
                return 0;
              }

              return magnitudes[(int)(i * windowSize + x), (int)(j * windowSize + y)];
            });

          //////////////////////////////////////////

          if (KernelHelper.Average(window) < averege * weight)
          {
            badRegions++;
          }
          else
          {          
              goodRegions++;            
          }
        }
      }
      return ((badRegions) / numberOfRegions)*100;
    }

    public static int[,] GetBigMask(int[,] mask, int imgX, int imgY, int windowSize)
    {
      int[,] bigMask = new int[imgX, imgY];

      bigMask = bigMask.Select2D((value, x, y) =>
      {
        int xBlock = (int)(((double)x) / windowSize);
        int yBlock = (int)(((double)y) / windowSize);
        return mask[xBlock, yBlock];
      });

      return bigMask;
    }

    // mask is supposed to be the size of the image
    public static double[,] ColorImage(double[,] img, int[,] mask)
    {
      return img.Select2D((value, x, y) => mask[x, y] > 0 ? img[x, y] : 255);
    }

    public static float[] ColorImage(float[] img, int rows, int columns, int[,] mask)
    {
      return img.Select2D(rows, columns, (value, x, y) => mask[x, y] > 0 ? img[x * columns + y] : 255);
    }   
  }
}

