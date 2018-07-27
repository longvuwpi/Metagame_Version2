using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenInterpreter {
    [System.NonSerialized] List<string> definedTokens = new List<string>() { "AfterClass", "DrewInterest", "LhakpaInterest", "MargotInterest", "FridgeStocked" };
    [System.NonSerialized]
    List<string> tokenConditionUnmet = new List<string>() { "Can only be done after salsa class",
                                                       "Drew needs to be interested",
                                                       "Lhakpa needs to be interested",
                                                       "Margot needs to interested",
                                                       "Your fridge needs to be stocked"};
    List<string> tokenConditionMet = new List<string>() { "You can do activities after salsa class",
                                                          "Drew is interested in hanging out",
                                                          "Lhakpa is interested in hanging out",
                                                          "Margot is interested in hanging out",
                                                          "You can cook some fancy dinner now"
    };

    static TokenInterpreter Instance;

    private TokenInterpreter() {}

    public static TokenInterpreter GetInstance()
    {
        if (Instance == null)
        {
            Instance = new TokenInterpreter();
        }

        return Instance;
    }

    public string GetConditionUnmetString(string token)
    {
        return tokenConditionUnmet[definedTokens.IndexOf(token)];
    }

    public string GetConditionMetString(string token)
    {
        return tokenConditionMet[definedTokens.IndexOf(token)];
    }
}