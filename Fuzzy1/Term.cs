using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
  internal class Term
  {
    private string varName;
    private string valName;


    public Term(string var, string val)
    {
     
      this.varName = var;
      this.valName = val;
    }

    public string VariableName
    {
      get { return varName; }
      set { varName = value; }
    }

    public string ValueName
    {
      get { return valName; }
      set { valName = value; }
    }

    public bool Evaluate(List<LingValue> currentValues, out double valueMembership)
    {
      int index = currentValues.FindIndex(x => x.ValueName == valName && x.VariableName == varName);
      if (index != -1)
      {
        valueMembership = currentValues[index].Membership;
        return true;
      }
      else
      {
        valueMembership = 0;
        return false;
      }
     
      
    }

    public int FindIndex(List<LingValue> currentValues)
    {
      return currentValues.FindIndex(x => x.ValueName == valName && x.VariableName == varName);
    }

  }
}
