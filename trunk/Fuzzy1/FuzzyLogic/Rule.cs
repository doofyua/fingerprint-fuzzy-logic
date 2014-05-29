using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{
  internal class Rule
  {
    private List<Term> terms;
    private Term answer;
    private double factor;


    public Rule(List<Term> terms, Term answer, double factor)
    {
      this.terms = terms;
      this.answer = answer;
      this.factor = factor;
    }

  

    public bool Evaluate(List<LingValue> currentValues)
    {

      double newMembership = 1;
      double currentMembership;

      foreach (Term term in terms)
      {
        if (!term.Evaluate(currentValues, out currentMembership))
        {
          return false;
        }
        newMembership = Math.Min(newMembership, currentMembership);
      }
      int index = answer.FindIndex(currentValues);
      if (index == -1)
      {
        LingValue newValue = new LingValue(answer.VariableName, answer.ValueName, newMembership * factor);
        currentValues.Add(newValue);
      }
      else
      {
        currentValues[index].AddMembership(newMembership * factor);
      }
      return true;
    }
  }
}
