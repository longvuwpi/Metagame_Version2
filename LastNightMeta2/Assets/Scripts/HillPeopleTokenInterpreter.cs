using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A version of token interpreter to be used with the Hill People spreadsheet
public class HillPeopleTokenInterpreter
{
    [System.NonSerialized] List<string> definedTokens = new List<string>() { "AfterNegotiations", "DroneInterest", "FridgeStocked" };
    [System.NonSerialized]
    List<string> tokenConditionUnmet = new List<string>() { "Can only be done after negotiations",
                                                       "The Hill Drones have forgotten you",
                                                       "Your fridge has nothing fancy in it"};
    List<string> tokenConditionMet = new List<string>() { "The Queen directs the Drones to start working",
                                                          "The Hill Drones acknowledge your existence",
                                                          "Your fridge is full of useful stuffs"};

    static HillPeopleTokenInterpreter Instance;

    private HillPeopleTokenInterpreter() { }

    public static HillPeopleTokenInterpreter GetInstance()
    {
        if (Instance == null)
        {
            Instance = new HillPeopleTokenInterpreter();
        }

        return Instance;
    }

    /// <summary>
    /// If the player has the token and its duration is greater than 0, return the Condition Met string for the token
    /// otherwise return the Condition Unmet string for the token
    /// </summary>
    /// <param name="conditionString"></param>
    /// <returns></returns>
    public string GetConditionString(string conditionString)
    {
        string result = "";
        string[] conditions = conditionString.Split(null);
        foreach (string condition in conditions)
        {
            string[] conditionSplit = condition.Split(new[] { ">=" }, StringSplitOptions.None);

            if (definedTokens.Contains(conditionSplit[0]) && GameObject.FindObjectOfType<PlayerManager>().checkCondition(condition))
            {
                result += "<color=\"green\">" + GetConditionMetString(conditionSplit[0]) + "</color>\n";
            } else if (definedTokens.Contains(conditionSplit[0]) && !GameObject.FindObjectOfType<PlayerManager>().checkCondition(condition))
            {
                result += "<color=\"red\">" + GetConditionUnmetString(conditionSplit[0]) + "</color>\n";
            }
        }

        return result;
    }

    public string GetConditionUnmetString(string token)
    {
        return tokenConditionUnmet[definedTokens.IndexOf(token)];

    }

    public string GetConditionMetString(string token)
    {
        return tokenConditionMet[definedTokens.IndexOf(token)];
    }

    public bool DoesRequireToken(string conditionString)
    {
        foreach (string token in definedTokens)
        {
            if (conditionString.Contains(token))
            {
                return true;
            }
        }

        return false;
    }
}