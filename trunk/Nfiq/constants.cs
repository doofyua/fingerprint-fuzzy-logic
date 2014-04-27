using System;
using System.Collections.Generic;

namespace Nfiq
{
    public static partial class Nfiq
    {
        #region Constants

        internal static int debug = 0;

        #region nfiq.h

        internal const int DEFAULT_PPI = 500;
        internal const int NFIQ_VCTRLEN = 11;
        internal const int NFIQ_NUM_CLASSES = 5;
        internal const int EMPTY_IMG = 1;
        internal const int EMPTY_IMG_QUAL = 5;
        internal const int TOO_FEW_MINUTIAE = 2;
        internal const int MIN_MINUTIAE = 5;
        internal const int MIN_MINUTIAE_QUAL = 5;

        #endregion

        #region defs.h

        internal const int True = 1;
        internal const int TRUE = 1;
        internal const int Yes = 1;
        internal const int False = 0;
        internal const int FALSE = 0;
        internal const int No = 0;
        internal const object Empty = null;
        internal const int None = -1;
        internal const int FOUND = 1;
        internal const int NOT_FOUND = 0;
        internal const int NOT_FOUND_NEG = -1;
        internal const char EOL = (char)0x1a;
        //internal const double DEG2RAD = (double)(57.29578); ////////////////////////////////////
        internal const int CHUNKS = 100;

        #endregion

        #region lfs.h

        #region Structures

        // Lookup tables for converting from integer directions to angles in radians.
        internal struct DIR2RAD
        {
            internal int ndirs;
            internal List<double> cos; // array ? size?
            internal List<double> sin; // array ? size?
        };

        // DFT wave form structure containing both cosine and sine components for a specific frequency.
        internal struct DFTWAVE
        {
            internal List<double> cos; // array ? size?
            internal List<double> sin; // array ? size?
        };

        // DFT wave forms structure containing all wave forms to be used in DFT analysis.
        internal struct DFTWAVES
        {
            internal int nwaves;
            internal int wavelen;
            internal List<DFTWAVE> waves; // 2-dimention array ? size?
        };

        /*  Rotated pixel offsets for a grid of specified dimensions
            rotated at a specified number of different orientations
            (directions).  This structure used by the DFT analysis
            when generating a Direction Map and also for conducting
            isotropic binarization.                                 */

        internal struct ROTGRIDS
        {
            internal int pad;
            internal int relative2;
            internal double start_angle;
            internal int ngrids;
            internal int grid_w;
            internal int grid_h;
            internal Dictionary<int, List<int>> grids; // 2-dimention array ? size?
        };

        internal struct MINUTIA
        {
            internal int x;
            internal int y;
            internal int ex;
            internal int ey;
            internal int direction;
            internal double reliability;
            internal int type;
            internal int appearing;
            internal int feature_id;
            internal List<int> nbrs; // array ? size?
            internal List<int> ridge_counts; // array ? size?
            internal int num_nbrs;
        };

        internal struct MINUTIAE
        {
            internal int alloc;
            internal int num;
            internal List<MINUTIA> list;
        };

        // Parameters used by LFS for setting thresholds and defining testing criterion.
        internal struct LFSPARMS
        {
            /* Image Controls */
            internal int pad_value;
            internal int join_line_radius;

            /* Map Controls */
            internal int blocksize; /* Pixel dimension image block.                 */
            internal int windowsize; /* Pixel dimension window surrounding block.    */
            internal int windowoffset; /* Offset in X & Y from block to window origin. */
            internal int num_directions;
            internal double start_dir_angle;
            internal int rmv_valid_nbr_min;
            internal double dir_strength_min;
            internal int dir_distance_max;
            internal int smth_valid_nbr_min;
            internal int vort_valid_nbr_min;
            internal int highcurv_vorticity_min;
            internal int highcurv_curvature_min;
            internal int min_interpolate_nbrs;
            internal int percentile_min_max;
            internal int min_contrast_delta;

            /* DFT Controls */
            internal int num_dft_waves;
            internal double powmax_min;
            internal double pownorm_min;
            internal double powmax_max;
            internal int fork_interval;
            internal double fork_pct_powmax;
            internal double fork_pct_pownorm;

            /* Binarization Controls */
            internal int dirbin_grid_w;
            internal int dirbin_grid_h;
            internal int isobin_grid_dim;
            internal int num_fill_holes;

            /* Minutiae Detection Controls */
            internal int max_minutia_delta;
            internal double max_high_curve_theta;
            internal int high_curve_half_contour;
            internal int min_loop_len;
            internal double min_loop_aspect_dist;
            internal double min_loop_aspect_ratio;

            /* Minutiae Link Controls */
            internal int link_table_dim;
            internal int max_link_dist;
            internal int min_theta_dist;
            internal int maxtrans;
            internal double score_theta_norm;
            internal double score_dist_norm;
            internal double score_dist_weight;
            internal double score_numerator;

            /* False Minutiae Removal Controls */
            internal int max_rmtest_dist;
            internal int max_hook_len;
            internal int max_half_loop;
            internal int trans_dir_pix;
            internal int small_loop_len;
            internal int side_half_contour;
            internal int inv_block_margin;
            internal int rm_valid_nbr_min;
            internal int max_overlap_dist;
            internal int max_overlap_join_dist;
            internal int malformation_steps_1;
            internal int malformation_steps_2;
            internal double min_malformation_ratio;
            internal int max_malformation_dist;
            internal int pores_trans_r;
            internal int pores_perp_steps;
            internal int pores_steps_fwd;
            internal int pores_steps_bwd;
            internal double pores_min_dist2;
            internal double pores_max_ratio;

            /* Ridge Counting Controls */
            internal int max_nbrs;
            internal int max_ridge_steps;

