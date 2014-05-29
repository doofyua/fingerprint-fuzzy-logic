using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{ 
  public class InputVector
  {
    private double identity;

    public double Identity
    {
      get { return identity; }
      set
      {
        if (value <= 1 && value > 0)
        {
          identity = value;
        }
      }
    }
    private double darkness;

    public double Darkness
    {
      get { return darkness; }
      set
      {
        if (value > 0 && value <= 100)
        {
          darkness = value;
        }
      }
    }
    private double background;

    public double Background
    {
      get { return background; }
      set
      {
        if (value > 0 && value <= 100)
        {
          background = value;
        }
      }
    }
    private double averageQuality;

    public double AverageQuality
    {
      get { return averageQuality; }
      set
      {
        if (value < 4 && value > 0)
        {
          averageQuality = value;
        }
      }
    }
    private double badBlocks;

    public double BadBlocks
    {
      get { return badBlocks; }
      set
      {
        if (value <= 100 && value > 0)
        {
          badBlocks = value;
        }
      }
    }

  }
}
