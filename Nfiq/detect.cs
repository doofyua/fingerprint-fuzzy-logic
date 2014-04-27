using System;
using System.Drawing;

namespace Nfiq
{
  public static partial class Nfiq
  {
    internal static int lfs_detect_minutiae_V2(ref int[,] odmap, ref int[,] olcmap, ref int[,] olfmap,
        ref int[,] ohcmap, ref int[,] obdata, Bitmap idata, int iw, int ih, ref int mw, ref int mh, string name)
    {
      int[,] pdata = new int[iw, ih];
      int[,] bdata = new int[iw, ih];
      DIR2RAD dir2rad = new DIR2RAD();
      DFTWAVES dftwaves = new DFTWAVES();
      ROTGRIDS dftgrids = new ROTGRIDS();
      ROTGRIDS dirbingrids = new ROTGRIDS();
      int[,] direction_map = new int[iw, ih];
      int[,] low_contrast_map = new int[iw, ih];
      int[,] low_flow_map = new int[iw, ih];
      int[,] high_curve_map = new int[iw, ih];
      int maxpad = 0;

      int pw = 0, ph = 0;

      // initialization
      maxpad = get_max_padding_V2(lfsparms.windowsize, lfsparms.windowoffset,
          lfsparms.dirbin_grid_w, lfsparms.dirbin_grid_h);
      dir2rad = init_dir2rad(lfsparms.num_directions);
      dftwaves = init_dftwaves(dft_coefs, lfsparms.num_dft_waves, lfsparms.windowsize);
      dftgrids = init_rotgrids(iw, ih, maxpad, lfsparms.start_dir_angle,
          lfsparms.num_directions, lfsparms.windowsize, lfsparms.windowsize, RELATIVE2ORIGIN);

      if (maxpad > 0)
      {
        // печать
        pdata = pad_uchar_image(ref pw, ref ph, idata, iw, ih, maxpad, lfsparms.pad_value);
        // PrintMap(pdata, "pdata");
      }
      else
      {
        for (int i = 0; i < iw; i++)
        {
          for (int j = 0; j < ih; j++)
          {
            pdata[i, j] = idata.GetPixel(i, j).R;
          }
        }

        pw = iw;
        ph = ih;
      }

      gen_image_maps(ref direction_map, ref low_contrast_map, ref low_flow_map, ref high_curve_map,
          ref mw, ref mh, pdata, pw, ph, dir2rad, dftwaves, dftgrids, name);

      /* Assign results to output pointers. */
      odmap = direction_map;
      olcmap = low_contrast_map;
      olfmap = low_flow_map;
      ohcmap = high_curve_map;
      obdata = bdata;

      return (0);
    }
  }
}