            public LFSPARMS(int i)
            {
                if (i == 1)
                {
                    /* Image Controls */
                    pad_value = PAD_VALUE;
                    join_line_radius = JOIN_LINE_RADIUS;

                    /* Map Controls */
                    blocksize = IMAP_BLOCKSIZE;
                    windowsize = UNUSED_INT; /* windowsize */
                    windowoffset = UNUSED_INT; /* windowoffset */
                    num_directions = NUM_DIRECTIONS;
                    start_dir_angle = START_DIR_ANGLE;
                    rmv_valid_nbr_min = RMV_VALID_NBR_MIN;
                    dir_strength_min = DIR_STRENGTH_MIN;
                    dir_distance_max = DIR_DISTANCE_MAX;
                    smth_valid_nbr_min = SMTH_VALID_NBR_MIN;
                    vort_valid_nbr_min = VORT_VALID_NBR_MIN;
                    highcurv_vorticity_min = HIGHCURV_VORTICITY_MIN;
                    highcurv_curvature_min = HIGHCURV_CURVATURE_MIN;
                    min_interpolate_nbrs = UNUSED_INT; /* min_interpolate_nbrs */
                    percentile_min_max = UNUSED_INT; /* percentile_min_max   */
                    min_contrast_delta = UNUSED_INT; /* min_contrast_delta   */

                    /* DFT Controls */
                    num_dft_waves = NUM_DFT_WAVES;
                    powmax_min = POWMAX_MIN;
                    pownorm_min = POWNORM_MIN;
                    powmax_max = POWMAX_MAX;
                    fork_interval = FORK_INTERVAL;
                    fork_pct_powmax = FORK_PCT_POWMAX;
                    fork_pct_pownorm = FORK_PCT_POWNORM;

                    /* Binarization Controls */
                    dirbin_grid_w = DIRBIN_GRID_W;
                    dirbin_grid_h = DIRBIN_GRID_H;
                    isobin_grid_dim = ISOBIN_GRID_DIM;
                    num_fill_holes = NUM_FILL_HOLES;

                    /* Minutiae Detection Controls */
                    max_minutia_delta = MAX_MINUTIA_DELTA;
                    max_high_curve_theta = MAX_HIGH_CURVE_THETA;
                    high_curve_half_contour = HIGH_CURVE_HALF_CONTOUR;
                    min_loop_len = MIN_LOOP_LEN;
                    min_loop_aspect_dist = MIN_LOOP_ASPECT_DIST;
                    min_loop_aspect_ratio = MIN_LOOP_ASPECT_RATIO;

                    /* Minutiae Link Controls */
                    link_table_dim = LINK_TABLE_DIM;
                    max_link_dist = MAX_LINK_DIST;
                    min_theta_dist = MIN_THETA_DIST;
                    maxtrans = MAXTRANS;
                    score_theta_norm = SCORE_THETA_NORM;
                    score_dist_norm = SCORE_DIST_NORM;
                    score_dist_weight = SCORE_DIST_WEIGHT;
                    score_numerator = SCORE_NUMERATOR;

                    /* False Minutiae Removal Controls */
                    max_rmtest_dist = MAX_RMTEST_DIST;
                    max_hook_len = MAX_HOOK_LEN;
                    max_half_loop = MAX_HALF_LOOP;
                    trans_dir_pix = TRANS_DIR_PIX;
                    small_loop_len = SMALL_LOOP_LEN;
                    side_half_contour = SIDE_HALF_CONTOUR;
                    inv_block_margin = INV_BLOCK_MARGIN;
                    rm_valid_nbr_min = RM_VALID_NBR_MIN;
                    max_overlap_dist = UNUSED_INT; /* max_overlap_dist       */
                    max_overlap_join_dist = UNUSED_INT; /* max_overlap_join_dist  */
                    malformation_steps_1 = UNUSED_INT; /* malformation_steps_1   */
                    malformation_steps_2 = UNUSED_INT; /* malformation_steps_2   */
                    min_malformation_ratio = UNUSED_DBL; /* min_malformation_ratio */
                    max_malformation_dist = UNUSED_INT; /* max_malformation_dist  */
                    pores_trans_r = PORES_TRANS_R;
                    pores_perp_steps = PORES_PERP_STEPS;
                    pores_steps_fwd = PORES_STEPS_FWD;
                    pores_steps_bwd = PORES_STEPS_BWD;
                    pores_min_dist2 = PORES_MIN_DIST2;
                    pores_max_ratio = PORES_MAX_RATIO;

                    /* Ridge Counting Controls */
                    max_nbrs = MAX_NBRS;
                    max_ridge_steps = MAX_RIDGE_STEPS;
                }

                //if (i == 2)
                //{
                /* Image Controls */
                pad_value = PAD_VALUE;
                join_line_radius = JOIN_LINE_RADIUS;

                /* Map Controls */
                blocksize = MAP_BLOCKSIZE_V2;
                windowsize = MAP_WINDOWSIZE_V2; /* windowsize */
                windowoffset = MAP_WINDOWOFFSET_V2; /* windowoffset */
                num_directions = NUM_DIRECTIONS;
                start_dir_angle = START_DIR_ANGLE;
                rmv_valid_nbr_min = RMV_VALID_NBR_MIN;
                dir_strength_min = DIR_STRENGTH_MIN;
                dir_distance_max = DIR_DISTANCE_MAX;
                smth_valid_nbr_min = SMTH_VALID_NBR_MIN;
                vort_valid_nbr_min = VORT_VALID_NBR_MIN;
                highcurv_vorticity_min = HIGHCURV_VORTICITY_MIN;
                highcurv_curvature_min = HIGHCURV_CURVATURE_MIN;
                min_interpolate_nbrs = MIN_INTERPOLATE_NBRS; /* min_interpolate_nbrs */
                percentile_min_max = PERCENTILE_MIN_MAX; /* percentile_min_max   */
                min_contrast_delta = MIN_CONTRAST_DELTA; /* min_contrast_delta   */

                /* DFT Controls */
                num_dft_waves = NUM_DFT_WAVES;
                powmax_min = POWMAX_MIN;
                pownorm_min = POWNORM_MIN;
                powmax_max = POWMAX_MAX;
                fork_interval = FORK_INTERVAL;
                fork_pct_powmax = FORK_PCT_POWMAX;
                fork_pct_pownorm = FORK_PCT_POWNORM;

                /* Binarization Controls */
                dirbin_grid_w = DIRBIN_GRID_W;
                dirbin_grid_h = DIRBIN_GRID_H;
                isobin_grid_dim = UNUSED_INT;
                num_fill_holes = NUM_FILL_HOLES;

                /* Minutiae Detection Controls */
                max_minutia_delta = MAX_MINUTIA_DELTA;
                max_high_curve_theta = MAX_HIGH_CURVE_THETA;
                high_curve_half_contour = HIGH_CURVE_HALF_CONTOUR;
                min_loop_len = MIN_LOOP_LEN;
                min_loop_aspect_dist = MIN_LOOP_ASPECT_DIST;
                min_loop_aspect_ratio = MIN_LOOP_ASPECT_RATIO;

                /* Minutiae Link Controls */
                link_table_dim = UNUSED_INT;
                max_link_dist = UNUSED_INT;
                min_theta_dist = UNUSED_INT;
                maxtrans = MAXTRANS;
                score_theta_norm = UNUSED_DBL;
                score_dist_norm = UNUSED_DBL;
                score_dist_weight = UNUSED_DBL;
                score_numerator = UNUSED_DBL;

                /* False Minutiae Removal Controls */
                max_rmtest_dist = MAX_RMTEST_DIST_V2;
                max_hook_len = MAX_HOOK_LEN_V2;
                max_half_loop = MAX_HALF_LOOP_V2;
                trans_dir_pix = TRANS_DIR_PIX_V2;
                small_loop_len = SMALL_LOOP_LEN;
                side_half_contour = SIDE_HALF_CONTOUR;
                inv_block_margin = INV_BLOCK_MARGIN_V2;
                rm_valid_nbr_min = RM_VALID_NBR_MIN;
                max_overlap_dist = MAX_OVERLAP_DIST; /* max_overlap_dist       */
                max_overlap_join_dist = MAX_OVERLAP_JOIN_DIST; /* max_overlap_join_dist  */
                malformation_steps_1 = MALFORMATION_STEPS_1; /* malformation_steps_1   */
                malformation_steps_2 = MALFORMATION_STEPS_2; /* malformation_steps_2   */
                min_malformation_ratio = MIN_MALFORMATION_RATIO; /* min_malformation_ratio */
                max_malformation_dist = MAX_MALFORMATION_DIST; /* max_malformation_dist  */
                pores_trans_r = PORES_TRANS_R;
                pores_perp_steps = PORES_PERP_STEPS;
                pores_steps_fwd = PORES_STEPS_FWD;
                pores_steps_bwd = PORES_STEPS_BWD;
                pores_min_dist2 = PORES_MIN_DIST2;
                pores_max_ratio = PORES_MAX_RATIO;

                /* Ridge Counting Controls */
                max_nbrs = MAX_NBRS;
                max_ridge_steps = MAX_RIDGE_STEPS;
                //}
            }
        };

