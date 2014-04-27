using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
    public static partial class Nfiq
    {
        internal static void bubble_sort_double_dec_2(double[] ranks, ref int[] items, int len)
        {
            int done = 0, n = len, titem;
            double trank;

            while (done == 0)
            {
                done = 1;
                // для каждого rank из списка...
                // p - current rank, i - next rank
                for (int i = 1, p = 0; i < n; i++, p++)
                {
                    /* If previous rank is < current rank ... */
                    if (ranks[p] < ranks[i])
                    {
                        /* Swap ranks */
                        trank = ranks[i];
                        ranks[i] = ranks[p];
                        ranks[p] = trank;
                        /* Swap corresponding items */
                        titem = items[i];
                        items[i] = items[p];
                        items[p] = titem;
                        // Изменения выполнены, меняем флаг (выходим из while)
                        done = 0;
                    }
                }
                n--;
            }
        }
    }
}
