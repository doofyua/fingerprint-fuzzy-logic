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
    /// Вычисляем карты для изображения (для Version 2 of the NIST LFS System)
    /// карта направлений (Direction Map) - матрица, целые значения, доминирующее направление
    /// карта контрастов (Low Contrast Map) - матрица, помечены блоки низкого контраста
    /// карта потоков (Low Flow Map) - нет выделенного направления
    /// карта кривизны (High Curve Map) - блоки высокой кривизны
    /// Изображение: произвольное, не квадратное
    /// </summary>
    /// <param name="odmap">карта направлений</param>
    /// <param name="olcmap">карта контраста</param>
    /// <param name="olfmap">карта потоков</param>
    /// <param name="ohcmap">карта кривизны</param>
    /// <param name="omw"> количество блоков по горизонтали</param>
    /// <param name="omh">количество блоков по вертикали</param>
    /// <param name="pdata">padded входное изображение</param>
    /// <param name="pw">padded ширина</param>
    /// <param name="ph">padded высота</param>
    /// <param name="dir2rad">Поисковая таблица преобразования целых направлений</param>
    /// <param name="dftwaves">структура для DFT сигналов (wave forms)</param>
    /// <param name="dftgrids">структура смещений для повёрнутых пикселей окна\грида  (rotated pixel grid offsets)</param>
    internal static void gen_image_maps(ref int[,] odmap, ref int[,] olcmap, ref int[,] olfmap, ref int[,] ohcmap,
       ref int omw, ref int omh, int[,] pdata, int pw, int ph,
       DIR2RAD dir2rad, DFTWAVES dftwaves, ROTGRIDS dftgrids, string name)
    {
      int[,] direction_map = new int[omw, omh];
      int[,] low_contrast_map = new int[omw, omh];
      int[,] low_flow_map = new int[omw, omh];
      int mw = 0, mh = 0;

      if (dftgrids.grid_w != dftgrids.grid_h)
      {
        throw new Exception("ERROR : gen_image_maps : DFT grids must be square\n");
      }

      int iw = pw - (dftgrids.pad << 1);
      int ih = ph - (dftgrids.pad << 1);
      // int blkoffsCount = ((int)Math.Ceiling(iw / (double)lfsparms.blocksize)) *
      //                    (int)Math.Ceiling(ih / (double)lfsparms.blocksize);

      // blkoffsCount = blkoffs.GetLength(0) * blkoffs.GetLength(1)  - проверено
      // считаем смещения для блоков
      // предполагая, что у нас +12 пикселей со всех сторон изображения со значениями 128
      // чтобы нормально считать в окнах
      int[,] blkoffs = block_offsets(ref mw, ref mh, iw, ih, dftgrids.pad, lfsparms.blocksize);

      //  PrintMap(blkoffs, "blkoffs");

      gen_initial_maps(ref direction_map, ref low_contrast_map, ref low_flow_map,
          blkoffs, mw, mh, ref pdata, pw, ph, dftwaves, dftgrids);

      // печать
      //  PrintMap(direction_map, "direction_map");
      //  PrintMap(low_contrast_map, "low_contrast_map");
      //  PrintMap(low_flow_map, "low_flow_map");

      // direction_map не меняется, также low_flow = 0
      // но low_contrast = 1

      morph_TF_map(ref low_flow_map, mw, mh);
      remove_incon_dirs(ref direction_map, mw, mh, dir2rad);
      smooth_direction_map(ref direction_map, ref low_contrast_map, mw, mh, dir2rad);
      interpolate_direction_map(ref direction_map, ref low_contrast_map, mw, mh);
      remove_incon_dirs(ref direction_map, mw, mh, dir2rad);
      smooth_direction_map(ref direction_map, ref low_contrast_map, mw, mh, dir2rad);
      set_margin_blocks(ref direction_map, mw, mh, INVALID_DIR);

      int[,] high_curve_map = gen_high_curve_map(ref direction_map, mw, mh);

      odmap = direction_map;
      olcmap = low_contrast_map;
      olfmap = low_flow_map;
      ohcmap = high_curve_map;
      omw = mw;
      omh = mh;

      // печать
      //PrintMap(direction_map, name + "_direction_map");
      //PrintMap(low_contrast_map, name + "_low_contrast_map");
      //PrintMap(low_flow_map, name + "_low_flow_map");
      //PrintMap(high_curve_map, name + "_high_curve_map");
    }

    /// <summary>
    /// Создаёт начальную карту направлений (Direction Map)
    /// Важно, чтобы были те самые padding - чтобы не вылезти за границы
    /// Странные направления сгладятся и уйдут после основанного на DFT анализа
    /// Валидные значения >=0, невалидные -1
    /// Также возвращаются две карты: 
    /// карта контрастов (Low Contrast Map)
    ///  - помечает блоки с низким контрастом (у них невалидное значение - направление)
    /// карта потоков(Low Flow Map)
    /// - помечает блоки, в которых DFT анализ не нашёл направления (также невалидное направление)
    /// </summary>
    /// <param name="odmap">карта направлений</param>
    /// <param name="olcmap">карта контрастов</param>
    /// <param name="olfmap">карта потоков</param>
    /// <param name="blkoffs">смещения блоков</param>
    /// <param name="mw">количество блоков по горизонтали в padded изображении</param>
    /// <param name="mh">количество блоков по вертикали в padded изображении</param>
    /// <param name="pdata">padded входное изображение</param>
    /// <param name="pw">ширина padded изображения</param>
    /// <param name="ph">высота padded изображения</param>
    /// <param name="dftwaves">структура для DFT сигналов (wave forms)</param>
    /// <param name="dftgrids">структура смещений для повёрнутых пикселей окна\грида (rotated pixel grid offsets)</param>
    internal static void gen_initial_maps(ref int[,] odmap, ref int[,] olcmap, ref int[,] olfmap,
        int[,] blkoffs, int mw, int mh, ref int[,] pdata, int pw, int ph, DFTWAVES dftwaves, ROTGRIDS dftgrids)
    {
      int nstats = dftwaves.nwaves - 1;
      int[,] direction_map = new int[mw, mh];
      int[,] low_contrast_map = new int[mw, mh];
      int[,] low_flow_map = new int[mw, mh];

      double[,] powers = new double[dftwaves.nwaves, dftgrids.ngrids];

      int[] wis = new int[nstats];
      double[] powmaxs = new double[nstats];
      int[] powmax_dirs = new int[nstats];
      double[] pownorms = new double[nstats];

      int xminlimit = dftgrids.pad;
      int xmaxlimit = pw - dftgrids.pad - lfsparms.windowsize - 1;
      int yminlimit = dftgrids.pad;
      int ymaxlimit = ph - dftgrids.pad - lfsparms.windowsize - 1;

      int win_x, win_y, low_contrast_offset;
      int blkdir;
      int dft_offset;

      for (int jndex = 0; jndex < mh; jndex++)
      {
        for (int index = 0; index < mw; index++)
        {
          // инициализируем карты
          // direction_map    -1
          // low_contrast_map  0
          // low_flow_map      0
          direction_map[index, jndex] = INVALID_DIR;
          low_contrast_map[index, jndex] = 0;
          low_flow_map[index, jndex] = 0;
        }
      }

      for (int jndex = 0; jndex < mh; jndex++)
      {
        for (int index = 0; index < mw; index++)
        {
          // dft_offset = blkoffs[bi] - (lfsparms->windowoffset * pw) - lfsparms->windowoffset;
          dft_offset = blkoffs[index, jndex] - (lfsparms.windowoffset * pw) - lfsparms.windowoffset;
          win_x = dft_offset % pw;
          win_y = (int)(dft_offset / pw);

          // проверка на попадание в padding (т.е. рамку)
          win_x = Max(xminlimit, win_x);
          win_x = Min(xmaxlimit, win_x);
          win_y = Max(yminlimit, win_y);
          win_y = Min(ymaxlimit, win_y);
          low_contrast_offset = (win_y * pw) + win_x; // просто передаём координаты

          if (low_contrast_block(low_contrast_offset, win_x, win_y, lfsparms.windowsize, pdata, pw, ph))
          {
            /* block is low contrast ... */
            low_contrast_map[index, jndex] = TRUE;
            continue;
          }

          //  вычислить dft веса // blkoffs[bi] ? low_contrast_offset, win_x, win_y
          dft_dir_powers(ref powers, ref pdata, low_contrast_offset, win_x, win_y, pw, ph, dftwaves, dftgrids);

          // вычислить статистики для dft весов (?)
          dft_power_stats(ref wis, ref powmaxs, ref powmax_dirs, ref pownorms, powers,
              1, dftwaves.nwaves, dftgrids.ngrids);

          // Проведение _первичного_ теста для определения направления
          blkdir = primary_dir_test(ref powers, wis, powmaxs, powmax_dirs, pownorms, nstats);

          if (blkdir != INVALID_DIR)
          {
            direction_map[index, jndex] = blkdir;
            continue;
          }

          // Проведение _вторичного_ теста для определения направления (вилка)
          blkdir = secondary_fork_test(ref powers, wis, powmaxs, powmax_dirs, pownorms, nstats);

          if (blkdir != INVALID_DIR)
          {
            direction_map[index, jndex] = blkdir;
            continue;
          }

          low_flow_map[index, jndex] = TRUE;
        }
      }

      odmap = direction_map;
      olcmap = low_contrast_map;
      olfmap = low_flow_map;
    }

    internal static int primary_dir_test(ref double[,] powers, int[] wis, double[] powmaxs,
      int[] powmax_dirs, double[] pownorms, int nstats)
    {
      int index = 0;

      for (int w = 0; w < nstats; w++)
      {
        index = wis[w];

        if ((powmaxs[index] > lfsparms.powmax_min) &&
           (pownorms[index] > lfsparms.pownorm_min) &&
           (powers[0, powmax_dirs[index]] <= lfsparms.powmax_max))
        {
          return powmax_dirs[index];
        }
      }

      return INVALID_DIR;
    }

    internal static int secondary_fork_test(ref double[,] powers, int[] wis, double[] powmaxs,
       int[] powmax_dirs, double[] pownorms, int nstats)
    {
      double fork_pownorm_min = lfsparms.fork_pct_pownorm * lfsparms.pownorm_min;

      if ((powmaxs[wis[0]] > lfsparms.powmax_min) &&
         (pownorms[wis[0]] >= fork_pownorm_min) &&
         (powers[0, powmax_dirs[wis[0]]] <= lfsparms.powmax_max))
      {
        int rdir = (powmax_dirs[wis[0]] + lfsparms.fork_interval) % lfsparms.num_directions;
        int ldir = (powmax_dirs[wis[0]] + lfsparms.num_directions - lfsparms.fork_interval) % lfsparms.num_directions;
        double fork_pow_thresh = powmaxs[wis[0]] * lfsparms.fork_pct_powmax;

        if (((powers[wis[0] + 1, ldir] <= fork_pow_thresh) ||
            (powers[wis[0] + 1, rdir] <= fork_pow_thresh)) &&
           ((powers[wis[0] + 1, ldir] > fork_pow_thresh) ||
            (powers[wis[0] + 1, rdir] > fork_pow_thresh)))
        {
          return powmax_dirs[wis[0]];
        }
      }

      return INVALID_DIR;
    }

    /// <summary>
    /// Берёт карту и пытается заполнить пустоты, анализируя соседние блоки
    /// </summary>
    /// <param name="tfmap"></param>
    /// <param name="mw"></param>
    /// <param name="mh"></param>
    internal static void morph_TF_map(ref int[,] tfmap, int mw, int mh)
    {
      int[,] cimage = new int[mw, mh];
      int[,] mimage = new int[mw, mh];

      for (int i = 0; i < mw; i++)
      {
        for (int j = 0; j < mh; j++)
        {
          cimage[i, j] = tfmap[i, j];
        }
      }

      //  PrintMap(cimage, "start");
      dilate_charimage_2(ref cimage, ref mimage, mw, mh);
      //  PrintMap(mimage, "dilate_charimage1");
      dilate_charimage_2(ref mimage, ref cimage, mw, mh);
      //  PrintMap(cimage, "dilate_charimage2");
      erode_charimage_2(ref cimage, ref mimage, mw, mh);
      //   PrintMap(mimage, "erode_charimage1");
      erode_charimage_2(ref mimage, ref cimage, mw, mh);
      //  PrintMap(cimage, "erode_charimage2");

      for (int i = 0; i < mw; i++)
      {
        for (int j = 0; j < mh; j++)
        {
          tfmap[i, j] = cimage[i, j];
        }
      }
    }

    internal static void remove_incon_dirs(ref int[,] imap, int mw, int mh, DIR2RAD dir2rad)
    {
      int nremoved;
      int lbox, rbox, tbox, bbox;

      /* Compute center coords of IMAP */
      int cx = mw >> 1;
      int cy = mh >> 1;

      /* Do pass, while directions have been removed in a pass ... */
      do
      {
        /* Reinitialize number of removed directions to 0 */
        nremoved = 0;

        /* If valid IMAP direction and test for removal is true ... */
        if ((imap[cx, cy] != INVALID_DIR) && remove_dir(ref imap, cx, cy, mw, mh, dir2rad))
        {
          /* Set to INVALID */
          imap[cx, cy] = INVALID_DIR;
          /* Bump number of removed IMAP directions */
          nremoved++;
        }

        /* Initialize side indices of concentric boxes */
        lbox = cx - 1;
        tbox = cy - 1;
        rbox = cx + 1;
        bbox = cy + 1;

        /* Grow concentric boxes, until ALL edges of imap are exceeded */
        while ((lbox >= 0) || (rbox < mw) || (tbox >= 0) || (bbox < mh))
        {

          /* test top edge of box */
          if (tbox >= 0)
            nremoved += test_top_edge(lbox, tbox, rbox, bbox, ref imap, mw, mh, dir2rad);

          /* test right edge of box */
          if (rbox < mw)
            nremoved += test_right_edge(lbox, tbox, rbox, bbox, ref imap, mw, mh, dir2rad);

          /* test bottom edge of box */
          if (bbox < mh)
            nremoved += test_bottom_edge(lbox, tbox, rbox, bbox, ref imap, mw, mh, dir2rad);

          /* test left edge of box */
          if (lbox >= 0)
            nremoved += test_left_edge(lbox, tbox, rbox, bbox, ref imap, mw, mh, dir2rad);

          /* Resize current box */
          lbox--;
          tbox--;
          rbox++;
          bbox++;
        }
      } while (nremoved != 0);
    }

    internal static int test_top_edge(int lbox, int tbox, int rbox,
             int bbox, ref int[,] imap, int mw, int mh, DIR2RAD dir2rad)
    {
      /* Initialize number of directions removed on edge to 0 */
      int nremoved = 0;

      /* Set start pointer to top-leftmost point of box, or set it to */
      /* the leftmost point in the IMAP row (0), whichever is larger. */
      int sx = Max(lbox, 0);

      /* Set end pointer to either 1 point short of the top-rightmost */
      /* point of box, or set it to the rightmost point in the IMAP   */
      /* row (lastx=mw-1), whichever is smaller.                      */
      int ex = Min(rbox - 1, mw - 1);

      /* For each point on box's edge ... */
      for (int bx = sx, by = tbox;
          bx <= ex && bx < imap.GetLength(0);
          bx++)
      {
        /* If valid IMAP direction and test for removal is true ... */
        if ((imap[bx, by] != INVALID_DIR) && (remove_dir(ref imap, bx, by, mw, mh, dir2rad)))
        {
          imap[bx, by] = INVALID_DIR;
          nremoved++;
        }
      }

      /* Return the number of directions removed on edge */
      return (nremoved);
    }

    internal static int test_right_edge(int lbox, int tbox, int rbox, int bbox, ref int[,] imap,
        int mw, int mh, DIR2RAD dir2rad)
    {
      int bx, by;

      /* Initialize number of directions removed on edge to 0 */
      int nremoved = 0;

      /* Set start pointer to top-rightmost point of box, or set it to */
      /* the topmost point in IMAP column (0), whichever is larger.    */
      int sy = Max(tbox, 0);

      /* Set end pointer to either 1 point short of the bottom-    */
      /* rightmost point of box, or set it to the bottommost point */
      /* in the IMAP column (lasty=mh-1), whichever is smaller.    */
      int ey = Min(bbox - 1, mh - 1);

      /* For each point on box's edge ... */
      for (bx = rbox, by = sy;
          (by < ey || (by == ey && bx <= rbox)) && by < imap.GetLength(1);
          by++)
      {
        /* If valid IMAP direction and test for removal is true ... */
        if ((imap[bx, by] != INVALID_DIR) && (remove_dir(ref imap, bx, by, mw, mh, dir2rad)))
        {
          /* Set to INVALID */
          imap[bx, by] = INVALID_DIR;
          /* Bump number of removed IMAP directions */
          nremoved++;
        }
      }

      /* Return the number of directions removed on edge */
      return (nremoved);
    }

    internal static int test_bottom_edge(int lbox, int tbox, int rbox, int bbox, ref int[,] imap,
        int mw, int mh, DIR2RAD dir2rad)
    {
      /* Initialize number of directions removed on edge to 0 */
      int nremoved = 0;

      /* Set start pointer to bottom-rightmost point of box, or set it to the */
      /* rightmost point in the IMAP ROW (lastx=mw-1), whichever is smaller.  */
      int sx = Min(rbox, mw - 1);

      /* Set end pointer to either 1 point short of the bottom-    */
      /* lefttmost point of box, or set it to the leftmost point   */
      /* in the IMAP row (x=0), whichever is larger.               */
      int ex = Max(lbox - 1, 0);

      /* For each point on box's edge ... */
      for (int bx = sx, by = bbox;
          bx >= 0 && bx >= ex && by >= bbox;
          bx--)
      {
        /* If valid IMAP direction and test for removal is true ... */
        if ((imap[bx, by] != INVALID_DIR) && (remove_dir(ref imap, bx, by, mw, mh, dir2rad)))
        {
          /* Set to INVALID */
          imap[bx, by] = INVALID_DIR;
          /* Bump number of removed IMAP directions */
          nremoved++;
        }
      }

      /* Return the number of directions removed on edge */
      return (nremoved);
    }

    internal static int test_left_edge(int lbox, int tbox, int rbox, int bbox, ref int[,] imap,
        int mw, int mh, DIR2RAD dir2rad)
    {
      /* Initialize number of directions removed on edge to 0 */
      int nremoved = 0;

      /* Set start pointer to bottom-leftmost point of box, or set it to */
      /* the bottommost point in IMAP column (lasty=mh-1), whichever     */
      /* is smaller.                                                     */
      int sy = Min(bbox, mh - 1);

      /* Set end pointer to either 1 point short of the top-leftmost */
      /* point of box, or set it to the topmost point in the IMAP    */
      /* column (y=0), whichever is larger.                          */
      int ey = Max(tbox - 1, 0);

      /* For each point on box's edge ... */
      for (int bx = lbox, by = sy;
          by >= ey && by >= 0;
          by--)
      {
        /* If valid IMAP direction and test for removal is true ... */
        if ((imap[bx, by] != INVALID_DIR) && (remove_dir(ref imap, bx, by, mw, mh, dir2rad)))
        {
          /* Set to INVALID */
          imap[bx, by] = INVALID_DIR;
          /* Bump number of removed IMAP directions */
          nremoved++;
        }
      }

      /* Return the number of directions removed on edge */
      return (nremoved);
    }

    internal static bool remove_dir(ref int[,] imap, int mx, int my, int mw, int mh, DIR2RAD dir2rad)
    {
      int avrdir = 0, nvalid = 0;
      double dir_strength = 0;

      /* Compute average direction from neighbors, returning the */
      /* number of valid neighbors used in the computation, and  */
      /* the "strength" of the average direction.                */
      average_8nbr_dir(ref avrdir, ref dir_strength, ref nvalid, imap, mx, my, mw, mh, dir2rad);

      /* Conduct valid neighbor test (Ex. thresh==3) */
      if (nvalid < lfsparms.rmv_valid_nbr_min)
      {
        // return 1
        return true;
      }

      /* If stregnth of average neighbor direction is large enough to */
      /* put credence in ... (Ex. thresh==0.2)                        */
      if (dir_strength >= lfsparms.dir_strength_min)
      {
        /* Conduct direction distance test (Ex. thresh==3) */
        /* Compute minimum absolute distance between current and       */
        /* average directions accounting for wrapping from 0 to NDIRS. */
        int dist = Math.Abs(avrdir - imap[mx, my]);
        dist = Min(dist, dir2rad.ndirs - dist);

        if (dist > lfsparms.dir_distance_max)
        {
          // return 2
          return true;
        }
      }

      // return 0
      return false;
    }

    internal static void average_8nbr_dir(ref int avrdir, ref double dir_strength, ref int nvalid,
                  int[,] imap, int mx, int my, int mw, int mh, DIR2RAD dir2rad)
    {
      double pi2, pi_factor, theta;
      double avr;

      /* Compute neighbor coordinates to current IMAP direction */
      int e = mx + 1;  /* East */
      int w = mx - 1;  /* West */
      int n = my - 1;  /* North */
      int s = my + 1;  /* South */

      /* Intialize accumulators */
      nvalid = 0;
      double cospart = 0.0;
      double sinpart = 0.0;

      /* 1. Test NW */
      /* If NW point within IMAP boudaries ... */
      if ((w >= 0) && (n >= 0) && (imap[w, n] != INVALID_DIR))
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[w, n]];
        sinpart += dir2rad.sin[imap[w, n]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 2. Test N */
      /* If N point within IMAP boudaries ... */
      if ((n >= 0) && (imap[mx, n] != INVALID_DIR))
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[mx, n]];
        sinpart += dir2rad.sin[imap[mx, n]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 3. Test NE */
      /* If NE point within IMAP boudaries ... */
      if ((e < mw) && (n >= 0) && (imap[e, n] != INVALID_DIR))
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[e, n]];
        sinpart += dir2rad.sin[imap[e, n]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 4. Test E */
      /* If E point within IMAP boudaries ... */
      if ((e < mw) && (imap[e, my] != INVALID_DIR))
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[e, my]];
        sinpart += dir2rad.sin[imap[e, my]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 5. Test SE */
      /* If SE point within IMAP boudaries ... */
      if ((e < mw) && (s < mh) && (imap[e, s] != INVALID_DIR))
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[e, s]];
        sinpart += dir2rad.sin[imap[e, s]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 6. Test S */
      /* If S point within IMAP boudaries ... */
      if (s < mh && imap[mx, s] != INVALID_DIR)
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[mx, s]];
        sinpart += dir2rad.sin[imap[mx, s]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 7. Test SW */
      /* If SW point within IMAP boudaries ... */
      if ((w >= 0) && (s < mh) && imap[w, s] != INVALID_DIR)
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[w, s]];
        sinpart += dir2rad.sin[imap[w, s]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* 8. Test W */
      /* If W point within IMAP boudaries ... */
      if ((w >= 0) && (imap[w, my] != INVALID_DIR))
      {
        /* Accumulate cosine and sine components of the direction */
        cospart += dir2rad.cos[imap[w, my]];
        sinpart += dir2rad.sin[imap[w, my]];
        /* Bump number of accumulated directions */
        nvalid++;
      }

      /* If there were no neighbors found with valid direction ... */
      if (nvalid == 0)
      {
        /* Return INVALID direction. */
        dir_strength = 0;
        avrdir = INVALID_DIR;
        return;
      }

      /* Compute averages of accumulated cosine and sine direction components */
      cospart /= (double)nvalid;
      sinpart /= (double)nvalid;

      /* Compute directional strength as hypotenuse (without sqrt) of average */
      /* cosine and sine direction components.  Believe this value will be on */
      /* the range of [0 .. 1].                                               */
      dir_strength = (cospart * cospart) + (sinpart * sinpart);
      /* Need to truncate precision so that answers are consistent   */
      /* on different computer architectures when comparing doubles. */
      dir_strength = trunc_dbl_precision(dir_strength, TRUNC_SCALE);

      /* If the direction strength is not sufficiently high ... */
      if (dir_strength < DIR_STRENGTH_MIN)
      {
        /* Return INVALID direction. */
        dir_strength = 0;
        avrdir = INVALID_DIR;
        return;
      }

      /* Compute angle (in radians) from Arctan of avarage         */
      /* cosine and sine direction components.  I think this order */
      /* is necessary because 0 direction is vertical and positive */
      /* direction is clockwise.                                   */
      theta = Math.Atan2(sinpart, cospart);

      /* Atan2 returns theta on range [-PI..PI].  Adjust theta so that */
      /* it is on the range [0..2PI].                                  */
      pi2 = 2 * M_PI;
      theta += pi2;
      theta = theta % pi2;

      /* Pi_factor sets the period of the trig functions to NDIRS units in x. */
      /* For example, if NDIRS==16, then pi_factor = 2(PI/16) = .3926...      */
      /* Dividing theta (in radians) by this factor ((1/pi_factor)==2.546...) */
      /* will produce directions on the range [0..NDIRS].                     */
      pi_factor = pi2 / (double)dir2rad.ndirs; /* 2(M_PI/ndirs) */

      /* Round off the direction and return it as an average direction */
      /* for the neighborhood.                                         */
      avr = theta / pi_factor;
      /* Need to truncate precision so that answers are consistent */
      /* on different computer architectures when rounding doubles. */
      avr = trunc_dbl_precision(avr, TRUNC_SCALE);
      avrdir = Sround(avr);

      /* Really do need to map values > NDIRS back onto [0..NDIRS) range. */
      avrdir %= dir2rad.ndirs;
    }

    internal static void smooth_direction_map(ref int[,] direction_map, ref int[,] low_contrast_map,
            int mw, int mh, DIR2RAD dir2rad)
    {
      int avrdir = 0, nvalid = 0;
      double dir_strength = 0;

      /* Foreach block in maps ... */
      for (int my = 0; my < mh; my++)
      {
        for (int mx = 0; mx < mw; mx++)
        {
          /* If the current block does NOT have LOW CONTRAST ... */
          if (low_contrast_map[mx, my] == 0)
          {
            /* Compute average direction from neighbors, returning the */
            /* number of valid neighbors used in the computation, and  */
            /* the "strength" of the average direction.                */
            average_8nbr_dir(ref avrdir, ref dir_strength, ref nvalid, direction_map, mx, my, mw, mh, dir2rad);

            /* If average direction strength is strong enough */
            /*    (Ex. thresh==0.2)...                        */
            if (dir_strength >= lfsparms.dir_strength_min)
            {
              /* If Direction Map direction is valid ... */
              if (direction_map[mx, my] != INVALID_DIR)
              {
                /* Conduct valid neighbor test (Ex. thresh==3)... */
                if (nvalid >= lfsparms.rmv_valid_nbr_min)
                {
                  /* Reassign valid direction with average direction. */
                  direction_map[mx, my] = avrdir;
                }
              }
              /* Otherwise direction is invalid ... */
              else
              {
                /* Even if DIRECTION_MAP value is invalid, if number of */
                /* valid neighbors is big enough (Ex. thresh==7)...     */
                if (nvalid >= lfsparms.smth_valid_nbr_min)
                {
                  /* Assign invalid direction with average direction. */
                  direction_map[mx, my] = avrdir;
                }
              }
            }
          }
          /* Otherwise, block has LOW CONTRAST, so keep INVALID direction. */
        }
      }
    }

    internal static int interpolate_direction_map(ref int[,] direction_map, ref int[,] low_contrast_map, int mw, int mh)
    {
      int new_dir = 0;
      int n_dir = 0, e_dir = 0, s_dir = 0, w_dir = 0;
      int n_dist = 0, e_dist = 0, s_dist = 0, w_dist = 0, total_dist = 0;
      int n_found = 0, e_found = 0, s_found = 0, w_found = 0, total_found = 0;
      int n_delta = 0, e_delta = 0, s_delta = 0, w_delta = 0, total_delta = 0;
      int nbr_x = 0, nbr_y = 0;
      double avr_dir = 0;

      int[,] omap = new int[mw, mh];

      /* Set pointers to the first block in the maps. */
      //  dptr = direction_map;
      //  cptr = low_contrast_map;
      //   optr = omap;

      /* Foreach block in the maps ... */
      for (int y = 0; y < mh; y++)
      {
        for (int x = 0; x < mw; x++)
        {
          /* If image block is NOT LOW CONTRAST and has INVALID direction ... */
          if ((low_contrast_map[x, y] == 0) && (direction_map[x, y] == INVALID_DIR))
          {
            /* Set neighbor accumulators to 0. */
            total_found = 0;
            total_dist = 0;

            /* Find north neighbor. */
            n_found = find_valid_block(ref n_dir, ref nbr_x, ref nbr_y,
                ref direction_map, ref low_contrast_map, x, y, mw, mh, 0, -1);

            if (n_found == FOUND)
            {
              /* Compute north distance. */
              n_dist = y - nbr_y;
              /* Accumulate neighbor distance. */
              total_dist += n_dist;
              /* Bump number of neighbors found. */
              total_found++;
            }

            /* Find east neighbor. */
            e_found = find_valid_block(ref e_dir, ref nbr_x, ref nbr_y,
                ref direction_map, ref low_contrast_map, x, y, mw, mh, 1, 0);

            if (e_found == FOUND)
            {
              /* Compute east distance. */
              e_dist = nbr_x - x;
              /* Accumulate neighbor distance. */
              total_dist += e_dist;
              /* Bump number of neighbors found. */
              total_found++;
            }

            /* Find south neighbor. */
            s_found = find_valid_block(ref s_dir, ref nbr_x, ref nbr_y,
                ref direction_map, ref low_contrast_map, x, y, mw, mh, 0, 1);

            if (s_found == FOUND)
            {
              /* Compute south distance. */
              s_dist = nbr_y - y;
              /* Accumulate neighbor distance. */
              total_dist += s_dist;
              /* Bump number of neighbors found. */
              total_found++;
            }

            /* Find west neighbor. */
            w_found = find_valid_block(ref w_dir, ref nbr_x, ref nbr_y,
                ref direction_map, ref low_contrast_map, x, y, mw, mh, -1, 0);

            if (w_found == FOUND)
            {
              /* Compute west distance. */
              w_dist = x - nbr_x;
              /* Accumulate neighbor distance. */
              total_dist += w_dist;
              /* Bump number of neighbors found. */
              total_found++;
            }

            /* If a sufficient number of neighbors found (Ex. 2) ... */
            if (total_found >= lfsparms.min_interpolate_nbrs)
            {
              /* Accumulate weighted sum of neighboring directions     */
              /* inversely related to the distance from current block. */
              total_delta = 0;
              /* If neighbor found to the north ... */
              if (n_found != 0)
              {
                n_delta = total_dist - n_dist;
                total_delta += n_delta;
              }
              /* If neighbor found to the east ... */
              if (e_found != 0)
              {
                e_delta = total_dist - e_dist;
                total_delta += e_delta;
              }
              /* If neighbor found to the south ... */
              if (s_found != 0)
              {
                s_delta = total_dist - s_dist;
                total_delta += s_delta;
              }
              /* If neighbor found to the west ... */
              if (w_found != 0)
              {
                w_delta = total_dist - w_dist;
                total_delta += w_delta;
              }

              avr_dir = 0.0;

              if (n_found != 0)
              {
                avr_dir += (n_dir * (n_delta / (double)total_delta));
              }
              if (e_found != 0)
              {
                avr_dir += (e_dir * (e_delta / (double)total_delta));
              }
              if (s_found != 0)
              {
                avr_dir += (s_dir * (s_delta / (double)total_delta));
              }
              if (w_found != 0)
              {
                avr_dir += (w_dir * (w_delta / (double)total_delta));
              }

              /* Need to truncate precision so that answers are consistent  */
              /* on different computer architectures when rounding doubles. */
              avr_dir = trunc_dbl_precision(avr_dir, TRUNC_SCALE);

              /* Assign interpolated direction to output Direction Map. */
              new_dir = Sround(avr_dir);
              omap[x, y] = new_dir;
            }
            else
            {
              /* Otherwise, the direction remains INVALID. */
              omap[x, y] = direction_map[x, y];
            }
          }
          else
          {
            /* Otherwise, assign the current direction to the output block. */
            omap[x, y] = direction_map[x, y];
          }
        }
      }

      /* Copy the interpolated directions into the input map. */

      for (int y = 0; y < mh; y++)
      {
        for (int x = 0; x < mw; x++)
        {
          direction_map[x, y] = omap[x, y];
        }
      }

      /* Return normally. */
      return (0);
    }

    internal static int[,] gen_high_curve_map(ref int[,] direction_map, int mw, int mh)
    {
      //  int mapsize = mw * mh;
      int[,] high_curve_map = new int[mw, mh];
      int hptrX = 0, hptrY = 0, dptrX = 0, dptrY = 0;
      int bx, by;
      int nvalid, cmeasure, vmeasure;


      for (by = 0; by < mh; by++)
      {
        for (bx = 0; bx < mw; bx++)
        {
          // хотя это и так верно...
          high_curve_map[bx, by] = 0;
        }
      }

      /* Foreach row in maps ... */
      for (by = 0; by < mh; by++)
      {
        /* Foreach column in maps ... */
        for (bx = 0; bx < mw; bx++)
        {

          /* Count number of valid neighbors around current block ... */
          nvalid = num_valid_8nbrs(direction_map, bx, by, mw, mh);

          /* If valid neighbors exist ... */
          if (nvalid > 0)
          {
            /* If current block's direction is INVALID ... */
            if (direction_map[dptrX, dptrY] == INVALID_DIR)
            {
              /* If a sufficient number of VALID neighbors exists ... */
              if (nvalid >= lfsparms.vort_valid_nbr_min)
              {
                /* Measure vorticity of neighbors. */
                vmeasure = vorticity(direction_map, bx, by, mw, mh,
                    lfsparms.num_directions);
                /* If vorticity is sufficiently high ... */
                if (vmeasure >= lfsparms.highcurv_vorticity_min)
                {
                  /* Flag block as HIGH CURVATURE. */
                  high_curve_map[hptrX, hptrY] = TRUE;
                }
              }
            }
            /* Otherwise block has valid direction ... */
            else
            {
              /* Measure curvature around the valid block. */
              cmeasure = curvature(direction_map, bx, by, mw, mh,
                                   lfsparms.num_directions);
              /* If curvature is sufficiently high ... */
              if (cmeasure >= lfsparms.highcurv_curvature_min)
                high_curve_map[hptrX, hptrY] = TRUE;
            }
          } /* Else (nvalid <= 0) */

          /* Bump pointers to next block in maps. */

          IncrementPointer(ref dptrX, ref dptrY, mw, mh, 1);
          IncrementPointer(ref hptrX, ref hptrY, mw, mh, 1);

          // Раньше было это. Но это ерунда. Теперь замена на 2 строчки выше.
          //if (dptrY >= mh - 1)
          //{
          //  dptrX++;
          //  dptrY = 0;
          //}
          //else
          //{
          //  dptrY++;
          //}

          //if (hptrY >= mh - 1)
          //{
          //  hptrX++;
          //  hptrY = 0;
          //}
          //else
          //{
          //  hptrY++;
          //}

        } /* bx */
      } /* by */

      /* Return normally. */
      return high_curve_map;
    }

    internal static int num_valid_8nbrs(int[,] imap, int mx, int my, int mw, int mh)
    {
      int e_ind, w_ind, n_ind, s_ind;
      int nvalid;

      /* Initialize VALID IMAP counter to zero. */
      nvalid = 0;

      /* Compute neighbor coordinates to current IMAP direction */
      e_ind = mx + 1;  /* East index */
      w_ind = mx - 1;  /* West index */
      n_ind = my - 1;  /* North index */
      s_ind = my + 1;  /* South index */

      /* 1. Test NW IMAP value.  */
      /* If neighbor indices are within IMAP boundaries and it is VALID ... */
      if ((w_ind >= 0) && (n_ind >= 0) && (imap[w_ind, n_ind] >= 0))
        /* Bump VALID counter. */
        nvalid++;

      /* 2. Test N IMAP value.  */
      if ((n_ind >= 0) && (imap[mx, n_ind] >= 0))
        nvalid++;

      /* 3. Test NE IMAP value. */
      if ((n_ind >= 0) && (e_ind < mw) && (imap[e_ind, n_ind] >= 0))
        nvalid++;

      /* 4. Test E IMAP value. */
      if ((e_ind < mw) && (imap[e_ind, my] >= 0))
        nvalid++;

      /* 5. Test SE IMAP value. */
      if ((e_ind < mw) && (s_ind < mh) && (imap[e_ind, s_ind] >= 0))
        nvalid++;

      /* 6. Test S IMAP value. */
      if ((s_ind < mh) && (imap[mx, s_ind] >= 0))
        nvalid++;

      /* 7. Test SW IMAP value. */
      if ((w_ind >= 0) && (s_ind < mh) && (imap[w_ind, s_ind] >= 0))
        nvalid++;

      /* 8. Test W IMAP value. */
      if ((w_ind >= 0) && (imap[w_ind, my] >= 0))
        nvalid++;

      /* Return number of neighbors with VALID IMAP values. */
      return nvalid;
    }

    internal static int vorticity(int[,] imap, int mx, int my, int mw, int mh, int ndirs)
    {
      int e_ind, w_ind, n_ind, s_ind;
      int nw_val, n_val, ne_val, e_val, se_val, s_val, sw_val, w_val;
      int vmeasure;

      /* Compute neighbor coordinates to current IMAP direction */
      e_ind = mx + 1; /* East index */
      w_ind = mx - 1; /* West index */
      n_ind = my - 1; /* North index */
      s_ind = my + 1; /* South index */

      /* 1. Get NW IMAP value.  */
      /* If neighbor indices are within IMAP boundaries ... */
      if ((w_ind >= 0) && (n_ind >= 0))
      {
        /* Set neighbor value to IMAP value. */
        nw_val = imap[w_ind, n_ind];
      }
      else
      {
        /* Otherwise, set the neighbor value to INVALID. */
        nw_val = INVALID_DIR;
      }

      /* 2. Get N IMAP value.  */
      if (n_ind >= 0)
      {
        n_val = imap[mx, n_ind];
      }
      else
      {
        n_val = INVALID_DIR;
      }

      /* 3. Get NE IMAP value. */
      if ((n_ind >= 0) && (e_ind < mw))
        ne_val = imap[e_ind, n_ind];
      else
        ne_val = INVALID_DIR;

      /* 4. Get E IMAP value. */
      if (e_ind < mw)
        e_val = imap[e_ind, my];
      else
        e_val = INVALID_DIR;

      /* 5. Get SE IMAP value. */
      if ((e_ind < mw) && (s_ind < mh))
        se_val = imap[e_ind, s_ind];
      else
        se_val = INVALID_DIR;

      /* 6. Get S IMAP value. */
      if (s_ind < mh)
        s_val = imap[mx, s_ind];
      else
        s_val = INVALID_DIR;

      /* 7. Get SW IMAP value. */
      if ((w_ind >= 0) && (s_ind < mh))
        sw_val = imap[w_ind, s_ind];
      else
        sw_val = INVALID_DIR;

      /* 8. Get W IMAP value. */
      if (w_ind >= 0)
        w_val = imap[w_ind, my];
      else
        w_val = INVALID_DIR;

      /* Now that we have all IMAP neighbors, accumulate vorticity between */
      /* the neighboring directions.                                       */

      /* Initialize vorticity accumulator to zero. */
      vmeasure = 0;

      /* 1. NW & N */
      accum_nbr_vorticity(ref vmeasure, nw_val, n_val, ndirs);

      /* 2. N & NE */
      accum_nbr_vorticity(ref vmeasure, n_val, ne_val, ndirs);

      /* 3. NE & E */
      accum_nbr_vorticity(ref vmeasure, ne_val, e_val, ndirs);

      /* 4. E & SE */
      accum_nbr_vorticity(ref vmeasure, e_val, se_val, ndirs);

      /* 5. SE & S */
      accum_nbr_vorticity(ref vmeasure, se_val, s_val, ndirs);

      /* 6. S & SW */
      accum_nbr_vorticity(ref vmeasure, s_val, sw_val, ndirs);

      /* 7. SW & W */
      accum_nbr_vorticity(ref vmeasure, sw_val, w_val, ndirs);

      /* 8. W & NW */
      accum_nbr_vorticity(ref vmeasure, w_val, nw_val, ndirs);

      /* Return the accumulated vorticity measure. */
      return (vmeasure);
    }

    internal static void accum_nbr_vorticity(ref int vmeasure, int dir1, int dir2, int ndirs)
    {
      int dist;

      /* Measure difference in direction between a pair of neighboring */
      /* directions.                                                   */
      /* If both neighbors are not equal and both are VALID ... */
      if ((dir1 != dir2) && (dir1 >= 0) && (dir2 >= 0))
      {
        /* Measure the clockwise distance from the first to the second */
        /* directions.                                                 */
        dist = dir2 - dir1;
        /* If dist is negative, then clockwise distance must wrap around */
        /* the high end of the direction range. For example:             */
        /*              dir1 = 8                                         */
        /*              dir2 = 3                                         */
        /*       and   ndirs = 16                                        */
        /*             3 - 8 = -5                                        */
        /*        so  16 - 5 = 11  (the clockwise distance from 8 to 3)  */
        if (dist < 0)
        {
          dist += ndirs;
        }
        /* If the change in clockwise direction is larger than 90 degrees as */
        /* in total the total number of directions covers 180 degrees.       */
        if (dist > (ndirs >> 1))
        {
          /* Decrement the vorticity measure. */
          vmeasure--;
        }
        else
        {
          /* Otherwise, bump the vorticity measure. */
          vmeasure++;
        }
      }
      /* Otherwise both directions are either equal or  */
      /* one or both directions are INVALID, so ignore. */
    }

    /*************************************************************************
   **************************************************************************
   #cat: curvature - Measures the largest change in direction between the
   #cat:             current IMAP direction and its immediate neighbors.

      Input:
         imap  - 2D vector of ridge flow directions
         mx    - horizontal coord of current IMAP block
         my    - vertical coord of current IMAP block
         mw    - width (in blocks) of the IMAP
         mh    - height (in blocks) of the IMAP
         ndirs - number of possible directions in the IMAP
      Return Code:
         Non-negative - maximum change in direction found (curvature)
         Negative     - No valid neighbor found to measure change in direction
   **************************************************************************/
    internal static int curvature(int[,] imap, int mx, int my, int mw, int mh, int ndirs)
    {
      int iptr;
      int e_ind, w_ind, n_ind, s_ind;
      int nw_val, n_val, ne_val, e_val, se_val, s_val, sw_val, w_val;
      int cmeasure, dist;

      /* Compute neighbor coordinates to current IMAP direction */
      e_ind = mx + 1;  /* East index */
      w_ind = mx - 1;  /* West index */
      n_ind = my - 1;  /* North index */
      s_ind = my + 1;  /* South index */

      /* 1. Get NW IMAP value.  */
      /* If neighbor indices are within IMAP boundaries ... */
      if ((w_ind >= 0) && (n_ind >= 0))
        /* Set neighbor value to IMAP value. */
        nw_val = imap[w_ind, n_ind];
      else
        /* Otherwise, set the neighbor value to INVALID. */
        nw_val = INVALID_DIR;

      /* 2. Get N IMAP value.  */
      if (n_ind >= 0)
        n_val = imap[mx, n_ind];
      else
        n_val = INVALID_DIR;

      /* 3. Get NE IMAP value. */
      if ((n_ind >= 0) && (e_ind < mw))
        ne_val = imap[e_ind, n_ind];
      else
        ne_val = INVALID_DIR;

      /* 4. Get E IMAP value. */
      if (e_ind < mw)
        e_val = imap[e_ind, my];
      else
        e_val = INVALID_DIR;

      /* 5. Get SE IMAP value. */
      if ((e_ind < mw) && (s_ind < mh))
        se_val = imap[e_ind, s_ind];
      else
        se_val = INVALID_DIR;

      /* 6. Get S IMAP value. */
      if (s_ind < mh)
        s_val = imap[mx, s_ind];
      else
        s_val = INVALID_DIR;

      /* 7. Get SW IMAP value. */
      if ((w_ind >= 0) && (s_ind < mh))
        sw_val = imap[w_ind, s_ind];
      else
        sw_val = INVALID_DIR;

      /* 8. Get W IMAP value. */
      if (w_ind >= 0)
        w_val = imap[w_ind, my];
      else
        w_val = INVALID_DIR;

      /* Now that we have all IMAP neighbors, determine largest change in */
      /* direction from current block to each of its 8 VALID neighbors.   */

      /* Initialize pointer to current IMAP value. */
      //iptr = my * mw + mx;

      /* Initialize curvature measure to negative as closest_dir_dist() */
      /* always returns -1=INVALID or a positive value.                 */
      cmeasure = -1;

      /* 1. With NW */
      /* Compute closest distance between neighboring directions. */
      dist = closest_dir_dist(imap[mx, my], nw_val, ndirs);
      /* Keep track of maximum. */
      if (dist > cmeasure)
        cmeasure = dist;

      /* 2. With N */
      dist = closest_dir_dist(imap[mx, my], n_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* 3. With NE */
      dist = closest_dir_dist(imap[mx, my], ne_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* 4. With E */
      dist = closest_dir_dist(imap[mx, my], e_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* 5. With SE */
      dist = closest_dir_dist(imap[mx, my], se_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* 6. With S */
      dist = closest_dir_dist(imap[mx, my], s_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* 7. With SW */
      dist = closest_dir_dist(imap[mx, my], sw_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* 8. With W */
      dist = closest_dir_dist(imap[mx, my], w_val, ndirs);
      if (dist > cmeasure)
        cmeasure = dist;

      /* Return maximum difference between current block's IMAP direction */
      /* and the rest of its VALID neighbors.                             */
      return (cmeasure);
    }
  }
}
