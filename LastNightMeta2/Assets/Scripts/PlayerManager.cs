using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// Manages player stats, resources, relationships, and displaying those on the screen
public class PlayerManager : MonoBehaviour {
    //Why Vector2: X values are actual values, Y values are cap values for resources or goal value for stats
    Dictionary<string, Vector2> playerResources;
    Dictionary<string, Vector2> playerStats;
    Dictionary<string, int> relationships;

    //For Multipliers, should refactor later to combine all resources and stats into one single dictionary
    //X is current multiplier, Y is how long it lasts
    Dictionary<string, Vector2> multipliers;
    public TextMeshProUGUI statsText, resourcesText, multiplierText;

    //For tokens used in conditional activities
    Dictionary<string, int> tokens;

	// Use this for initialization
	void Start () {
        playerResources = new Dictionary<string, Vector2>();
        playerStats = new Dictionary<string, Vector2>();
        relationships = new Dictionary<string, int>();

        multipliers = new Dictionary<string, Vector2>();

        tokens = new Dictionary<string, int>();

        playerResources.Add("Money", new Vector2(50, 0));
        playerResources.Add("Rest", new Vector2(100, 100));
        playerResources.Add("Distraction", new Vector2(0, 100));
        multipliers.Add("Money", new Vector2(1, 0));
        multipliers.Add("Rest", new Vector2(1, 0));
        multipliers.Add("Distraction", new Vector2(1, 0));

        playerStats.Add("Plot", new Vector2(0, 10));
        playerStats.Add("Salsa", new Vector2(0, 50));
       // playerStats.Add("Culture", new Vector2(0, 20));
        multipliers.Add("Plot", new Vector2(1, 0));
        multipliers.Add("Salsa", new Vector2(1, 0));
        //multipliers.Add("Culture", new Vector2(1, 0));

        relationships.Add("Drew", 0);
        relationships.Add("Lhakpa", 0);
        relationships.Add("Margot", 0);
        multipliers.Add("Drew", new Vector2(1, 0));
        multipliers.Add("Lhakpa", new Vector2(1, 0));
        multipliers.Add("Margot", new Vector2(1, 0));
        FindObjectOfType<RelationshipsPanelController>().PopulateRelationships();

        statsText.text = "";
        resourcesText.text = "";
        multiplierText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        string resourcesString = "";
        string statsString = "";
        string multiplierString = "";

        // Display resources and stats
        // By just composing strings for now, probably will need classes and objects for each stat/resource later

        foreach (KeyValuePair<string, Vector2> resource in playerResources)
        {
            string key = resource.Key;
            Vector2 value = resource.Value;

            resourcesString += key + ": ";

            if (value.y == 0)
            {
                resourcesString += value.x.ToString() + "\n";
            }
            else
            {
                resourcesString += value.x.ToString() + "/" + value.y.ToString() + "\n";
            }
        }

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

        foreach (KeyValuePair<string, Vector2> multiplier in multipliers)
        {
            string eachMultiplier = "";
            if (multiplier.Value.y > 0)
            {
                eachMultiplier = "\n" + multiplier.Key + " gains x" + multiplier.Value.x.ToString() + " for " + multiplier.Value.y.ToString() + " time slots";
                multiplierString += eachMultiplier;
            }
        }

        if (tokens.Count > 0)
        {
            foreach (string token in tokens.Keys)
            {
                multiplierString += "\n" + TokenInterpreter.GetInstance().GetConditionMetString(token);
            }
        }

        statsText.text = statsString;
        resourcesText.text = resourcesString;
        multiplierText.text = multiplierString;
    }

    public Dictionary<string, int> GetRelationships()
    {
        return relationships;
    }

    //Check if there is enough resource to pay the cost for the specified activity
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

    // Check the conditions in the string, return true if all conditions are satisfied
    public bool checkCondition(string conditions)
    {
        if (conditions.Equals(""))
        {
            return true;
        } else
        {
            string[] conditionsSplit = conditions.Split(null);
            bool result = true;

            foreach (string condition in conditionsSplit)
            {
                bool eachResult = false;
                if (condition.Contains(">="))
                {
                    string[] conditionSplit = condition.Split(new[] { ">=" }, StringSplitOptions.None);
                    string conditionAttribute = conditionSplit[0];
                    int conditionValue = int.Parse(conditionSplit[1]);

                    if (conditionAttribute.Equals("Day"))
                    {
                        eachResult = (FindObjectOfType<GameManager>().GetCurrentDay() > conditionValue);
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
                } else
                {
                    eachResult = tokens.ContainsKey(condition);
                }

                result = result && eachResult;
            }

            return result;
        }
    }

    // check the acessibility condition of the specified activity
    public bool checkAccessibility(Activity activity)
    {
        return checkCondition(activity.accessibilityCondition);
    }
    
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

    public void decreaseTokenDuration()
    {
        if (tokens.Count > 0)
        {
            List<string> keys = new List<string>(tokens.Keys);
            foreach (string token in keys)
            {
                if (tokens[token] > 1)
                {
                    tokens[token] -= 1;
                }
                else
                {
                    tokens.Remove(token);
                }
            }
        }
    }

    // apply the costs and mechanical outcomes of the activity
    public void applyChanges(Activity activity)
    {
        if (activity.isConditional())
        {
            string[] tokenSplit = activity.accessibilityCondition.Split(':');

            tokens.Remove(tokenSplit[0]);
        }

        string costs = activity.costs;
        string gains = activity.mechanicalOutcomes;
        string[] costSplit = costs.Split(null);
        string[] gainSplit = gains.Split(null);

        if (!costs.Equals(""))
        {
            foreach (string cost in costSplit)
            {
                string[] eachCostSplit = cost.Split('-');
                int costValue = int.Parse(eachCostSplit[1]);
                playerResources[eachCostSplit[0]] -= new Vector2(costValue, 0);
            }
        }

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
                        FindObjectOfType<PopUpController>().AddNotification(TokenInterpreter.GetInstance().GetConditionMetString(tokenSplit[0]));
                    }
                }
                else
                {
                    string[] eachGainSplit = gain.Split('+');
                    string gainAttribute = eachGainSplit[0];
                    int gainValue = int.Parse(eachGainSplit[1]);

                    if (gainAttribute != "Relationships")
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
                }
            }
        }
    }
}