        #endregion

        #region Other constants

        internal const string MIN_TXT_EXT = "min";
        internal const string LOW_CONTRAST_MAP_EXT = "lcm";
        internal const string HIGH_CURVE_MAP_EXT = "hcm";
        internal const string DIRECTION_MAP_EXT = "dm";
        internal const string LOW_FLOW_MAP_EXT = "lfm";
        internal const string QUALITY_MAP_EXT = "qm";
        internal const string AN2K_OUT_EXT = "mdt";
        internal const string BINARY_IMG_EXT = "brw";
        internal const string XYT_EXT = "xyt";
        internal const int NIST_INTERNAL_XYT_REP = 0;
        internal const int M1_XYT_REP = 1;
        internal const double M_PI = 3.14159265358979323846;

        // 10, 2X3 pixel pair feature patterns used to define ridge endings and bifurcations.
        // 2nd pixel pair is permitted to repeat multiple times in match.
        internal const int NFEATURES = 10;
        internal const int BIFURCATION = 0;
        internal const int RIDGE_ENDING = 1;
        internal const int DISAPPEARING = 0;
        internal const int APPEARING = 1;
        // Intensity used to fill padded image area 
        internal const int PAD_VALUE = 128; /* medium gray @ 8 bits */
        // Intensity used to draw on grayscale images 
        internal const int DRAW_PIXEL = 255; /* white in 8 bits */
        // Definitions for 8-bit binary pixel intensities.
        internal const int WHITE_PIXEL = 255;
        internal const int BLACK_PIXEL = 0;


        // Definitions for controlling join_miutia(). Draw without opposite perimeter pixels.
        internal const int NO_BOUNDARY = 0;

        // Draw with opposite perimeter pixels.
        internal const int WITH_BOUNDARY = 1;

        // Radial width added to join line (not including the boundary pixels).
        internal const int JOIN_LINE_RADIUS = 1;

        #endregion

        #region MAP CONSTANTS

        // Map value for not well-defined directions 
        internal const int INVALID_DIR = -1;

        // Map value assigned when the current block has no neighbors with valid direction.
        internal const int NO_VALID_NBRS = -3;

        // Map value designating a block is near a high-curvature area such as a core or delta.
        internal const int HIGH_CURVATURE = -2;

        // This specifies the pixel dimensions of each block in the IMAP 
        internal const int IMAP_BLOCKSIZE = 24;

        /* Pixel dimension of image blocks. The following three constants work */
        /* together to define a system of 8X8 adjacent and non-overlapping     */
        /* blocks that are assigned results from analyzing a larger 24X24      */
        /* window centered about each of the 8X8 blocks.                       */
        /* CAUTION: If MAP_BLOCKSIZE_V2 is changed, then the following will    */
        /* likely need to be changed:  MAP_WINDOWOFFSET_V2,                    */
        /*                             TRANS_DIR_PIX_V2,                       */
        /*                             INV_BLOCK_MARGIN_V2                     */
        internal const int MAP_BLOCKSIZE_V2 = 8;

        // Pixel dimension of window that surrounds the block.  The result from analyzing the content of the window is stored in the interior block.
        internal const int MAP_WINDOWSIZE_V2 = 24;

        // Pixel offset in X & Y from the origin of the block to the origin of the surrounding window.
        internal const int MAP_WINDOWOFFSET_V2 = 8;

        /* This is the number of integer directions to be used in semicircle. */
        /* CAUTION: If NUM_DIRECTIONS is changed, then the following will     */
        /* likely need to be changed:  HIGHCURV_VORTICITY_MIN,                */
        /*                             HIGHCURV_CURVATURE_MIN,                */
        /*                             FORK_INTERVAL                          */
        internal const int NUM_DIRECTIONS = 16;

        // This is the theta from which integer directions are to begin.
        internal const double START_DIR_ANGLE = (double)(M_PI / 2.0); /* 90 degrees */

        // Minimum number of valid neighbors required for a valid block value to keep from being removed.
        internal const int RMV_VALID_NBR_MIN = 3;

        /* Minimum strength for a direction to be considered significant. */
        internal const double DIR_STRENGTH_MIN = 0.2;

        /* Maximum distance allowable between valid block direction */
        /* and the average direction of its neighbors before the    */
        /* direction is removed.                                    */
        internal const int DIR_DISTANCE_MAX = 3;

        /* Minimum number of valid neighbors required for an       */
        /* INVALID block direction to receive its direction from   */
        /* the average of its neighbors.                           */
        internal const int SMTH_VALID_NBR_MIN = 7;

        /* Minimum number of valid neighbors required for a block  */
        /* with an INVALID block direction to be measured for      */
        /* vorticity.                                              */
        internal const int VORT_VALID_NBR_MIN = 7;

        /* The minimum vorticity value whereby an INVALID block       */
        /* is determined to be high-curvature based on the directions */
        /* of it neighbors.                                           */
        internal const int HIGHCURV_VORTICITY_MIN = 5;

        /* The minimum curvature value whereby a VALID direction block is  */
        /* determined to be high-curvature based on it value compared with */
        /* its neighbors' directions.                                      */
        internal const int HIGHCURV_CURVATURE_MIN = 5;

        /* Minimum number of neighbors with VALID direction for an INVALID         */
        /* directon block to have its direction interpolated from those neighbors. */
        internal const int MIN_INTERPOLATE_NBRS = 2;

        /* Definitions for creating a low contrast map. */
        /* Percentile cut off for choosing min and max pixel intensities */
        /* in a block.                                                   */
        internal const int PERCENTILE_MIN_MAX = 10;

        /* The minimum delta between min and max percentile pixel intensities */
        /* in block for block NOT to be considered low contrast.  (Note that  */
        /* this value is in terms of 6-bit pixels.)                           */
        internal const int MIN_CONTRAST_DELTA = 5;

        #endregion

