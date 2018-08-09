using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;

// Manages player stats, resources, relationships, etc and displaying those on the screen
public class PlayerManager : MonoBehaviour {
    //Why Vector2: X values are actual values, Y values are cap values for resources or goal value for stats
    /// <summary>
    /// Resources are things players use to pay to do stuffs (Rest, Money, etc)
    /// Stats are things players try to improve to get to a goal level (Salsa, Culture, etc)
    /// </summary>
    Dictionary<string, Vector2> playerResources;
    Dictionary<string, Vector2> playerStats;

    //For relationships
    Dictionary<string, int> relationships;

    /// <summary>
    /// Hidden variables are used to control the flow of the game from the spreadsheet,
    /// without showing the players what's happening under the hood
    /// </summary>
    Dictionary<string, int> hiddenVariables;

    /// <summary>
    /// Multipliers are applied on gains when players choose certain actions
    /// for example when the chosen activity has Rest+10 as an outcome, but there's an x2 multipliers on Rest
    /// then they'll gain 20 Rest
    /// X is current multiplier, Y is how long it lasts
    /// </summary>
    Dictionary<string, Vector2> multipliers;

    /// <summary>
    /// In the Forgotten Planet mission:
    ///     stats display Culture and Ritual
    ///     resources has Rest
    ///     larder has Fruits and Meats
    ///     food skills include Gatherer and Hunter
    ///     multiplier text shows if there's an active multiplier (there are no multipliers on hidden variables)
    /// </summary>
    public TextMeshProUGUI statsText, resourcesText, larderText, foodSkillsText, multiplierText;

    /// ***Not being used in the Forgotten Planet mission, replaced by hidden variables and override messages
    /// <summary>
    /// Tokens are arbitrary terms that can be specified beforehand inside the program + the spreadsheet,
    /// used to control the flow of the game and chain player actions together, make certain activities conditional
    /// a class called TokenInterpreter translate each token to a readable sentence when the token is met or unmet (see TokenInterpreter)
    /// </summary>
    Dictionary<string, int> tokens;

    // Used for initializations for things in the Forgotten Planet spreadsheet
    void Start()
    {
        playerResources = new Dictionary<string, Vector2>();
        playerStats = new Dictionary<string, Vector2>();
        relationships = new Dictionary<string, int>();

        hiddenVariables = new Dictionary<string, int>();

        multipliers = new Dictionary<string, Vector2>();

        tokens = new Dictionary<string, int>();

        playerResources.Add("Rest", new Vector2(100, 100));
        playerResources.Add("Fruits", new Vector2(0, 0));
        playerResources.Add("Meats", new Vector2(0, 0));
        multipliers.Add("Rest", new Vector2(1, 0));
        multipliers.Add("Fruits", new Vector2(1, 0));
        multipliers.Add("Meats", new Vector2(1, 0));

        playerStats.Add("Culture", new Vector2(0, 100));
        playerStats.Add("Ritual", new Vector2(0, 100));
        multipliers.Add("Culture", new Vector2(1, 0));
        multipliers.Add("Ritual", new Vector2(1, 0));

        relationships.Add("Hound", 0);
        relationships.Add("Acolyte", 0);
        multipliers.Add("Hound", new Vector2(1, 0));
        multipliers.Add("Acolyte", new Vector2(1, 0));

        hiddenVariables.Add("Gatherer", 0);
        hiddenVariables.Add("Hunter", 0);
        hiddenVariables.Add("Hound", 0);
        hiddenVariables.Add("Acolyte", 0);
        hiddenVariables.Add("MetAcolyte", 0);
        hiddenVariables.Add("HasHunted", 0);
        hiddenVariables.Add("HasGathered", 0);
        hiddenVariables.Add("Endgame", 0);
        hiddenVariables.Add("HutUpgrade", 0);

        //For later
        hiddenVariables.Add("GenericOne", 0);
        hiddenVariables.Add("GenericTwo", 0);
        hiddenVariables.Add("GenericThree", 0);
        hiddenVariables.Add("GenericFour", 0);
        hiddenVariables.Add("GenericFive", 0);
        hiddenVariables.Add("GenericSix", 0);
        hiddenVariables.Add("GenericSeven", 0);
        hiddenVariables.Add("GenericEight", 0);
        hiddenVariables.Add("GenericNine", 0);
        hiddenVariables.Add("GenericTen", 0);

        //tokens.Add("FridgeStocked", 0);
        //tokens.Add("AfterNegotiations", 0);
        //tokens.Add("DroneInterest", 0);

        FindObjectOfType<RelationshipsPanelController>().PopulateRelationships();

        statsText.text = "";
        resourcesText.text = "";
        larderText.text = "";
        foodSkillsText.text = "";
        multiplierText.text = "";
    }

