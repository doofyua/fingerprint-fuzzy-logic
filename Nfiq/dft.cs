using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
  public static partial class Nfiq
  {
    /// <summary>
    /// Проводит DFT анализ для блока.
    /// Блок отбирается по ряду направлений и по нескольким видам сигнала (волн)
    /// различной частоты, применённой для каждого направления.
    /// Для каждого направления пиксели суммируются по строкам и получается 
    /// вектор сумм. Каждый DFT вид сигнала затем применяется индивидуально этому вектору.
    /// Значение мощности DFT вычисляется для каждого типа сигнала.
    /// (frequency) на каждом направлении блока.
    /// Более того итоговый результат - вектор N сигналов * M направлений
    /// Используется для определения доминирующего направления.
    /// </summary>
    /// <param name="powers"></param>
    /// <param name="pdata"></param>
    /// <param name="blkoffset"></param>
    /// <param name="blkoffsetX"></param>
    /// <param name="blkoffsetY"></param>
    /// <param name="pw"></param>
    /// <param name="ph"></param>
    /// <param name="dftwaves"></param>
    /// <param name="dftgrids"></param>
    internal static void dft_dir_powers(ref double[,] powers, ref int[,] pdata,
                int blkoffset, int blkoffsetX, int blkoffsetY, int pw, int ph, DFTWAVES dftwaves, ROTGRIDS dftgrids)
    {
      if (dftgrids.grid_w != dftgrids.grid_h)
      {
        throw new Exception("ERROR : dft_dir_powers : DFT grids must be square\n");
      }

      int[] rowsums = new int[dftgrids.grid_w];

      // для кажого направления
      for (int dir = 0; dir < dftgrids.ngrids; dir++)
      {
        sum_rot_block_rows(ref rowsums, pdata, blkoffsetX, blkoffsetY, dftgrids.grids[dir], dftgrids.grid_w);

        // для каждого варианта сигнала\волны
        for (int w = 0; w < dftwaves.nwaves; w++)
        {
          powers[w, dir] = dft_power(rowsums, dftwaves.waves[w], dftwaves.wavelen);
        }
      }
    }

    internal static void sum_rot_block_rows(ref int[] rowsums, int[,] pdata, int blkoffsetX, int blkoffsetY,
    List<int> grid_offsets, int blocksize)
    {
      int gi = 0;

      for (int iy = 0; iy < blocksize; iy++)
      {
        rowsums[iy] = 0;

        for (int ix = 0; ix < blocksize; ix++)
        {
          /* Accumulate pixel value at rotated grid position in image */
          //rowsums[iy] += *(blkptr + grid_offsets[gi]);
          rowsums[iy] += GetValueOfArray(pdata, blkoffsetX, blkoffsetY, grid_offsets[gi]);
          gi++;
        }
      }
    }

    internal static double dft_power(int[] rowsums, DFTWAVE wave, int wavelen)
    {
      double cospart = 0.0, sinpart = 0.0;

      for (int i = 0; i < wavelen; i++)
      {
        cospart += rowsums[i] * wave.cos[i];
        sinpart += rowsums[i] * wave.sin[i];
      }

      return cospart * cospart + sinpart * sinpart;
    }

    internal static void dft_power_stats(ref int[] wis, ref double[] powmaxs, ref int[] powmax_dirs,
       ref double[] pownorms, double[,] powers, int fw, int tw, int ndirs)
    {
      double powmean, max_v, powsum;

      for (int w = fw, i = 0; w < tw; w++, i++)
      {
        max_v = powers[w, 0];
        powsum = powers[w, 0];

        for (int dir = 1; dir < ndirs; dir++)
        {
          powsum += powers[w, dir];

          if (powers[w, dir] > max_v)
          {
            max_v = powers[w, dir];
            powmax_dirs[i] = dir;
          }
        }

        powmaxs[i] = max_v;
        powmean = Max(powsum, MIN_POWER_SUM) / (double)ndirs;
        pownorms[i] = powmaxs[i] / powmean;
      }

      int nstats = tw - fw;
      double[] pownorms2 = new double[nstats];

      for (int i = 0; i < nstats; i++)
      {
        wis[i] = i;
        pownorms2[i] = powmaxs[i] * pownorms[i];
      }

      bubble_sort_double_dec_2(pownorms2, ref wis, nstats);
    }
  }
}
