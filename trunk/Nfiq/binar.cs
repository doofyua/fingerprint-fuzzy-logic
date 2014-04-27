using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
    public static partial class Nfiq
    {
        internal static int[,] binarize_V2(int[,] pdata, int pw, int ph, int[,] direction_map, int mw, int mh, ROTGRIDS dirbingrids)
        {
            int[,] bdata = new int[pdata.GetLength(0), pdata.GetLength(1)];
            int bw = 0, bh = 0;

            /* 1. Binarize the padded input image using directional block info. */

            bdata = binarize_image_V2(ref bw, ref bh, pdata, pw, ph, direction_map, mw, mh, lfsparms.blocksize, dirbingrids);

            /* 2. Fill black and white holes in binary image. */
            /* LFS scans the binary image, filling holes, 3 times. */
            for (int i = 0; i < lfsparms.num_fill_holes; i++)
            {
                fill_holes(ref bdata, bw, bh);
            }

            return bdata;
        }

        internal static int[,] binarize_image_V2(ref int bw, ref int bh,
                         int[,] pdata, int pw, int ph, int[,] direction_map, int mw, int mh,
                          int blocksize, ROTGRIDS dirbingrids)
        {
            int bx, by, mapval;

            bw = pw - (dirbingrids.pad << 1);
            bh = ph - (dirbingrids.pad << 1);

            int[,] bdata = new int[bw, bh];
            int bptrX = 0, bptrY = 0; //bdata
            int sptrX = dirbingrids.pad, sptrY = dirbingrids.pad; // pdata
            int pptrX = 0, pptrY = 0; //pdata

            for (int iy = 0; iy < bh; iy++)
            {
                /* Set pixel pointer to start of next row in grid. */
                pptrX = sptrX;
                pptrY = sptrY;

                for (int ix = 0; ix < bw; ix++)
                {
                    /* Compute which block the current pixel is in. */
                    bx = ix / blocksize;
                    by = iy / blocksize;
                    /* Get corresponding value in Direction Map. */
                    mapval = direction_map[bx, by];
                    /* If current block has has INVALID direction ... */
                    if (mapval == INVALID_DIR)
                    {
                        /* Set binary pixel to white (255). */
                        bdata[bptrX, bptrY] = WHITE_PIXEL;
                    }
                    /* Otherwise, if block has a valid direction ... */
                    else
                    {
                        /*if(mapval >= 0)*/
                        /* Use directional binarization based on block's direction. */
                        bdata[bptrX, bptrY] = dirbinarize(pdata, pptrX, pptrY, mapval, dirbingrids);
                    }

                    /* Bump input and output pixel pointers. */
                    // IncrementPointer(ref pptrX, ref pptrY, pdata.GetLength(0), pdata.GetLength(1), 1);
                    pptrX++;
                    IncrementPointer(ref bptrX, ref bptrY, bdata.GetLength(0), bdata.GetLength(1), 1);
                }
                /* Bump pointer to the next row in padded input image. */
                sptrY++;
            }

            return bdata;
        }

        internal static int dirbinarize(int[,] pdata, int pptrX, int pptrY, int idir, ROTGRIDS dirbingrids)
        {
            int gx, gy;
            int rsum, csum = 0;

            /* Assign nickname pointer. */
            List<int> grid = dirbingrids.grids[idir];

            /* Calculate center (0-oriented) row in grid. */
            double dcy = (dirbingrids.grid_h - 1) / (double)2.0;

            /* Need to truncate precision so that answers are consistent */
            /* on different computer architectures when rounding doubles. */
            dcy = trunc_dbl_precision(dcy, TRUNC_SCALE);

            int cy = Sround(dcy);

            /* Initialize grid's pixel offset index to zero. */
            int gi = 0;
            /* Initialize grid's pixel accumulator to zero */
            int gsum = 0;

            /* Foreach row in grid ... */
            for (gy = 0; gy < dirbingrids.grid_h; gy++)
            {
                /* Initialize row pixel sum to zero. */
                rsum = 0;
                /* Foreach column in grid ... */
                for (gx = 0; gx < dirbingrids.grid_w; gx++)
                {
                    /* Accumulate next pixel along rotated row in grid. */

                    //rsum += *(pptr+grid[gi]);
                    rsum += GetValueOfArray(pdata, pptrX, pptrY, grid[gi]);

                    /* Bump grid's pixel offset index. */
                    gi++;
                }
                /* Accumulate row sum into grid pixel sum. */
                gsum += rsum;
                /* If current row is center row, then save row sum separately. */
                if (gy == cy)
                {
                    csum = rsum;
                }
            }

            /* If the center row sum treated as an average is less than the */
            /* total pixel sum in the rotated grid ...                      */
            return csum * dirbingrids.grid_h < gsum ? BLACK_PIXEL : WHITE_PIXEL;
        }
    }
}