    /// <summary>
    /// Check a variable to see if it's a hidden variable
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public bool isHiddenVariable(string variable)
    {
        return hiddenVariables.ContainsKey(variable);
    }

    /// <summary>
    /// display values on the screen
    /// </summary>
    void Update()
    {
        string resourcesString = "";
        string statsString = "";
        string larderString = "";
        string foodSkillsString = "";
        string multiplierString = "";

        // Display resources and stats
        // By just composing strings for now, probably will need classes and objects for each stat/resource later

        // Resources text only includes Rest for the Forgotten Planet
        // the remaining resources belong to Larder
        Vector2 restValue = playerResources["Rest"];
        resourcesString += "Rest: ";
        if (restValue.y == 0)
        {
            resourcesString += restValue.x.ToString() + "\n";
        }
        else
        {
            resourcesString += restValue.x.ToString() + "/" + restValue.y.ToString() + "\n";
        }

        //Larder text
        foreach (KeyValuePair<string, Vector2> food in playerResources)
        {
            if (!food.Key.Equals("Rest"))
            {
                larderString += food.Key + ": " + food.Value.x + "\n";
            }
        }

        //stats text
        foreach (KeyValuePair<string, Vector2> stat in playerStats)
        {
            string key = stat.Key;
            Vector2 value = stat.Value;

            statsString += key + ": ";

            if (value.x >= value.y)
            { //goal for this stat achieved 
                statsString += "<color=\"green\">" + value.x.ToString() + "/" + value.y.ToString() + "</color>\n";
            }
            else
            {
                statsString += "<color=\"red\">" + value.x.ToString() + "/" + value.y.ToString() + "</color>\n";
            }
        }

        //multipliers text
        foreach (KeyValuePair<string, Vector2> multiplier in multipliers)
        {
            string eachMultiplier = "";
            if (multiplier.Value.y > 0)
            {
                eachMultiplier = "\n" + multiplier.Key + " gains x" + multiplier.Value.x.ToString() + " for " + multiplier.Value.y.ToString() + " time slots";
                multiplierString += eachMultiplier;
            }
        }
        
        //skills text
        if (hiddenVariables["Gatherer"] > 0)
        {
            foodSkillsString += "Gatherer\n";
        }
        if (hiddenVariables["Hunter"] > 0)
        {
            foodSkillsString += "Hunter\n";
        }

        //if (tokens.Count > 0)
        //{
        //    foreach (string token in tokens.Keys)
        //    {
        //        multiplierString += "\n" + tokenInterpreter.GetConditionMetString(token);
        //    }
        //}

        statsText.text = statsString;
        resourcesText.text = resourcesString;
        larderText.text = larderString;
        foodSkillsText.text = foodSkillsString;
        multiplierText.text = multiplierString;
    }

    public Dictionary<string, int> GetRelationships()
    {
        return relationships;
    }

    /// <summary>
    /// Check if there is enough resource to pay the cost for the specified activity
    /// costs for now only use variables in the Resources list
    /// </summary>
    /// <param name="activity"></param>
    /// <returns></returns>
    public bool checkCost(Activity activity)
    {
        string cost = activity.costs;
        if (cost.Equals("")) return true;
        string[] costSplit = cost.Split(null); //split by spaces in case there are more than 1 costs

        bool enoughResource = true;

        for (int i = 0; i < costSplit.Length; i++)
        {
            string[] eachCostSplit = costSplit[i].Split('-');
            int resourceValue = (int) playerResources[eachCostSplit[0]].x;
            int costValue = int.Parse(eachCostSplit[1]);

            enoughResource = enoughResource && (resourceValue >= costValue);
        }

        return enoughResource;
    }