        #region DFT CONSTANTS

        /* This specifies the number of DFT wave forms to be applied */
        internal const int NUM_DFT_WAVES = 4;

        /* Minimum total DFT power for any given block  */
        /* which is used to compute an average power.   */
        /* By setting a non-zero minimum total,possible */
        /* division by zero is avoided.  This value was */
        /* taken from HO39.                             */
        internal const double MIN_POWER_SUM = 10.0;

        /* Thresholds and factors used by HO39.  Renamed     */
        /* here to give more meaning.                        */
        /* HO39 Name=Value */
        /* Minimum DFT power allowable in any one direction. */
        internal const double POWMAX_MIN = 100000.0; /*     thrhf=1e5f  */

        /* Minimum normalized power allowable in any one     */
        /* direction.                                        */
        internal const double POWNORM_MIN = 3.8; /*      disc=3.8f  */

        /* Maximum power allowable at the lowest frequency   */
        /* DFT wave.                                         */
        internal const double POWMAX_MAX = 50000000.0; /*     thrlf=5e7f  */

        /* Check for a fork at +- this number of units from  */
        /* current integer direction.  For example,          */
        /*           2 dir ==> 11.25 X 2 degrees.            */
        internal const int FORK_INTERVAL = 2;

        /* Minimum DFT power allowable at fork angles is     */
        /* FORK_PCT_POWMAX X block's max directional power.  */
        internal const double FORK_PCT_POWMAX = 0.7;

        /* Minimum normalized power allowable at fork angles */
        /* is FORK_PCT_POWNORM X POWNORM_MIN                 */
        internal const double FORK_PCT_POWNORM = 0.75;

        #endregion

        #region BINRAIZATION CONSTANTS

        /* Directional binarization grid dimensions. */
        internal const int DIRBIN_GRID_W = 7;
        internal const int DIRBIN_GRID_H = 9;

        /* The pixel dimension (square) of the grid used in isotropic      */
        /* binarization.                                                   */
        internal const int ISOBIN_GRID_DIM = 11;

        /* Number of passes through the resulting binary image where holes */
        /* of pixel length 1 in horizontal and vertical runs are filled.   */
        internal const int NUM_FILL_HOLES = 3;

        #endregion

        #region MINUTIAE DETECTION CONSTANTS

        /* The maximum pixel translation distance in X or Y within which */
        /* two potential minutia points are to be considered similar.    */
        internal const int MAX_MINUTIA_DELTA = 10;

        /* If the angle of a contour exceeds this angle, then it is NOT */
        /* to be considered to contain minutiae.                         */
        internal const double MAX_HIGH_CURVE_THETA = (double)(M_PI / 3.0);

        /* Half the length in pixels to be extracted for a high-curvature contour. */
        internal const int HIGH_CURVE_HALF_CONTOUR = 14;

        /* Loop must be larger than this threshold (in pixels) to be considered */
        /* to contain minutiae.                                                  */
        internal const int MIN_LOOP_LEN = 20;

        /* If loop's minimum distance half way across its contour is less than */
        /* this threshold, then loop is tested for minutiae.                    */
        internal const double MIN_LOOP_ASPECT_DIST = 1.0;

        /* If ratio of loop's maximum/minimum distances half way across its   */
        /* contour is >=  to this threshold, then loop is tested for minutiae. */
        internal const double MIN_LOOP_ASPECT_RATIO = 2.25;

        /* There are 10 unique feature patterns with ID = [0..9] , */
        /* so set LOOP ID to 10 (one more than max pattern ID).    */
        internal const int LOOP_ID = 10;

        /* Definitions for controlling the scanning of minutiae. */
        internal const int SCAN_HORIZONTAL = 0;
        internal const int SCAN_VERTICAL = 1;
        internal const int SCAN_CLOCKWISE = 0;
        internal const int SCAN_COUNTER_CLOCKWISE = 1;

        /* The dimension of the chaincode loopkup matrix. */
        internal const int NBR8_DIM = 3;

        /* Default minutiae reliability. */
        internal const double DEFAULT_RELIABILITY = 0.99;

        /* Medium minutia reliability. */
        internal const double MEDIUM_RELIABILITY = 0.50;

        /* High minutia reliability. */
        internal const double HIGH_RELIABILITY = 0.99;

        #endregion

        #region MINUTIAE LINKING CONSTANTS

        /* Definitions for controlling the linking of minutiae. */
        /* Square dimensions of 2D table of potentially linked minutiae. */
        internal const int LINK_TABLE_DIM = 20;

        /* Distance (in pixels) used to determine if the orthogonal distance  */
        /* between the coordinates of 2 minutia points are sufficiently close */
        /* to be considered for linking.                                      */
        internal const int MAX_LINK_DIST = 20;

        /* Minimum distance (in pixels) between 2 minutia points that an angle */
        /* computed between the points may be considered reliable.             */
        internal const int MIN_THETA_DIST = 5;

        /* Maximum number of transitions along a contiguous pixel trajectory    */
        /* between 2 minutia points for that trajectory to be considered "free" */
        /* of obstacles.                                                        */
        internal const int MAXTRANS = 2;

        /* Parameters used to compute a link score between 2 minutiae. */
        internal const double SCORE_THETA_NORM = 15.0;
        internal const double SCORE_DIST_NORM = 10.0;
        internal const double SCORE_DIST_WEIGHT = 4.0;
        internal const double SCORE_NUMERATOR = 32000.0;

        #endregion

        #region FALSE MINUTIAE REMOVAL CONSTANTS

        /* Definitions for removing hooks, islands, lakes, and overlaps. */
        /* Distance (in pixels) used to determine if the orthogonal distance  */
        /* between the coordinates of 2 minutia points are sufficiently close */
        /* to be considered for removal.                                      */
        internal const int MAX_RMTEST_DIST = 8;

        internal const int MAX_RMTEST_DIST_V2 = 16;

        /* Length of pixel contours to be traced and analyzed for possible hooks. */
        internal const int MAX_HOOK_LEN = 15;

        internal const int MAX_HOOK_LEN_V2 = 30;

        /* Half the maximum length of pixel contours to be traced and analyzed */
        /* for possible loops (islands/lakes).                                 */
        internal const int MAX_HALF_LOOP = 15;

        internal const int MAX_HALF_LOOP_V2 = 30;

        /* Definitions for removing minutiae that are sufficiently close and */
        /* point to a block with invalid ridge flow.                         */
        /* Distance (in pixels) in direction opposite the minutia to be */
        /* considered sufficiently close to an invalid block.           */
        internal const int TRANS_DIR_PIX = 6;

        internal const int TRANS_DIR_PIX_V2 = 4;

        /* Definitions for removing small holes (islands/lakes).  */
        /* Maximum circumference (in pixels) of qualifying loops. */
        internal const int SMALL_LOOP_LEN = 15;

        /* Definitions for removing or adusting side minutiae. */
        /* Half the number of pixels to be traced to form a complete contour. */
        internal const int SIDE_HALF_CONTOUR = 7;

