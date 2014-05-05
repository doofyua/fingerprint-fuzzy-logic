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

  }
}