    /// <summary>
    /// Check the conditions in the string, return true if all conditions are satisfied
    /// condition checks for Day, or any variable defined in the Start() function
    /// each condition in the string has to be in the form of Variable<X or Variable>=X
    /// (not used for hide conditions)
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public bool checkCondition(string conditions)
    {
        //Default is true
        if (conditions.Equals(""))
        {
            return true;
        } else
        {
            //Split by space to get each condition
            string[] conditionsSplit = conditions.Split(null);
            bool result = true;

            foreach (string condition in conditionsSplit)
            {
                bool eachResult = false;
                string needed_operator;

                //string method = "op_GreaterThanOrEqual";
                //int number1 = 1;
                //int number2 = 2;
                //MethodInfo methodInfo = number1.GetType().GetMethod(method, BindingFlags.Static | BindingFlags.Public);
                //if (methodInfo.Equals(null))
                //{
                //    Debug.Log("null method");
                //}
                //bool compare = (bool)methodInfo.Invoke(null, new object[] { number1, number2 });
                //Debug.Log(number1 + (compare ? "greater than" : "less than") + number2);

                if (condition.Contains("<"))
                {
                    needed_operator = "<";
                } else
                {
                    needed_operator = ">=";
                }
                
                string[] conditionSplit = condition.Split(new[] { needed_operator }, StringSplitOptions.None);
                string conditionAttribute = conditionSplit[0];
                if (conditionSplit.Length == 1)
                {
                    Debug.Log("condition is " + conditions);
                }
                int conditionValue = int.Parse(conditionSplit[1]);

                if (conditionAttribute.Equals("Day"))
                {
                    eachResult = (FindObjectOfType<GameManager>().GetCurrentDay() >= conditionValue);
                }
                else if (conditionAttribute.Contains("AnyRelationship"))
                {
                    foreach (int value in relationships.Values)
                    {
                        eachResult = eachResult || (value >= conditionValue);
                    }
                }
                else if (playerStats.ContainsKey(conditionAttribute))
                {
                    eachResult = playerStats[conditionAttribute].x >= conditionValue;
                }
                else if (playerResources.ContainsKey(conditionAttribute))
                {
                    eachResult = playerResources[conditionAttribute].x >= conditionValue;
                }
                else if (relationships.ContainsKey(conditionAttribute))
                {
                    eachResult = relationships[conditionAttribute] >= conditionValue;
                } else if (tokens.ContainsKey(conditionAttribute))
                {
                    eachResult = tokens[conditionAttribute] >= conditionValue;
                } else if (hiddenVariables.ContainsKey(conditionAttribute))
                {
                    eachResult = hiddenVariables[conditionAttribute] >= conditionValue;
                }

                if (needed_operator.Equals("<"))
                {
                    eachResult = !eachResult;
                }

                result = result && eachResult;
            }

            return result;
        }
    }

