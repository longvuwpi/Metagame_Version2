using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Each activity is an Unlockable. Attached to each Activity object
public class Activity : Unlockable {
    public Text activityText, costText, outcomeText;
    public TextMeshProUGUI availabilityText;

    [System.NonSerialized] public string activity;
    [System.NonSerialized] List<bool> availability;
    [System.NonSerialized] public string location;
    [System.NonSerialized] public string accessibilityCondition;
    [System.NonSerialized] public string costs;
    [System.NonSerialized] public string mechanicalOutcomes;
    [System.NonSerialized] string lockedOverrideMessage;
    [System.NonSerialized] public string hideCondition;
    [System.NonSerialized] string revealOverrideMessage;
    [System.NonSerialized] string notAccessibleOverrideText;
    [System.NonSerialized] string outcomeOverrideMessage;
    [System.NonSerialized] string popUpMessage;
    [System.NonSerialized] string popUpImage;
    [System.NonSerialized] Color unavailableColor = new Color(0.3725f, 0.2f, 0.2235f);
    [System.NonSerialized] bool hidden = false;

    // Use this for initialization
    void Start () {
        availabilityText.color = new Color(0.7914076f, 0, 1, 1);
        availabilityText.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        // If it's accessible
		if (isAvailable())
        {
            if (isAccessible())
            {
                availabilityText.text = GetNearestAvailable(false);
                GetComponent<Image>().color = Color.white;
                if (!FindObjectOfType<PlayerManager>().checkCost(this))
                {
                    costText.color = Color.red;
                } else
                {
                    costText.color = Color.black;
                }
            } else
            {
                //availabilityText.gameObject.SetActive(true);
                GetComponent<Image>().color = unavailableColor;
                if (!notAccessibleOverrideText.Equals(""))
                {
                    availabilityText.text = notAccessibleOverrideText;
                } else
                {
                    availabilityText.text = "Accessible when " + accessibilityCondition;
                }
            }
        } else
        {
            // else, card is unavailable
            //availabilityText.gameObject.SetActive(true);
            GetComponent<Image>().color = unavailableColor;
            availabilityText.text = GetNearestAvailable(true);
        }
	}

    // Set the content of this Activity based on the full string pulled from spreadsheet
    public void SetContent(string content)
    {
        string[] contentSplit = content.Split(',');
        int length = contentSplit.Length;

        activity = contentSplit[0].Trim();
        
        //
        //accessibilityCondition = contentSplit[3].Trim();
        //
        //mechanicalOutcomes = contentSplit[5].Trim();

        availability = new List<bool>();

        for (int i = 1; i <= 3; i++)
        {
            if (contentSplit[i].Trim().Equals("0"))
            {
                availability.Add(false);
            } else
            {
                availability.Add(true);
            }
        }

        location = contentSplit[4].Trim();

        unlockCondition = contentSplit[5].Trim();

        lockedOverrideMessage = contentSplit[6].Trim();

        hideCondition = contentSplit[7].Trim();

        revealOverrideMessage = contentSplit[8].Trim();

        accessibilityCondition = contentSplit[9].Trim();

        notAccessibleOverrideText = contentSplit[10].Trim();

        costs = contentSplit[11].Trim();

        mechanicalOutcomes = contentSplit[12].Trim();

        outcomeOverrideMessage = contentSplit[13].Trim();

        popUpMessage = contentSplit[14].Trim();

        popUpImage = contentSplit[15].Trim();

        activityText.text = activity;
        SetCostText();
        SetOutcomeText();

        SetUnlockedOnCreate();

    }

    void SetCostText()
    {

        costText.text = "Costs: " + costs;
    }

