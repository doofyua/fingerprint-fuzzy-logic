using System.Drawing;

namespace Nfiq
{
    public static partial class Nfiq
    {
        internal static int[,] pad_uchar_image(ref int ow, ref int oh,
              Bitmap idata, int iw, int ih, int pad, int pad_value)
        {
            int pad2 = pad << 1; // что тут происходит?
            ow = iw + pad2;
            oh = ih + pad2;

            int[,] odata = new int[ow, oh];

            for (int j = 0; j < oh; j++)
            {
                for (int i = 0; i < ow; i++)
                {
                    odata[i, j] = pad_value;
                }
            }

            for (int j = 0; j < ih; j++)
            {
                for (int i = 0; i < iw; i++)
                {
                    odata[i + pad, j + pad] = idata.GetPixel(i, j).R;
                }
            }

            return odata;
        }

        internal static void fill_holes(ref int[,] bdata, int iw, int ih)
        {
            int lptrX, lptrY, mptrX, mptrY, rptrX, rptrY, tptrX, tptrY, bptrX, bptrY;

            /* 1. Fill 1-pixel wide holes in horizontal runs first ... */
            int sptrX = 1, sptrY = 0; // bdata

            /* Foreach row in image ... */
            for (int iy = 0; iy < ih; iy++)
            {
                /* Initialize pointers to start of next line ... */
                lptrX = sptrX - 1; lptrY = sptrY; /* Left pixel   */
                mptrX = sptrX; mptrY = sptrY;  /* Middle pixel */
                rptrX = sptrX + 1; rptrY = sptrY;  /* Right pixel  */

                /* Foreach column in image (less far left and right pixels) ... */
                for (int ix = 1; ix < iw - 1; ix++)
                {
                    /* Do we have a horizontal hole of length 1? */
                    if ((bdata[lptrX, lptrY] != bdata[mptrX, mptrY]) && (bdata[lptrX, lptrY] == bdata[rptrX, rptrY]))
                    {
                        /* If so, then fill it. */
                        mptrX = lptrX; mptrY = lptrY;
                        /* Bump passed right pixel because we know it will not */
                        /* be a hole.                                          */
                        IncrementPointer(ref lptrX, ref lptrY, iw, ih, 2);
                        IncrementPointer(ref mptrX, ref mptrY, iw, ih, 2);
                        IncrementPointer(ref rptrX, ref rptrY, iw, ih, 2);

                        /* We bump ix once here and then the FOR bumps it again. */
                        ix++;
                    }
                    else
                    {
                        /* Otherwise, bump to the next pixel to the right. */
                        IncrementPointer(ref lptrX, ref lptrY, iw, ih, 1);
                        IncrementPointer(ref mptrX, ref mptrY, iw, ih, 1);
                        IncrementPointer(ref rptrX, ref rptrY, iw, ih, 1);
                    }
                }
                /* Bump to start of next row. */
                IncrementPointer(ref sptrX, ref sptrY, iw, ih, iw);
            }

            /* 2. Now, fill 1-pixel wide holes in vertical runs ... */
            int iw2 = iw << 1;
            /* Start processing column one row down from the top of the image. */
            sptrX = 0; sptrY = 1; // bdata

            /* Foreach column in image ... */
            for (int ix = 0; ix < iw; ix++)
            {
                /* Initialize pointers to start of next column ... */
                tptrX = sptrX; tptrY = sptrY - 1; /* Top pixel   */
                mptrX = sptrX; mptrY = sptrY;  /* Middle pixel */
                bptrX = sptrX; bptrY = sptrY + 1;  /* Bottom pixel  */

                /* Foreach row in image (less top and bottom row) ... */
                for (int iy = 1; iy < ih - 1; iy++)
                {
                    /* Do we have a vertical hole of length 1? */
                    if ((bdata[tptrX, tptrY] != bdata[mptrX, mptrY]) && (bdata[tptrX, tptrY] == bdata[bptrX, bptrY]))
                    {
                        /* If so, then fill it. */
                        mptrX = tptrX; mptrY = tptrY;
                        /* Bump passed bottom pixel because we know it will not */
                        /* be a hole.                                           */
                        IncrementPointer(ref tptrX, ref tptrY, iw, ih, iw2);
                        IncrementPointer(ref mptrX, ref mptrY, iw, ih, iw2);
                        IncrementPointer(ref bptrX, ref bptrY, iw, ih, iw2);

                        /* We bump iy once here and then the FOR bumps it again. */
                        iy++;
                    }
                    else
                    {
                        /* Otherwise, bump to the next pixel below. */
                        IncrementPointer(ref tptrX, ref tptrY, iw, ih, iw);
                        IncrementPointer(ref mptrX, ref mptrY, iw, ih, iw);
                        IncrementPointer(ref bptrX, ref bptrY, iw, ih, iw);
                    }
                }
                /* Bump to start of next column. */
                IncrementPointer(ref sptrX, ref sptrY, iw, ih, 1);
            }
        }
    }
}