    /// <summary>
    /// The equivalent of checkCondition but for hide conditions
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public bool checkHiddenCondition(string conditions)
    {
        //Default is false (meaning not hidden)
        if (conditions.Equals(""))
        {
            return false;
        }
        else
        {
            string[] conditionsSplit = conditions.Split(null);
            bool result = true;

            foreach (string condition in conditionsSplit)
            {
                bool eachResult = false;
                string needed_operator;

                if (condition.Contains("<"))
                {
                    needed_operator = "<";
                }
                else
                {
                    needed_operator = ">=";
                }

                string[] conditionSplit = condition.Split(new[] { needed_operator }, StringSplitOptions.None);
                string conditionAttribute = conditionSplit[0];
                int conditionValue = int.Parse(conditionSplit[1]);

                if (conditionAttribute.Equals("Day"))
                {
                    eachResult = (FindObjectOfType<GameManager>().GetCurrentDay() >= conditionValue);
                }
                else if (conditionAttribute.Contains("AnyRelationship"))
                {
                    foreach (int value in relationships.Values)
                    {
                        eachResult = eachResult || (value >= conditionValue);
                    }
                }
                else if (playerStats.ContainsKey(conditionAttribute))
                {
                    eachResult = playerStats[conditionAttribute].x >= conditionValue;
                }
                else if (playerResources.ContainsKey(conditionAttribute))
                {
                    eachResult = playerResources[conditionAttribute].x >= conditionValue;
                }
                else if (relationships.ContainsKey(conditionAttribute))
                {
                    eachResult = relationships[conditionAttribute] >= conditionValue;
                }
                else if (tokens.ContainsKey(conditionAttribute))
                {
                    eachResult = tokens[conditionAttribute] >= conditionValue;
                }
                else if (hiddenVariables.ContainsKey(conditionAttribute))
                {
                    eachResult = hiddenVariables[conditionAttribute] >= conditionValue;
                }

                if (needed_operator.Equals("<"))
                {
                    eachResult = !eachResult;
                }

                result = result && eachResult;
            }

            return result;
        }
    }

    /// <summary>
    /// Check the accessibility condition of the specified activity
    /// </summary>
    /// <param name="activity"></param>
    /// <returns></returns>
    public bool checkAccessibility(Activity activity)
    {
        return checkCondition(activity.accessibilityCondition);
    }
    
    /// <summary>
    /// Get the current multiplier for the specified variable
    /// </summary>
    /// <param name="multiplierAttribute"></param>
    /// <returns></returns>
    int getMultiplier(string multiplierAttribute)
    {
        Vector2 multiplierValue = multipliers[multiplierAttribute];

        if (multiplierValue.y > 0)
        {
            return (int)multiplierValue.x;
        } else
        {
            return 1;
        }
    }

    /// <summary>
    /// Decrease the duration of all multipliers until the duration is 0
    /// </summary>
    public void decreaseMultiplierDuration()
    {
        List<string> keys = new List<string>(multipliers.Keys);
        foreach (string multiplier in keys)
        {
            if (multipliers[multiplier].y > 0)
            {
                multipliers[multiplier] -= new Vector2(0, 1);
            }
        }
    }

    /// <summary>
    /// Decrease the duration of all tokens until the duration is 0
    /// </summary>
    public void decreaseTokenDuration()
    {
        if (tokens.Count > 0)
        {
            List<string> keys = new List<string>(tokens.Keys);
            foreach (string token in keys)
            {
                if (tokens[token] >= 1)
                {
                    tokens[token] -= 1;
                }
            }
        }
    }

