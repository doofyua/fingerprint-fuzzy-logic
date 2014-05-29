using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CUDAFingerprinting.Common;

namespace Fuzzy1
{

  internal class DecisionMaker
  {


    private List<Rule> qualityRules;
    private List<Rule> fingerprintRules;
    private List<Rule> answerRules;

    public DecisionMaker()
    {
      ParseRules();
    }

    private void ParseRules()
    {
      qualityRules = RuleParser.Parse(Constants.PathToQualityRules);
      fingerprintRules = RuleParser.Parse(Constants.PathToFingerprintRules);
      answerRules = RuleParser.Parse(Constants.PathToAnswerRules);
    }

    internal LingValue GetAnswer(Tuple<InputVector, InputVector, InputVector> input)
    {
      List<LingValue> firstAnswer = GetAnswersForOneFinger(input.Item1);
      List<LingValue> secondAnswer = GetAnswersForOneFinger(input.Item2);
      List<LingValue> thirdAnswer = GetAnswersForOneFinger(input.Item3);

      List<LingValue> existingValuesAnswer = new List<LingValue>();
      foreach (LingValue item in firstAnswer)
      {
        if (item.VariableName == "fingerprintAnswer".ToLower())
        {
          LingValue newVal = new LingValue("f1", item.ValueName, item.Membership);
          existingValuesAnswer.Add(newVal);
        }
      }

      foreach (LingValue item in firstAnswer)
      {
        if (item.VariableName == "fingerprintAnswer".ToLower())
        {
          LingValue newVal = new LingValue("fingerprintAnswer1".ToLower(), item.ValueName, item.Membership);
          existingValuesAnswer.Add(newVal);
        }
      }

      foreach (LingValue item in secondAnswer)
      {
        if (item.VariableName == "fingerprintAnswer".ToLower())
        {
          LingValue newVal = new LingValue("fingerprintAnswer2".ToLower(), item.ValueName, item.Membership);
          existingValuesAnswer.Add(newVal);
        }
      }

      foreach (LingValue item in thirdAnswer)
      {
        if (item.VariableName == "fingerprintAnswer".ToLower())
        {
          LingValue newVal = new LingValue("fingerprintAnswer3".ToLower(), item.ValueName, item.Membership);
          existingValuesAnswer.Add(newVal);
        }
      }

      var results = EvaluateFullAnswers(existingValuesAnswer);

      return FindMaxLingValue(results);
    }

    public LingValue GetAnswerForFinger(InputVector inputVector)
    {
      var results = GetAnswersForOneFinger(inputVector);
      return FindMaxLingValue(results);
    }

    private LingValue FindMaxLingValue(List<LingValue> answers)
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

    private List<LingValue> GetAnswersForOneFinger(InputVector inputVector)
    {
      List<LingValue> existingValuesQuality = new List<LingValue>();
      existingValuesQuality.AddRange(Fuzzyficator.AverageQualityNfiqFuzzification(inputVector.AverageQuality));
      existingValuesQuality.AddRange(Fuzzyficator.BackgroundFuzzification(inputVector.Background));
      existingValuesQuality.AddRange(Fuzzyficator.DarknessFuzzification(inputVector.Darkness));
      existingValuesQuality.AddRange(Fuzzyficator.LowQualityBlocksNfiqFuzzification(inputVector.BadBlocks));

      List<LingValue> existingValuesFingerprint = EvaluateQuality(existingValuesQuality);
      existingValuesFingerprint.AddRange(Fuzzyficator.IdentityFuzzification(inputVector.Identity));

      var result = EvaluateFinger(existingValuesFingerprint);
      return result;
    }
    
    private List<LingValue> EvaluateFullAnswers(List<LingValue> input)
    {
      foreach (Rule rule in answerRules)
      {
        rule.Evaluate(input);
      }
      List<LingValue> result = input.FindAll((x) => x.VariableName.ToLower() == "answer");
      return result;
    }
 
    private List<LingValue> EvaluateQuality(List<LingValue> input)
    {
      foreach (Rule rule in qualityRules)
      {
        rule.Evaluate(input);
      }
      return input.FindAll((x) => x.VariableName.ToLower() == "quality");
    }

    private List<LingValue> EvaluateFinger(List<LingValue> input)
    {
      foreach (Rule rule in fingerprintRules)
      {
        rule.Evaluate(input);
      }
      return input.FindAll((x) => x.VariableName.ToLower() == "fingerprintanswer");

    }
  }
}