        /* Definitions for removing minutiae near invalid blocks. */
        /* Maximum orthogonal distance a minutia can be neighboring a block with */
        /* invalid ridge flow in order to be removed.                            */
        internal const int INV_BLOCK_MARGIN = 6;

        internal const int INV_BLOCK_MARGIN_V2 = 4;

        /* Given a sufficiently close, neighboring invalid block, if that invalid */
        /* block has a total number of neighboring blocks with valid ridge flow   */
        /* less than this threshold, then the minutia point is removed.           */
        internal const int RM_VALID_NBR_MIN = 7;

        /* Definitions for removing overlaps. */
        /* Maximum pixel distance between 2 points to be tested for overlapping */
        /* conditions.                                                          */
        internal const int MAX_OVERLAP_DIST = 8;

        /* Maximum pixel distance between 2 points on opposite sides of an overlap */
        /* will be joined.                                                         */
        internal const int MAX_OVERLAP_JOIN_DIST = 6;

        /* Definitions for removing "irregularly-shaped" minutiae. */
        /* Contour steps to be traced to 1st measuring point. */
        internal const int MALFORMATION_STEPS_1 = 10;
        /* Contour steps to be traced to 2nd measuring point. */
        internal const int MALFORMATION_STEPS_2 = 20;
        /* Minimum ratio of distances across feature at the two point to be */
        /* considered normal.                                               */
        internal const double MIN_MALFORMATION_RATIO = 2.0;
        /* Maximum distance permitted across feature to be considered normal. */
        internal const int MAX_MALFORMATION_DIST = 20;

        /* Definitions for removing minutiae on pores. */
        /* Translation distance (in pixels) from minutia point in opposite direction */
        /* in order to get off a valley edge and into the neighboring ridge.         */
        internal const int PORES_TRANS_R = 3;

        /* Number of steps (in pixels) to search for edge of current ridge. */
        internal const int PORES_PERP_STEPS = 12;

        /* Number of pixels to be traced to find forward contour points. */
        internal const int PORES_STEPS_FWD = 10;

        /* Number of pixels to be traced to find backward contour points. */
        internal const int PORES_STEPS_BWD = 8;

        /* Minimum squared distance between points before being considered zero. */
        internal const double PORES_MIN_DIST2 = 0.5;

        /* Max ratio of computed distances between pairs of forward and backward */
        /* contour points to be considered a pore.                               */
        internal const double PORES_MAX_RATIO = 2.25;

        #endregion

        #region RIDGE COUNTING CONSTANTS

        /* Definitions for detecting nearest neighbors and counting ridges. */
        /* Maximum number of nearest neighbors per minutia. */
        internal const int MAX_NBRS = 5;

        /* Maximum number of contour steps taken to validate a ridge crossing. */
        internal const int MAX_RIDGE_STEPS = 10;

        #endregion

        #region  QUALITY/RELIABILITY DEFINITIONS

        /* Quality map levels */
        internal const int QMAP_LEVELS = 5;

        /* Neighborhood radius in millimeters computed from 11 pixles */
        /* scanned at 19.69 pixels/mm. */
        internal const double RADIUS_MM = (double)(11.0 / 19.69);

        /* Ideal Standard Deviation of pixel values in a neighborhood. */
        internal const int IDEALSTDEV = 64;
        /* Ideal Mean of pixel values in a neighborhood. */
        internal const int IDEALMEAN = 127;

        /* Look for neighbors this many blocks away. */
        internal const int NEIGHBOR_DELTA = 2;

        #endregion

        #region GENERAL DEFINITIONS

        internal const string LFS_VERSION_STR = "NIST_LFS_VER2";

        /* This factor converts degrees to radians. */
        internal const double DEG2RAD = (double)(M_PI / 180.0);

        internal const int NORTH = 0;
        internal const int SOUTH = 4;
        internal const int EAST = 2;
        internal const int WEST = 6;
        internal const int HOOK_FOUND = 1;
        internal const int LOOP_FOUND = 1;
        internal const int IGNORE = 2;
        internal const int LIST_FULL = 3;
        internal const int INCOMPLETE = 3;

        /* Pixel value limit in 6-bit image. */
        internal const int IMG_6BIT_PIX_LIMIT = 64;

        /* Maximum number (or reallocated chunks) of minutia to be detected */
        /* in an image.                                                     */
        internal const int MAX_MINUTIAE = 1000;

        /* If both deltas in X and Y for a line of specified slope is less than */
        /* this threshold, then the angle for the line is set to 0 radians.     */
        internal const double MIN_SLOPE_DELTA = 0.5;

        /* Designates that rotated grid offsets should be relative */
        /* to the grid's center.                                   */
        internal const int RELATIVE2CENTER = 0;

        /* Designates that rotated grid offsets should be relative */
        /* to the grid's origin.                                   */
        internal const int RELATIVE2ORIGIN = 1;

        /* Truncate floating point precision by multiply, rounding, and then */
        /* dividing by this value.  This enables consistant results across   */
        /* different computer architectures.                                 */
        internal const double TRUNC_SCALE = 16384.0;

        /* Designates passed argument as undefined. */
        internal const int UNDEFINED = -1;

        /* Dummy values for unused LFS control parameters. */
        internal const int UNUSED_INT = 0;
        internal const double UNUSED_DBL = 0.0;

        #endregion

        internal static List<double> dft_coefs = new List<double>() { 1, 2, 3, 4 };
        // internal static LFSPARMS lfsparms = new LFSPARMS(1);
        internal static LFSPARMS lfsparms = new LFSPARMS(2);
        // internal static List<int> nbr8_dx;
        // internal static List<int> nbr8_dy;
        // internal static List<int> chaincodes_nbr8;

        #endregion

        #region mlp.h

        /* Formerly in mlp/fmt_msgs.h */
        /* For use by strm_fmt() and lgl_tbl(), which format the warning and
        error messages that may be written as the result of scanning a
        specfile.  Columns are numbered starting at 0. */

        internal const int MESSAGE_FIRSTCOL_FIRSTLINE = 6; /* for first line of a msg */
        internal const int MESSAGE_FIRSTCOL_LATERLINES = 8; /* later lines indented */
        internal const int MESSAGE_LASTCOL = 70;
        internal const int MESSAGE_FIRSTCOL_TABLE = 12; /* table indented even more */

        /***********************************************************************/
        /* Formerly in mlp/get_phr.h */
        /* Names of get_phr()'s return values: */
        internal const char WORD_PAIR = (char)0;
        internal const char NEWRUN = (char)1;
        internal const char ILLEGAL_PHRASE = (char)2;
        internal const char FINISHED = (char)3;

        /***********************************************************************/
        /* Formerly in mlp/lbfgs_dr.h */
        //internal const int  STPMIN =1.e-20;
        //internal const int  STPMAX= 1.e+20;

