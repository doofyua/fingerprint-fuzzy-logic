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
    /// Делит изображение на mw*mh равных блоков
    /// Возвращает список сдвигов с верхнего левого угла для каждого блока
    /// Блоки не пересекают друг друга.
    /// Для изображений НЕ кратных BLOCKSIZE, блоки последнего столбца и\или строки 
    /// перекрывают друг друга. Данные становятся чуть более "грязными".
    /// </summary>
    /// <param name="ow"></param> - количество горизонтальных блоков входного изображения
    /// <param name="oh"></param> - количество вертикальных блоков входного изображения
    /// <param name="iw"></param> - ширина в пикселях исходного изображения
    /// <param name="ih"></param> - высота в пикселях исходного изображения
    /// <param name="pad"></param> - рамка, необходимая для поддержки 
    /// желаемого диапазона направлений блока для анализа DFT. 
    /// Это заполнение необходимо по всему периметру входного изображения. 
    /// Для некоторых случаев, рамка может быть нулевой.
    /// <param name="blocksize"></param> - ширина и высота каждого блока изображения
    /// <returns> Возвращается список сдвигов в пикселях для каждого блока изображения</returns>
    internal static int[,] block_offsets(ref int ow, ref int oh, int iw, int ih, int pad, int blocksize)
    {
      // проверка того, что изображение без сдвигов (padding) не меньше одного блока
      if (iw < blocksize || ih < blocksize)
      {
        throw new Exception(String.Format(
            "ERROR : block_offsets : image must be at least {0} by {1} in size\n",
            blocksize, blocksize));
      }
      // попробую вернуть 38*38 матрицу со значениями - смещениями
      int offset;
      // вычисляются padded ширина (и высота) изображения
      int pad2 = pad << 1; // ???
      int pw = iw + pad2;
      // int ph = ih + pad2  // не используется

      int bi = 0;
      int blkrow_start = (pad * pw) + pad;
      int blkrow_size = pw * blocksize;

      // количество столбцов и строк блоков в изображении
      // берётся приближённое значение 
      // 37,5 ~ 38 !!!!
      // Take the ceiling to account for "leftovers" at the right and bottom 
      //of the unpadded image
      ow = (int)Math.Ceiling(iw / (double)blocksize);
      oh = (int)Math.Ceiling(ih / (double)blocksize);

      // общее количество блоков изображения
      int bsize = ow * oh;
      int lastbw = ow - 1;
      int lastbh = oh - 1;
      int[] blkoffs = new int[bsize];
      int[,] blkoffs2D = new int[ow, oh];

      for (int by = 0; by < lastbh; by++)
      {
        offset = blkrow_start;

        for (int bx = 0; bx < lastbw; bx++)
        {
          blkoffs[bi++] = offset;
          blkoffs2D[bx, by] = offset;
          offset += blocksize;
        }

        // вычисления значений для кусочных блоков строки
        // справа внизу
        //Вот ТУТ возникает пересечение крайних блоков
        blkoffs[bi++] = blkrow_start + iw - blocksize;
        blkoffs2D[lastbw, by] = blkrow_start + iw - blocksize;
        blkrow_start += blkrow_size;
      }

      // вычисления значений для кусочных блоков столбца (т.е. для всех из последней строки)
      // справа внизу
      //Вот ТУТ возникает пересечение крайних блоков
      blkrow_start = ((pad + ih - blocksize) * pw) + pad;
      offset = blkrow_start;

      for (int bx = 0; bx < lastbw; bx++)
      {
        blkoffs[bi++] = offset;
        blkoffs2D[bx, lastbh] = offset;
        offset += blocksize;
      }

      blkoffs[bi] = blkrow_start + iw - blocksize;
      blkoffs2D[lastbw, lastbh] = blkrow_start + iw - blocksize;

      //return blkoffs;
      return blkoffs2D;
    }

    /// <summary>
    /// Берёт смещение к блоку указанного положения
    /// и анализирует интенсивности пикселей в блоке,
    /// чтобы определить, есть ли достаточный контраст для дальнейшей обработки
    /// </summary>
    /// <param name="blkoffset"></param>
    /// <param name="blkoffsetX"></param>
    /// <param name="blkoffsetY"></param>
    /// <param name="blocksize"></param>
    /// <param name="pdata"></param>
    /// <param name="pw"></param>
    /// <param name="ph"></param>
    /// <returns></returns>
    internal static bool low_contrast_block(int blkoffset, int blkoffsetX, int blkoffsetY, int blocksize, int[,] pdata, int pw, int ph)
    {
      //int[] pixtable = new int[IMG_6BIT_PIX_LIMIT]; // why 6 bit???
      const int IMG_8BIT_PIX_LIMIT = 256;
      int[] pixtable = new int[IMG_8BIT_PIX_LIMIT];  // т.к. 8 бит => 256 оттенков
      int pi = 0;
      int prctmin = 0, prctmax = 0;
      int pixsum = 0, found = FALSE;

      for (int index = 0; index < IMG_8BIT_PIX_LIMIT; index++)
      {
        pixtable[index] = 0;
      }

      double tdbl = (lfsparms.percentile_min_max / 100.0) * ((double)(blocksize * blocksize - 1));
      tdbl = trunc_dbl_precision(tdbl, TRUNC_SCALE);
      int prctthresh = Sround(tdbl);
      int pptrX = blkoffsetX;
      int pptrY = blkoffsetY;
      int index_pixtable;

      for (int py = 0; py < blocksize; py++)
      {
        for (int px = 0; px < blocksize; px++)
        {
          index_pixtable = pdata[pptrX, pptrY]; // index = 238, but pixtable[64]
          pixtable[index_pixtable]++;

          if (pptrX >= pw - 1)
          {
            pptrX = 0;
            pptrY++;
          }
          else
          {
            pptrX++;
          }

          //IncrementPointer(ref pptrX, ref pptrY, blocksize, blocksize, 1);
        }

        pptrX = blkoffsetX;
        pptrY++;
      }

      // Считаем сумму первых значений из pixtable
      // Хотим, чтобы количество было больше порога prctthresh
      // Т.е. чтобы пикселей из pdata с меньшими значениями было не менее prctthresh
      // Соответственно тогда говорим, что пиксели значений с 0 по pi - какие-то особенные
      while (pi < IMG_8BIT_PIX_LIMIT)
      {
        pixsum += pixtable[pi];

        if (pixsum >= prctthresh)
        {
          prctmin = pi;
          found = TRUE;
          break;
        }

        pi++;
      }

      if (found == FALSE)
      {
        throw new Exception("ERROR : low_contrast_block : min percentile pixel not found\n");
      }

      pi = IMG_8BIT_PIX_LIMIT - 1;
      pixsum = 0;
      found = FALSE;

      // Считаем сумму последних значений из pixtable
      // Хотим, чтобы количество было больше порога prctthresh
      // Т.е. чтобы пикселей из pdata с большими значениями было не менее prctthresh
      // Соответственно тогда говорим, что пиксели значений с pi по последнее - какие-то особенные
      while (pi >= 0)
      {
        pixsum += pixtable[pi];
        if (pixsum >= prctthresh)
        {
          prctmax = pi;
          found = TRUE;
          break;
        }
        pi--;
      }

      if (found == FALSE)
      {
        throw new Exception("ERROR : low_contrast_block : max percentile pixel not found\n");
      }

      // возвращаем значение: контраст не менее порога lfsparms.min_contrast_delta
      //return prctmax - prctmin < lfsparms.min_contrast_delta;
      return prctmax - prctmin < 10;
    }

    internal static int find_valid_block(ref int nbr_dir, ref int nbr_x, ref int nbr_y,
                 ref int[,] direction_map, ref int[,] low_contrast_map, int sx, int sy,
                 int mw, int mh, int x_incr, int y_incr)
    {
      int dir;

      /* Initialize starting block coords. */
      int x = sx + x_incr;
      int y = sy + y_incr;

      /* While we are not outside the boundaries of the map ... */
      while ((x >= 0) && (x < mw) && (y >= 0) && (y < mh))
      {
        /* Stop unsuccessfully if we encounter a LOW CONTRAST block. */
        if (low_contrast_map[x, y] != 0)
        {
          return (NOT_FOUND);
        }

        /* Stop successfully if we encounter a block with valid direction. */
        dir = direction_map[x, y];

        if (dir >= 0)
        {
          nbr_dir = dir;
          nbr_x = x;
          nbr_y = y;

          return (FOUND);
        }

        /* Otherwise, advance to the next block in the map. */
        x += x_incr;
        y += y_incr;
      }

      /* If we get here, then we did not find a valid block in the given */
      /* direction in the map.                                           */
      return (NOT_FOUND);
    }

    internal static void set_margin_blocks(ref int[,] map, int mw, int mh, int margin_value)
    {
      for (int x = 0; x < mw; x++)
      {
        map[x, 0] = margin_value;
        map[x, mh - 1] = margin_value;
      }

      for (int y = 1; y < mh - 1; y++)
      {
        map[0, y] = margin_value;
        map[mw - 1, y] = margin_value;
      }
    }
  }
}
