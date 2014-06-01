using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
  internal static class Constants
  {
    public const int SegmentationWindowSize = 12;
    public const double SegmentationWeight = 0.3;
    public const int SegmentationThreshold = 5;
    public static double BinarizationThreshold = 140.0d;

    public const int OrFieldWindowSize = 16;
    public const int OrFieldOverlap = 0;

    public const string PathToQualityRules = @"../../QualityRules.txt";
    public const string PathToFingerprintRules = @"../../FingerprintRules.txt";
    public const string PathToAnswerRules = @"../../AnswerRules.txt";

    public const string PathToDb = @"../../../Data/db/";
    //public const string PathToDb2 = @"D:\Учеба\Было\fingerprint\Handbook II Ed\FVC2000\Dbs\Db2_b\10";

    public const string pathToTemplatesDb = @"../../../Data/TemplateDB/";

    public const string qualityDb = @"../../../Data/";

    public const string resultsPath = @"../../../Data/res/";

    public const string pathToMccParams = @"../../../Sdk/MccMatchParams.xml";

    //<=35
    public const int numOfMatchForOneFinger = 10;
  }
}