        /***********************************************************************/
        /* Formerly in mlp/lims.h */
        internal const int MAXMED = 100000;
        internal const int LONG_CLASSNAME_MAXSTRLEN = 32;

        /***********************************************************************/
        /* Formerly in mlp/macros.h */
        //internal const int  mlp_min(x,y) ((x)<=(y)?(x):(y))
        //internal const int  mlp_max(x,y) ((x)>=(y)?(x):(y))

        //!!!!!

        /***********************************************************************/
        /* Formerly in mlp/mtch_pnm.h */
        /* Names of the values of the a_type parm of mtch_pnm. */
        internal const char MP_FILENAME = (char)0;
        internal const char MP_INT = (char)1;
        internal const char MP_FLOAT = (char)2;
        internal const char MP_SWITCH = (char)3;

        /* Bundles together some parms for mtch_pnm, to reduce the verbosity
        of the (many) calls of it by st_nv_ok. */

        /***********************************************************************/
        /* Formerly in mlp/rd_words.h */
        internal const char RD_INT = (char)0;
        internal const char RD_FLOAT = (char)1;

        /***********************************************************************/
        /* Formerly in mlp/scg.h */
        internal const float XLSTART = 0.01F; /* Starting value for xl. */
        internal const int NF = 3; /* Don't quit until NF * nfreq iters or... */
        internal const int NITER = 40; /* ...until NITER iters, whichever is larger... */
        internal const int NBOLTZ = 100; /* ...until NBOLTZ iters, if doing Boltzmann. */
        internal const int NNOT = 3; /* Quit if not improving NNOT times in row. */
        internal const int NRESTART = 100000; /* Restart after NRESTART iterations. */

        /***********************************************************************/
        /* Formerly in mlp/parms.h */
        internal const char PARMTYPE_FILENAME = (char)0;
        internal const char PARMTYPE_INT = (char)1;
        internal const char PARMTYPE_FLOAT = (char)2;
        internal const char PARMTYPE_SWITCH = (char)3;

        internal const int PARM_FILENAME_VAL_DIM = 100;

        internal struct SSL
        {
            private char set_tried, set;
            private int linenum;
        };

        internal struct PARM_FILENAME
        {
            private string val; // length PARM_FILENAME_VAL_DIM
            private SSL ssl;
        };

        internal struct PARM_INT
        {
            private int val;
            private SSL ssl;
        };

        internal struct PARM_FLOAT
        {
            private float val;
            private SSL ssl;
        };

        internal struct PARM_SWITCH
        {
            private char val;
            private SSL ssl;
        };

        internal struct PARMS
        {
            private PARM_FILENAME long_outfile,
                short_outfile,
                patterns_infile,
                wts_infile,
                wts_outfile,
                class_wts_infile,
                pattern_wts_infile,
                lcn_scn_infile;

            private PARM_INT npats,
                ninps,
                nhids,
                nouts,
                seed,
                niter_max,
                nfreq,
                nokdel,
                lbfgs_mem;

            private PARM_FLOAT regfac,
                alpha,
                temperature,
                egoal,
                gwgoal,
                errdel,
                oklvl,
                trgoff,
                scg_earlystop_pct,
                lbfgs_gtol;

            private PARM_SWITCH errfunc,
                purpose,
                boltzmann,
                train_or_test,
                acfunc_hids,
                acfunc_outs,
                priors,
                patsfile_ascii_or_binary,
                do_confuse,
                show_acs_times_1000,
                do_cvr;
        };


        /* Symbolic names of values of "switch" parms.  The corresponding
        value strings (expected in the spec file) are these names but in lower
        case; the numerical values are also ok in the spec file.  For example,
        to set errfunc to MSE, use either of the following in the spec file:
          errfunc mse
          errfunc 0
        Note that the names and corresponding code-numbers here must match the
        contents of the legal_names_codes_str parms in the calls of mtch_pnm()
        by st_nv_ok(), but with the names in lower case in those calls. */

        /* For errfunc: */
        internal const char MSE = (char)0;
        internal const char TYPE_1 = (char)1;
        internal const char POS_SUM = (char)2;

        /* For purpose: */
        internal const char CLASSIFIER = (char)0;
        internal const char FITTER = (char)1;

        /* For boltzmann: */
        internal const char NO_PRUNE = (char)0;
        internal const char ABS_PRUNE = (char)2;
        internal const char SQUARE_PRUNE = (char)3;

        /* For train_or_test: */
        internal const char TRAIN = (char)0;
        internal const char TEST = (char)1;

        /* For acfunc_hids and acfunc_outs: */
        internal const char SINUSOID = (char)0;
        internal const char SIGMOID = (char)1;
        internal const char LINEAR = (char)2;
        internal const char BAD_AC_CODE = (char)127;

        /* For priors: */
        internal const char ALLSAME = (char)0;
        internal const char CLASS = (char)1;
        internal const char PATTERN = (char)2;
        internal const char BOTH = (char)3;

        /* For patsfile_ascii_or_binary: */
        internal const char ASCII = (char)0;
        internal const char BINARY = (char)1;

        /* The allowed values for the following "logical" switch parms are
        TRUE and FALSE (defined in defs.h), which should be represented in the
        spec file as true and false: do_confuse, show_acts_times_1000,
        do_cvr. */



        internal const int MAX_NHIDS = 1000; /* Maximum number of hidden nodes */
        internal const int TREEPATSFILE = 5151;
        internal const int JUSTPATSFILE = 0;
        internal const int FMT_ITEMS = 8;

        #endregion

        #region nfiqgbls.h

        //Default global means for Z-Normalization of feature vectors
        internal static float[] dflt_znorm_means = new float[]
        {
            2881.918457F,
            119.406013F,
            42.890446F,
            42.011002F,
            33.318542F,
            18.573952F,
            5.602001F,
            0.122406F,
            0.206616F,
            0.223217F,
            0.447761F
        };

        //Default global stddevs for Z-Normalization of feature vectors
        internal static float[] dflt_znorm_stds = new float[]
        {
            1.522167e+03F,
            6.759113e+01F,
            2.685183e+01F,
            2.699416e+01F,
            2.686451e+01F,
            2.199553e+01F,
            1.067551e+01F,
            5.670758e-02F,
            9.548551e-02F,
            7.412220e-02F,
            1.551811e-01F
        };

        //Default MLP weights & attributes used to classify NFIQ feature vectors
        internal static char dflt_purpose = CLASSIFIER;
        internal static int dflt_nInps = NFIQ_VCTRLEN;
        internal static int dflt_nHids = 22;
        internal static int dflt_nOuts = NFIQ_NUM_CLASSES;
        internal static char dflt_acfunc_hids = SINUSOID;
        internal static char dflt_acfunc_outs = SINUSOID;

