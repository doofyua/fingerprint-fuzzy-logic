using System;
using System.Collections.Generic;
using System.IO;

namespace Fuzzy1
{
  internal static class RuleParser
  {
    public static List<Rule> Parse(string path)
    {
      List<Rule> result = new List<Rule>();

      string[] ruleStrings = File.ReadAllLines(path);

      //rule:
      // variable1 value1, variable2 value2 = variable3 value3 * factor
      foreach (string str in ruleStrings)
      {
        if (String.IsNullOrWhiteSpace(str))
        {
          continue;
        }
        List<Term> terms = new List<Term>();
        //devide on two part terms and (answer and factor)
        string[] twoPart = str.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        //devide terms and parse it
        string[] termStrings = twoPart[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string termString in termStrings)
        {
          Term t = ParseTerm(termString);
          terms.Add(t);
        }
        //parse answer
        string[] answerAndFactor = twoPart[1].Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
        Term answer = ParseTerm(answerAndFactor[0].Trim());
        //parse factor
        double factor = (answerAndFactor.Length > 1) ? Double.Parse(answerAndFactor[1].Trim()) : 1;

        //create new rule
        Rule newRule = new Rule(terms, answer, factor);
        result.Add(newRule);
      }
      return result;
    }

    private static Term ParseTerm(string termString)
    {
      string[] variableAdnValue = termString.Split(new[]{" "},StringSplitOptions.RemoveEmptyEntries);
      return new Term(variableAdnValue[0], variableAdnValue[1]);
    }
  }
}
