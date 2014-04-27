﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy1
{

  #region toDel
  //internal enum Answers
  //{
  //  Idn,
  //  Yes,
  //  No
  //}

  //internal enum Qualities
  //{
  //  Low,
  //  Middle,
  //  High
  //}

  //internal enum Identity
  //{
  //  Same,
  //  Different
  //}

  //internal struct Rule
  //{
  //  internal Qualities quality;
  //  internal Identity Identity;
  //  internal Answers answer;
  //  internal double factor;
  //}
  #endregion


  internal class DecisionMaker
  {

    List<LingValue> existingValues = new List<LingValue>();
    List<Rule> UsedRules = new List<Rule>();

    private List<Rule> rules;

    internal LingValue GetAnswer(double mccAnswer, double qualityAnswer)
    {

      rules = new List<Rule>();
      
      bool isChanged = true;

      while (IsEnd() && isChanged)
      {
        isChanged = false;
        foreach (Rule rule in rules)
        {
          if (!UsedRules.Contains(rule) && (rule.Evaluate(existingValues)))
          {
            isChanged = true;
            UsedRules.Add(rule);
          }
        }
      }
      List<LingValue> answers = existingValues.FindAll(x => x.VariableName == "Answer");

      return FindMaxAnswer(answers);

    }

    //Defuzzification
    private LingValue FindMaxAnswer(List<LingValue> answers)
    {
      int index = 0;
      double maxMem = 0;
      for (int i = 0; i < answers.Count; i++)
      {
        if (maxMem <= answers[i].Membership)
        {
          index = i;
          maxMem = answers[i].Membership;
        }
      }
      return answers[index];
    }

    private bool IsEnd()
    {
      //так сказал решарпер
      return !rules.Except(UsedRules).Any()
        ||
        existingValues.Exists(x => x.VariableName == "Answer"
        && x.ValueName == "Yes") &&
        existingValues.Exists(x => x.VariableName == "Answer"
        && x.ValueName == "No") &&
        existingValues.Exists(x => x.VariableName == "Answer"
        && x.ValueName == "Idn");
    }

    #region fuzzifcation

    //identity
    List<LingValue> IdentityFuzzification(double mccAnswer)
    {
      LingValue sameValue = new LingValue("Identity", "Same", SameFunction(mccAnswer));
      LingValue differentValue = new LingValue("Identity", "Different", DifferentFunction(mccAnswer));
      return new List<LingValue>(new[] { sameValue, differentValue });
    }
    private static double SameFunction(double x)
    {
      if (x >= 0.75)
      {
        return 1;
      }
      if (x >= 0.25 && x < 0.75)
      {
        return 1 / (1 + Math.Exp(-(20 * x - 10)));
      }
      return 0;
    }
    private static double DifferentFunction(double x)
    {
      if (x >= 0.75)
      {
        return 0;
      }
      if (x >= 0.25 && x < 0.75)
      {
        return 1 / (1 + Math.Exp(20 * x - 10));
      }
      return 1;
    }

    //darkness
    List<LingValue> DarknessFuzzification(double darknessAnswer)
    {
      LingValue lowValue = new LingValue("Darkness", "Low", LowDarknessFunction(darknessAnswer));
      LingValue highValue = new LingValue("Darkness", "High", HidhDarknessFunction(darknessAnswer));
      return new List<LingValue>(new[] { lowValue, highValue });
    }
    //TODO
    private static double LowDarknessFunction(double x)
    {
      //все меньше 40%
      return 0;
    }
    private static double HidhDarknessFunction(double x)
    {
      //все больше 40%
      return 0;
    }

    //background
    List<LingValue> BackgroundFuzzification(double backgrouundAnswer)
    {
      LingValue largeBacground = new LingValue("Background", "Large", LowDarknessFunction(backgrouundAnswer));
      LingValue normalBacground = new LingValue("Background", "Normal", HidhDarknessFunction(backgrouundAnswer));
      return new List<LingValue>(new[] { largeBacground, normalBacground });
    }
    //TODO
    private static double LargeBacgroundFunction(double x)
    {
      return 0;
    }
    private static double NormalBacgroundFunction(double x)
    {
      return 0;
    }

    
    //LowQualityBlocksNfiq
    private List<LingValue> LowQualityBlocksNfiqFuzzification(double nfiqBlocksAnswer)
    {
      LingValue manyBlocks = new LingValue("LowQualityBlocks","Many", ManyLowQualityBlocksFunction(nfiqBlocksAnswer));
      LingValue littleBlocks = new LingValue("LowQualityBlocks","Little", LittleLowQualityBlocksFunction(nfiqBlocksAnswer));
      return new List<LingValue>(new[] {manyBlocks, littleBlocks});
    }
    //TODO
    private double LittleLowQualityBlocksFunction(double nfiqBlocksAnswer)
    {
      throw new NotImplementedException();
    }
    private double ManyLowQualityBlocksFunction(double nfiqBlocksAnswer)
    {
      throw new NotImplementedException();
    }


    private List<LingValue> AverageQualityNfiqFuzzification(double nfiqQualityAnswer)
    {
      LingValue highQualityNfiq = new LingValue("QualityNfiq","High",HighQualityNfiqFunction(nfiqQualityAnswer));
      //LingValue middleQualityNfiq = new LingValue("QualityNfiq","Middle",MiddleQualityNfiqFunction(nfiqQualityAnswer));
      LingValue lowQualityNfiq = new LingValue("QualityNfiq","Low",LowQualityNfiqFunction(nfiqQualityAnswer));
      return new List<LingValue>(new[] {highQualityNfiq, lowQualityNfiq});
    }
   
    //private double MiddleQualityNfiqFunction(double nfiqQualityAnswer)
    //{
    //  throw new NotImplementedException();
    //}
    private double LowQualityNfiqFunction(double x)
    {
      if (x<1)
      {
        return 0;
      }
      else
      {
        return Math.Pow(3, (-x + 1));
      }
    }
    private double HighQualityNfiqFunction(double x)
    {
      if (x > 2.5)
      {
        return 1;
      }
      else
      {
        return Math.Pow(4, (x - 2.5));
      }
    }

    private static double LowQualytyFunction(double x)
    {
      if (x < 15)
      {
        return 1;
      }
      if (x >= 15 && x < 35)
      {
        return (-0.05 * x + 1.75);
      }
      return 0;
    }
    private static double MiddleQualytyFunction(double x)
    {
      if (x < 15 || x >= 75)
      {
        return 0;
      }
      if (x >= 15 && x < 35)
      {
        return (0.05 * x - 1.75);
      }
      if (x >= 35 && x < 60)
      {
        return 1;
      }
      //x>=60 && x<75
      return -(1 / 15) * x + 5;
    }
    private static double HighQualytyFunction(double x)
    {
      if (x < 50)
      {
        return 1;
      }
      if (x >= 50 && x < 75)
      {
        return (0.04 * x - 2);
      }
      return 1;
    }

    #endregion
  }
}