        internal static float[] dflt_wts = new float[]
        {
            -3.119589e-01F, 6.611657e-01F, -5.026219e-01F, 3.649307e-01F, -8.559146e-01F,
            0.000000e+00F, 0.000000e+00F, 6.067616e-01F, -1.805089e-01F, -1.131759e-01F,
            -4.456701e-02F,
            1.485702e-01F, -8.047382e-01F, 1.536431e+00F, -6.267055e-01F, -2.291293e-01F,
            3.588256e-01F, -4.772107e-02F, -3.209697e-02F, 5.032505e-02F, 8.261050e-02F,
            -1.254393e-01F,
            6.132897e-01F, -1.411894e+00F, -2.838681e+00F, 3.381080e-01F, 6.434316e-01F,
            -3.236280e-01F, -7.699983e-01F, 6.468319e-01F, -4.339712e-01F, 3.309879e-01F,
            -1.358493e-01F,
            5.641593e-01F, -5.056323e-01F, 1.904472e+00F, -5.811639e-01F, 1.072900e-01F,
            5.268215e-01F, -4.427979e-02F, -4.738337e-01F, -4.169644e-01F, 2.207548e-01F,
            3.257259e-01F,
            1.309097e+00F, -2.789800e-01F, 7.167372e-01F, -1.587375e+00F, -1.048402e+00F,
            -6.534492e-01F, 7.294747e-01F, -6.075797e-01F, -4.250264e-01F, -1.189583e+00F,
            1.053694e+00F,
            9.861280e-01F, -2.313339e+00F, 1.757847e+00F, -1.355448e+00F, 1.940423e-01F,
            2.536710e-01F, 1.017378e-01F, -1.868141e-01F, -7.930036e-01F, 4.395724e-02F,
            5.294373e-01F,
            3.463260e-01F, -7.978010e-01F, 1.308727e+00F, -6.636613e-01F, -6.908953e-01F,
            2.089418e-01F, -3.036060e-01F, 1.602989e-01F, -3.998688e-01F, -1.066859e-01F,
            2.452043e-01F,
            -2.258594e+00F, 8.253821e-01F, -1.959279e+00F, -1.823486e+00F, 2.705060e-01F,
            2.474845e-01F, -5.042679e-01F, -7.290037e-01F, -9.019606e-01F, 1.191169e+00F,
            3.203081e-01F,
            -4.853204e-03F, 1.646509e+00F, 3.500651e+00F, -8.528205e-01F, -2.984882e-01F,
            4.489827e-01F, -2.571398e-01F, -1.674349e-01F, 5.363578e-01F, 7.658816e-02F,
            -3.055720e-01F,
            5.880828e-02F, -1.379533e-01F, -6.978128e-02F, -1.724346e-01F, -1.093761e+00F,
            2.889399e-01F, -4.144008e-01F, 3.218620e-01F, 1.125677e-01F, 6.271239e-01F,
            -4.318374e-01F,
            1.436580e-01F, 6.330154e-01F, -1.869801e+00F, -7.242393e-02F, 4.479175e-01F,
            -2.368623e-01F, 3.373586e-01F, -4.974723e-01F, 1.072444e+00F, -5.307779e-01F,
            -2.307383e-01F,
            -7.633747e-01F, -2.442474e+00F, 8.769030e-01F, 0.000000e+00F, -1.343813e-01F,
            -1.110623e-01F, -4.376957e-01F, 2.013028e-01F, 4.305054e-01F, 1.585102e-01F,
            -3.958981e-01F,
            5.240926e-02F, -2.490937e-01F, 1.546568e+00F, 0.000000e+00F, -1.498653e-01F,
            6.201565e-01F, 2.109777e-01F, -2.393391e-02F, 3.447105e-01F, 2.182600e-01F,
            -3.149046e-01F,
            4.906907e-01F, 1.403351e-01F, 9.017189e-01F, -1.210668e-01F, 0.000000e+00F,
            -1.031165e+00F, 3.980837e-01F, -4.873019e-01F, 4.677160e-01F, -7.404075e-01F,
            2.573620e-01F,
            0.000000e+00F, 7.471633e-02F, 0.000000e+00F, 0.000000e+00F, 4.203801e-01F,
            3.415256e-01F, -7.038820e-01F, -3.723057e-01F, -1.196930e-01F, 5.805833e-01F,
            -4.376991e-02F,
            2.435902e-01F, -8.516605e-01F, 1.756720e+00F, -3.907821e-01F, -5.916777e-01F,
            2.893333e-01F, -2.783922e-01F, 4.216255e-01F, -2.463683e-01F, 5.500950e-02F,
            0.000000e+00F,
            1.970175e+00F, -2.958485e+00F, 1.532450e+00F, -8.923713e-01F, 2.417961e-01F,
            5.628923e-01F, -2.440243e-01F, -5.965174e-01F, 1.139410e-01F, -2.022855e-01F,
            2.361645e-01F,
            0.000000e+00F, 6.754364e-01F, -7.820091e-01F, 1.013413e+00F, 2.463124e-01F,
            0.000000e+00F, 1.023046e-01F, 9.711869e-01F, -1.343744e-01F, 1.707461e-01F,
            -4.111827e-01F,
            1.260770e-01F, -2.684742e-01F, 2.472874e-01F, 1.606629e+00F, 1.411805e+00F,
            3.064711e-01F, -7.956589e-01F, 8.479106e-01F, -5.768294e-01F, -1.585826e-01F,
            1.294553e-01F,
            2.168621e+00F, -3.034966e+00F, -2.278084e+00F, 4.302388e-01F, -2.894798e-01F,
            1.411841e-01F, 9.723052e-01F, -3.195443e-01F, 1.879465e-01F, 2.471027e-01F,
            -1.112849e-01F,
            1.656582e+00F, 4.157036e-01F, -2.338839e+00F, 1.443451e-01F, 1.116763e+00F,
            -9.645813e-01F, 3.676646e-01F, 3.832221e-02F, 4.517725e-01F, 3.238751e-01F,
            -4.588898e-01F,
            1.916666e+00F, 1.443587e-01F, -1.433865e-01F, 3.240053e-01F, -4.565576e-01F,
            5.283880e-01F, -4.196049e-01F, -5.186083e-01F, -2.434821e-01F, -1.174421e-01F,
            4.113020e-01F,

            -1.783172e-01F, 0.000000e+00F, -4.627872e-01F, -3.577171e-01F, -1.231293e-01F,
            -2.868853e-02F, 7.272963e-01F, -1.405041e+00F, 2.315883e-01F, -9.232272e-02F,
            -8.280038e-01F, -8.069845e-01F, 8.964362e-01F, -1.068020e+00F, 5.136124e-01F,
            1.601462e-01F, 1.949571e+00F, 9.790514e-02F, -1.881924e+00F, -1.167217e+00F,
            -1.862109e+00F, 2.797367e-01F,

            -1.119622e+00F, 6.913635e-01F, -2.314126e+00F, 5.009105e-01F, 8.618460e-01F,
            -1.282761e-01F, 7.247450e-01F, 1.302407e+00F, -2.491474e+00F, 8.938180e-01F,
            -9.503597e-01F, -8.490843e-01F, 2.466252e-01F, -2.921042e-01F, -1.599071e-01F,
            6.380814e-01F, 1.745111e+00F, -6.558891e-01F, 1.536597e+00F, -1.801105e+00F,
            8.278723e-01F, -1.115732e+00F,
            0.000000e+00F, 4.914292e-01F, 2.043260e+00F, -1.017582e+00F, -1.337804e+00F,
            3.123492e-01F, 2.861827e-01F, -5.710307e-01F, 3.570163e-01F, -7.774934e-01F,
            1.263964e-01F, -1.019178e+00F, 4.599980e-01F, 1.334275e+00F, -8.112756e-01F,
            2.932526e-01F, 8.066791e-01F, -3.043762e-01F, -6.072499e-01F, 7.365437e-01F,
            -2.282936e+00F, 2.773401e-01F,
            -4.716995e-01F, -1.667477e-01F, 1.097124e+00F, -1.401806e+00F, -4.670044e-02F,
            -1.744312e+00F, 2.863425e-01F, -6.154195e-01F, 1.668778e+00F, -3.543290e-01F,
            -5.540912e-01F, 2.741340e-01F, 1.720121e-01F, -1.260201e+00F, -1.101267e-01F,
            2.400029e-01F, 3.960383e-02F, -6.528167e-01F, -8.679667e-01F, 1.132805e+00F,
            -1.164291e-01F, 8.910555e-01F,
            -6.713058e-01F, -2.950077e-01F, -9.163866e-01F, -5.672185e-01F, 3.286342e-01F,
            -8.068405e-01F, -5.663291e-01F, 6.598314e-01F, 1.728234e+00F, -2.267976e-01F,
            7.203241e-02F, -1.469119e-01F, -7.360125e-01F, -5.350198e-01F, 7.811673e-01F,
            -1.242507e-01F, -3.277558e-01F, 1.590503e-01F, -6.800562e-01F, 5.582014e-01F,
            1.338933e+00F, -2.008687e+00F,
            7.849125e-01F, -1.091962e+00F, -1.454492e-01F, 1.507305e+00F, -5.163293e-01F,
            5.807534e-01F, -9.741927e-01F, -1.288107e-01F, 6.773449e-01F, 2.835115e-02F,
            0.000000e+00F, 2.863308e+00F, -9.951057e-01F, 1.037028e-01F, -8.899538e-01F,
            -1.521524e+00F, -2.041125e+00F, 4.552743e-01F, 3.357547e-01F, -2.086011e-01F,
            -8.699499e-01F, 2.495338e-01F,

            -6.955023e-01F, -1.218332e+00F, -3.902654e-01F, -2.646753e-01F, -8.862965e-01F,
        };

