using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
    public static partial class Nfiq
    {
        internal static int get_max_padding_V2(int map_windowsize, int map_windowoffset,
            int dirbin_grid_w, int dirbin_grid_h)
        {
            double diag = Math.Sqrt((double)(2.0 * map_windowsize * map_windowsize));
            double pad = (diag - map_windowsize) / (double)2.0;

            pad = trunc_dbl_precision(pad, TRUNC_SCALE);

            int dft_pad = Sround(pad) + map_windowoffset;

            diag = Math.Sqrt((double)((dirbin_grid_w * dirbin_grid_w) + (dirbin_grid_h * dirbin_grid_h)));
            pad = (diag - 1) / (double)2.0;
            pad = trunc_dbl_precision(pad, TRUNC_SCALE);

            return Max(dft_pad, Sround(pad));
        }

        internal static DIR2RAD init_dir2rad(int ndirs)
        {
            DIR2RAD dir2rad = new DIR2RAD();
            double theta, pi_factor;
            double cs, sn;

            dir2rad.ndirs = ndirs;
            pi_factor = 2.0 * M_PI / (double)ndirs;
            dir2rad.cos = new List<double>();
            dir2rad.sin = new List<double>();

            for (int i = 0; i < ndirs; ++i)
            {
                theta = (double)(i * pi_factor);
                cs = Math.Cos(theta);
                sn = Math.Sin(theta);
                cs = trunc_dbl_precision(cs, TRUNC_SCALE);
                sn = trunc_dbl_precision(sn, TRUNC_SCALE);
                dir2rad.cos.Add(cs);
                dir2rad.sin.Add(sn);
            }

            return dir2rad;
        }

        internal static DFTWAVES init_dftwaves(List<double> dft_coefs, int nwaves, int blocksize)
        {
            DFTWAVES dftwaves = new DFTWAVES();
            DFTWAVE dftwaveResult = new DFTWAVE();
            double pi_factor, freq, x;

            dftwaves.nwaves = nwaves;
            dftwaves.wavelen = blocksize;
            dftwaves.waves = new List<DFTWAVE>(); //count = nwaves
            pi_factor = 2.0 * M_PI / (double)blocksize;

            for (int i = 0; i < nwaves; ++i)
            {
                dftwaveResult = new DFTWAVE();
                dftwaveResult.sin = new List<double>(); // count = blocksize
                dftwaveResult.cos = new List<double>(); // count = blocksize
                freq = pi_factor * dft_coefs[i];

                for (int j = 0; j < blocksize; ++j)
                {
                    x = freq * j;
                    dftwaveResult.cos.Add(Math.Cos(x));
                    dftwaveResult.sin.Add(Math.Sin(x));
                }

                dftwaves.waves.Add(dftwaveResult);
            }

            return dftwaves;
        }

        internal static ROTGRIDS init_rotgrids(int iw, int ih, int ipad, double start_dir_angle,
           int ndirs, int grid_w, int grid_h, int relative2)
        {
            ROTGRIDS rotgrids;
            double pi_offset, pi_incr;
            int dir, pw, grid_pad, min_dim;
            List<int> grid;
            double theta, cs, sn, cx, cy;
            double fxm, fym, fx, fy;
            int ixt, iyt;
            double pad;
            double diag = Math.Sqrt(grid_w * grid_w + grid_h * grid_h);

            rotgrids.ngrids = ndirs;
            rotgrids.grid_w = grid_w;
            rotgrids.grid_h = grid_h;
            rotgrids.start_angle = start_dir_angle;
            rotgrids.relative2 = relative2;

            switch (relative2)
            {
                case RELATIVE2CENTER:
                    {
                        pad = (diag - 1) / (double)2.0;
                        pad = trunc_dbl_precision(pad, TRUNC_SCALE);
                        grid_pad = Sround(pad);
                        break;
                    }
                case RELATIVE2ORIGIN:
                    {
                        min_dim = Min(grid_w, grid_h);
                        pad = (diag - min_dim) / (double)2.0;
                        pad = trunc_dbl_precision(pad, TRUNC_SCALE);
                        grid_pad = Sround(pad);
                        break;
                    }
                default:
                    {
                        throw new Exception(String.Format("ERROR : init_rotgrids : Illegal relative flag : {0}\n", relative2));
                    }
            }

            if (ipad == UNDEFINED)
            {
                rotgrids.pad = grid_pad;
            }
            else
            {
                if (ipad < grid_pad)
                {
                    throw new Exception("ERROR : init_rotgrids : Pad passed is too small\n");
                }

                rotgrids.pad = ipad;
            }

            //grid_size = grid_w * grid_h;
            pw = iw + (rotgrids.pad << 1);
            cx = (grid_w - 1) / 2.0;
            cy = (grid_h - 1) / 2.0;

            // < <i,j>, value >; count = ndirs * grid_size
            rotgrids.grids = new Dictionary<int, List<int>>();
            pi_offset = start_dir_angle;
            pi_incr = M_PI / ndirs; /* if ndirs == 16, incr = 11.25 degrees */

            for (dir = 0, theta = pi_offset;
                dir < ndirs;
                dir++, theta += pi_incr)
            {
                cs = Math.Cos(theta);
                sn = Math.Sin(theta);
                grid = new List<int>();

                for (int iy = 0; iy < grid_h; ++iy)
                {
                    fxm = -1.0 * ((iy - cy) * sn);
                    fym = ((iy - cy) * cs);

                    if (relative2 == RELATIVE2ORIGIN)
                    {
                        fxm += cx;
                        fym += cy;
                    }

                    for (int ix = 0; ix < grid_w; ++ix)
                    {
                        fx = fxm + ((ix - cx) * cs);
                        fy = fym + ((ix - cx) * sn);
                        fx = trunc_dbl_precision(fx, TRUNC_SCALE);
                        fy = trunc_dbl_precision(fy, TRUNC_SCALE);
                        ixt = Sround(fx);
                        iyt = Sround(fy);
                        grid.Add(ixt + (iyt * pw));

                    } /* ix */
                } /* iy */

                rotgrids.grids.Add(dir, grid);
            } /* dir */

            return rotgrids;
        }

    }
}
