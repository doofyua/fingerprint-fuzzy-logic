using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
  internal class LingValue
  {
    private string valueName;
    private double sumMembership;
    private string variableName;
    private int numOfRules;

    public LingValue()
    { }

    public LingValue(string variableName, string valueName, double membership)
    {
      this.variableName = variableName;
      this.valueName = valueName;
      this.sumMembership = membership;
      this.numOfRules = 1;
    }

    public string ValueName
    {
      get { return valueName; }
    }

    public string VariableName
    {
      get { return variableName; }
    }

    public void AddMembership(double mem)
    {
      if (sumMembership<mem)
      {
        sumMembership = mem; 
      }
      
      //numOfRules++;
    }

    public int NumOfRules
    {
      get { return numOfRules; }     
    }

    bool IsSame(LingValue other)
    {
      return other.Membership == sumMembership
         && other.NumOfRules == numOfRules
         && other.ValueName.ToLower() == valueName.ToLower()
         && other.VariableName.ToLower() == variableName.ToLower();
    }

    public double Membership
    {
      get { return sumMembership; } // / numOfRules; }
    }
  }
}
