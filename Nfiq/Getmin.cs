using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Nfiq
{
  public static partial class Nfiq
  {
    private static int[,] quality_map = null;

    public static void CalculateQualityMap(Bitmap bitmap, string name)
    {
      int iw = bitmap.Width;
      int ih = bitmap.Height;
      int[,] direction_map = new int[iw, ih];
      int[,] low_contrast_map = new int[iw, ih];
      int[,] low_flow_map = new int[iw, ih];
      int[,] high_curve_map = new int[iw, ih];
      int map_w = 0, map_h = 0;
      int[,] bdata = new int[iw, ih];

      lfs_detect_minutiae_V2(ref direction_map, ref low_contrast_map, ref low_flow_map,
          ref high_curve_map, ref bdata, bitmap, iw, ih, ref map_w, ref map_h, name);

      quality_map = gen_quality_map(direction_map, low_contrast_map,
          low_flow_map, high_curve_map, map_w, map_h);

     // PrintMap(quality_map, name + "_qualityMap");
    }

    public static int[,] GetQualityMap(Bitmap bitmap, string name)
    {
      CalculateQualityMap(bitmap, name);
      
      return quality_map;
    }

    public static int Main()
    {
      return 0;
    }
  }
}