    /// <summary>
    /// apply the costs and mechanical outcomes of the activity
    /// this is only called when the activity is available and accessible and the costs are met
    /// </summary>
    /// <param name="activity"></param>
    public void applyChanges(Activity activity)
    {
        //if (activity.doesRequireToken())
        //{
        //    string[] tokenSplit = activity.accessibilityCondition.Split(':');

        //    tokens.Remove(tokenSplit[0]);
        //}

        string costs = activity.costs;
        string gains = activity.mechanicalOutcomes;
        string[] costSplit = costs.Split(null);
        string[] gainSplit = gains.Split(null);

        //apply the costs to resources
        if (!costs.Equals(""))
        {
            foreach (string cost in costSplit)
            {
                string[] eachCostSplit = cost.Split('-');
                int costValue = int.Parse(eachCostSplit[1]);
                playerResources[eachCostSplit[0]] -= new Vector2(costValue, 0);
            }
        }

        // apply the outcomes
        // outcomes can be:
        //           Variable+XdY+C or Variable+XdY-C (roll Y-sided dice X times, add them all up, add C to (or subtract C from) the result, add that to the current value of the Variable
        //           VariableGains*X:Y (bonus multiplier X for the Variable for the amount of Y time slots)
        //           Relationships+X (all relationships increase by X)
        //           Variable+X (any Variable increases by X)
        //           Token:X (player has the effect of Token for X time slots)
        if (!gains.Equals(""))
        {
            foreach (string gain in gainSplit)
            {
                if (!gain.Contains("+"))
                {
                    if (gain.Contains("Gains*"))
                    {
                        string[] multiplierSplit = gain.Split(new[] { "Gains*" }, StringSplitOptions.None);
                        string multiplierAttribute = multiplierSplit[0];
                        string[] multiplierValueSplit = multiplierSplit[1].Split(':');
                        Vector2 multiplierValue = new Vector2(int.Parse(multiplierValueSplit[0]), int.Parse(multiplierValueSplit[1]));

                        if (multipliers.ContainsKey(multiplierAttribute))
                        {
                            multipliers[multiplierAttribute] = multiplierValue;
                        }
                    } else
                    {
                        string[] tokenSplit = gain.Split(':');
                        if (tokens.ContainsKey(tokenSplit[0]))
                        {
                            tokens[tokenSplit[0]] = int.Parse(tokenSplit[1]);
                        }
                        else
                        {
                            tokens.Add(tokenSplit[0], int.Parse(tokenSplit[1]));
                        }
                        //FindObjectOfType<PopUpController>().AddNotification(tokenInterpreter.GetConditionMetString(tokenSplit[0]));
                    }
                }
                else
                {
                    string[] eachGainSplit = gain.Split(new[] { '+' }, 2);
                    string gainAttribute = eachGainSplit[0];
                    int gainValue = 0;

                    char needed_operator;

                    if (!eachGainSplit[1].Contains("d"))
                    {
                        gainValue = int.Parse(eachGainSplit[1]);
                    } else
                    {
                        string[] diceSplit = eachGainSplit[1].Split('d');
                        int rollTimes = int.Parse(diceSplit[0]);

                        if (gain.Contains("-"))
                        {
                            needed_operator = '-';
                        }
                        else
                        {
                            needed_operator = '+';
                        }

                        string[] diceSplit1 = diceSplit[1].Split(new[] { needed_operator }, StringSplitOptions.None);
                        int splitMax = int.Parse(diceSplit1[0]);
                        int addOn = (needed_operator.Equals('-')? (-1) : 1) * int.Parse(diceSplit1[1]);

                        System.Random random = new System.Random();

                        for (int i = 1; i <= rollTimes; i++)
                        {
                            gainValue += random.Next(1, splitMax + 1);
                        }
                        gainValue += addOn;

                    }

                    if ((gainAttribute != "Relationships") && (!hiddenVariables.ContainsKey(gainAttribute)))
                    {
                        gainValue *= getMultiplier(gainAttribute);
                    }

                    if (gainAttribute.Equals("Relationships"))
                    {
                        List<string> keys = new List<string>(relationships.Keys);

                        foreach (string relationship in keys)
                        {
                            relationships[relationship] += gainValue;
                            if (relationships[relationship] > 100)
                            {
                                relationships[relationship] = 100;
                            }
                        }
                    } else if (playerStats.ContainsKey(gainAttribute))
                    {
                        playerStats[gainAttribute] += new Vector2(gainValue, 0);
                    }
                    else if (playerResources.ContainsKey(gainAttribute))
                    {
                        playerResources[gainAttribute] += new Vector2(gainValue, 0);
                        Vector2 currentValue = playerResources[gainAttribute];
                        //if the value is greater than the cap, value is set to the cap
                        if ((currentValue.x > currentValue.y) && (currentValue.y > 0))
                        {
                            playerResources[gainAttribute] = new Vector2(currentValue.y, currentValue.y);
                        }
                    }
                    else if (relationships.ContainsKey(gainAttribute))
                    {
                        relationships[gainAttribute] += gainValue;
                        if (relationships[gainAttribute] > 100)
                        {
                            relationships[gainAttribute] = 100;
                        }
                    }
                    else if (hiddenVariables.ContainsKey(gainAttribute))
                    {
                         hiddenVariables[gainAttribute] += gainValue;
                    }
                }
            }
        }
    }
}
