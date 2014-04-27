using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
    public static partial class Nfiq
    {
        /// <summary>
        /// Объединение всех карт в карту качества: каждый блок имеет качество 1..5
        /// Сделано благодаря некоторому множеству эвристик
        /// 
        /// Качество 0(не использовать)..4(хорошо) (они называют это градачиями A..F)
        /// 
        /// 0/F: low contrast OR no direction
        /// 1/D: low flow OR high curve
        ///     (with low contrast OR no direction neighbor)
        ///     (or within NEIGHBOR_DELTA of edge)
        /// 2/C: low flow OR high curve
        ///     (or good quality with low contrast/no direction neighbor)
        /// 3/B: good quality with low flow / high curve neighbor
        /// 4/A: good quality (none of the above)
        ///
        /// Generally, the features in A/B quality are useful, the C/D quality ones are not.
        /// </summary>
        /// <param name="direction_map">карта направлений</param>
        /// <param name="low_contrast_map">карта контраста</param>
        /// <param name="low_flow_map">карта потоков</param>
        /// <param name="high_curve_map">карта кривизны</param>
        /// <param name="map_w">ширина в блоках</param>
        /// <param name="map_h">высота в блоках</param>
        /// <returns>карта качества</returns>
        internal static int[,] gen_quality_map(int[,] direction_map, int[,] low_contrast_map,
                    int[,] low_flow_map, int[,] high_curve_map, int map_w, int map_h)
        {
            int[,] quality_map = new int[map_w, map_h];
            int qual_offset;

            /* Foreach row of blocks in maps ... */
            for (int thisY = 0; thisY < map_h; thisY++)
            {
                /* Foreach block in current row ... */
                for (int thisX = 0; thisX < map_w; thisX++)
                {
                    /* If current block has low contrast or INVALID direction ... */
                    if (low_contrast_map[thisX, thisY] != 0 || direction_map[thisX, thisY] < 0)
                    {
                        /* Set block's quality to 0/F. */
                        quality_map[thisX, thisY] = 0;
                    }
                    else
                    {
                        /* Set baseline quality before looking at neighbors    */
                        /*     (will subtract QualOffset below)                */
                        /* If current block has low flow or high curvature ... */
                        if (low_flow_map[thisX, thisY] != 0 || high_curve_map[thisX, thisY] != 0)
                        {
                            /* Set block's quality initially to 3/B. */
                            quality_map[thisX, thisY] = 3; /* offset will be -1..-2 */
                        }
                        /* Otherwise, block is NOT low flow AND NOT high curvature... */
                        else
                        {
                            /* Set block's quality to 4/A. */
                            quality_map[thisX, thisY] = 4; /* offset will be 0..-2 */
                        }

                        /* If block within NEIGHBOR_DELTA of edge ... */
                        if (thisY < NEIGHBOR_DELTA || thisY > map_h - 1 - NEIGHBOR_DELTA ||
                            thisX < NEIGHBOR_DELTA || thisX > map_w - 1 - NEIGHBOR_DELTA)
                        {
                            /* Set block's quality to 1/E. */
                            quality_map[thisX, thisY] = 1;
                        }
                        /* Otherwise, test neighboring blocks ... */
                        else
                        {
                            /* Initialize quality adjustment to 0. */
                            qual_offset = 0;
                            /* Foreach row in neighborhood ... */
                            for (int compY = thisY - NEIGHBOR_DELTA;
                                compY <= thisY + NEIGHBOR_DELTA;
                                compY++)
                            {
                                /* Foreach block in neighborhood */
                                /*  (including current block)... */
                                for (int compX = thisX - NEIGHBOR_DELTA;
                                    compX <= thisX + NEIGHBOR_DELTA;
                                    compX++)
                                {
                                    /* If neighbor block (which might be itself) has */
                                    /* low contrast or INVALID direction .. */
                                    if (low_contrast_map[compX, compY] != 0 || direction_map[compX, compY] < 0)
                                    {
                                        /* Set quality adjustment to -2. */
                                        qual_offset = -2;
                                        /* Done with neighborhood row. */
                                        break;
                                    }
                                    /* Otherwise, if neighbor block (which might be */
                                    /* itself) has low flow or high curvature ... */
                                    if (low_flow_map[compX, compY] != 0 || high_curve_map[compX, compY] != 0)
                                    {
                                        /* Set quality to -1 if not already -2. */
                                        qual_offset = Min(qual_offset, -1);
                                    }
                                }
                            }
                            /* Decrement minutia quality by neighborhood adjustment. */
                            quality_map[thisX, thisY] += qual_offset;
                        }
                    }
                }
            }

            return quality_map;
        }
    }
}
