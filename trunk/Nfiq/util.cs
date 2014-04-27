using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
    public static partial class Nfiq
    {
        /*************************************************************************
     **************************************************************************
     #cat: closest_dir_dist - Takes to integer IMAP directions and determines the
     #cat:                    closest distance between them accounting for
     #cat:                    wrap-around either at the beginning or ending of
     #cat:                    the range of directions.

        Input:
           dir1  - integer value of the first direction
           dir2  - integer value of the second direction
           ndirs - the number of possible directions
        Return Code:
           Non-negative - distance between the 2 directions
     **************************************************************************/
        internal static int closest_dir_dist(int dir1, int dir2, int ndirs)
        {
            int d1, d2, dist;

            /* Initialize distance to -1 = INVALID. */
            dist = INVALID_DIR;

            /* Measure shortest distance between to directions. */
            /* If both neighbors are VALID ... */
            if ((dir1 >= 0) && (dir2 >= 0))
            {
                /* Compute inner and outer distances to account for distances */
                /* that wrap around the end of the range of directions, which */
                /* may in fact be closer.                                     */
                d1 = Math.Abs(dir2 - dir1);
                d2 = ndirs - d1;
                dist = Min(d1, d2);
            }
            /* Otherwise one or both directions are INVALID, so ignore */
            /* and return INVALID. */

            /* Return determined closest distance. */
            return (dist);
        }
    }
}