        #endregion

        #region imgdecod.h

        internal const int IMG_IGNORE = 2;

        #endregion

        #region imgtype.h

        internal const int UNKNOWN_IMG = -1;
        internal const int RAW_IMG = 0;
        internal const int WSQ_IMG = 1;
        internal const int JPEGL_IMG = 2;
        internal const int JPEGB_IMG = 3;
        internal const int IHEAD_IMG = 4;
        internal const int ANSI_NIST_IMG = 5;
        internal const int JP2_IMG = 6;
        internal const int PNG_IMG = 7;

        #endregion

        #region jpegl.h

        /* JPEGL Marker Definitions */
        internal static int SOF3 = 0xffc3;
        internal static int DHT = 0xffc4;
        internal static int RST0 = 0xffd0;
        internal static int RST1 = 0xffd1;
        internal static int RST2 = 0xffd2;
        internal static int RST3 = 0xffd3;
        internal static int RST4 = 0xffd4;
        internal static int RST5 = 0xffd5;
        internal static int RST6 = 0xffd6;
        internal static int RST7 = 0xffd7;
        internal const int SOI = 0xffd8;
        internal static int EOI = 0xffd9;
        internal static int SOS = 0xffda;
        internal static int SOF0 = 0xffc0;
        internal static int DNL = 0xffdc;
        internal static int DRI = 0xffdd;
        internal static int COM = 0xfffe;
        internal const int APP0 = 0xffe0;
        /* Case for getting ANY marker. */
        internal static int ANY = 0xffff;
        /* Cases for getting a table from a set of possible ones. */
        internal const int TBLS_N_SOF = 2;
        internal const int TBLS_N_SOS = TBLS_N_SOF + 1;

        internal struct IMG_DAT
        {
            internal int max_width, max_height, pix_depth, ppi;
            internal int intrlv; /* 0 = no, 1 = yes */
            internal int n_cmpnts;
            internal int cmpnt_depth;
            internal int[] hor_sampfctr; // = new int[MAX_CMPNTS];
            internal int[] vrt_sampfctr; // = new int[MAX_CMPNTS];
            internal int[] samp_width; // = new int[MAX_CMPNTS];
            internal int[] samp_height; // = new int[MAX_CMPNTS];
            //internal char point_trans[MAX_CMPNTS];
            //internal char predict[MAX_CMPNTS];
            //internal char *image[MAX_CMPNTS];
            internal string point_trans;
            internal string predict;
            internal string image;
            internal short[] diff; // = new short[MAX_CMPNTS]; /* was short ** */
        };

        #endregion

        #region Constant intrlv.h

        internal const int MAX_CMPNTS = 4;

        #endregion

        #region defs.h

        internal static double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        internal static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        internal static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        internal static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        internal static int Sround(int x)
        {
            return (int)(x < 0 ? x - 0.5 : x + 0.5);
        }

        internal static int Sround(double x)
        {
            return (int)(x < 0 ? x - 0.5 : x + 0.5);
        }

        internal static int AlignTo16(int v)
        {
            return ((v + 15) >> 4) << 4;
        }

        internal static int AlignTo32(int v)
        {
            return ((v + 31) >> 5) << 5;
        }

        #endregion

        #region lfs.h

        internal static double trunc_dbl_precision(int x, int scale)
        {
            return (double)(x < 0.0
                ? (int)((x * scale - 0.5) / scale)
                : (int)((x * scale + 0.5) / scale));
        }

        internal static double trunc_dbl_precision(double x, double scale)
        {
            return (double)(x < 0.0
                ? (int)((x * scale - 0.5) / scale)
                : (int)((x * scale + 0.5) / scale));
        }

        #endregion

        #region zconf.h

        internal static int SEEK_END = 2;

        #endregion

        #region wsq.h

        internal static int SOI_WSQ = 0xffa0;

        #endregion

        #region ihead.h

        /* Defines used by the ihead structure */
        internal static int IHDR_SIZE = 288; /* len of hdr record (always even bytes) */
        internal static int SHORT_CHARS = 8; /* # of ASCII chars to represent a short */
        internal static int BUFSIZE = 80; /* default buffer size */
        internal static int DATELEN = 26; /* character length of date string */

        #endregion

        #endregion
    }
}