    void SetOutcomeText()
    {
        outcomeText.text = "Outcomes:";
        string[] outcomeSplit = mechanicalOutcomes.Split(null);
        foreach (string outcome in outcomeSplit)
        {
            if (outcome.Contains("Gains"))
            {
                outcomeText.text += " " + outcome;
            } else
            {
                string[] eachOutcome = outcome.Split(new[] { '+' }, 2);
                if (eachOutcome.Length < 2)
                {
                    Debug.Log("short outcome, it's " + outcome);
                }
                if (!FindObjectOfType<PlayerManager>().isHiddenVariable(eachOutcome[0])) {
                    if (!eachOutcome[1].Contains("d"))
                    {
                        outcomeText.text += " " + outcome;
                    } else
                    {
                        outcomeText.text += " " + eachOutcome[0] + "+";

                        string[] diceSplit = eachOutcome[1].Split('d');
                        int rollTimes = int.Parse(diceSplit[0]);
                        char needed_operator;

                        if (outcome.Contains("-"))
                        {
                            needed_operator = '-';
                        }
                        else
                        {
                            needed_operator = '+';
                        }

                        string[] diceSplit1 = diceSplit[1].Split(new[] { needed_operator }, StringSplitOptions.None);
                        int splitMax = int.Parse(diceSplit1[0]);
                        int addOn = (needed_operator.Equals('-') ? (-1) : 1) * int.Parse(diceSplit1[1]);
                        int lowest = 0;
                        int highest = 0;

                        for (int i = 1; i <= rollTimes; i++)
                        {
                            lowest += 1;
                            highest += splitMax;
                        }

                        lowest += addOn;
                        highest += addOn;

                        outcomeText.text += " " + lowest + " to " + highest;

                    }
                }
            }
        }
    }

    // Check the accessibility condition of this activity
    public bool isAccessible()
    {
        return FindObjectOfType<PlayerManager>().checkAccessibility(this);
    }

    public bool doesRequireToken()
    {
        return false;
    }

    // Check if this activity is available at the current time frame
    public bool isAvailable()
    {
        int currentTimeFrame = FindObjectOfType<GameManager>().GetCurrentAvailabilitySlot();
        return availability[currentTimeFrame];
    }

    public bool isHidden()
    {
        hidden = FindObjectOfType<PlayerManager>().checkHiddenCondition(hideCondition);

        return hidden;
    }

    // get the nearest time frame that the activity will be come available (if isAvailable is true) or unavailable (if isAvailable is false)
    string GetNearestAvailable(bool isAvailable)
    {
        if (!notAccessibleOverrideText.Equals(""))
        {
            return notAccessibleOverrideText;
        }

        string result = "";
        if (isAvailable)
        {
            result = "Next available: ";
        } else
        {
            result = "Next unavailable: ";
        }

        int currentAvailabilitySlot = FindObjectOfType<GameManager>().GetCurrentAvailabilitySlot();
        
        if (currentAvailabilitySlot >= (availability.Count - 1))
        {
            currentAvailabilitySlot = 0;
        } else
        {
            currentAvailabilitySlot++;
        }

        for (int slot = currentAvailabilitySlot; slot < availability.Count; slot++)
        {
            if (availability[slot].Equals(isAvailable))
            {
                result += FindObjectOfType<GameManager>().GetTimeString(slot);
                break;
            }
        }

        if (result.Equals("Next available: ") || result.Equals("Next unavailable: "))
        {
            result += "Never";
        }

        return result;
    }

    // When this activity is clicked
    public void ActivityClicked()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        GameManager gameManager = FindObjectOfType<GameManager>();

        // If the activity is accessible, available, and player has enough resource to pay for it
        if (isAccessible() && isAvailable() && playerManager.checkCost(this))
        {
            if (!popUpMessage.Equals(""))
            {
                FindObjectOfType<PopUpController>().AddNotification(popUpMessage, popUpImage);
            }

            // To next time frame
            gameManager.IncreaseTime();
            // Apply costs and mechanical outcomes of the activity
            playerManager.applyChanges(this);
            // Close the activities panel
            FindObjectOfType<ActivitiesController>().ClickedOutside();
        }
    }
}
