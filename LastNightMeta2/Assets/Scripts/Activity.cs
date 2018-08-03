﻿using System;
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
    [System.NonSerialized] public string location;
    [System.NonSerialized] public string accessibilityCondition;
    [System.NonSerialized] public string costs;
    [System.NonSerialized] public string mechanicalOutcomes;
    [System.NonSerialized] List<bool> availability;
    [System.NonSerialized] string availabilityOverrideText;
    [System.NonSerialized] string outcomeOverrideText;
    [System.NonSerialized] string popUpText;
    [System.NonSerialized] string popUpPicture;
    [System.NonSerialized] public string hiddenCondition;
    [System.NonSerialized] Color unavailableColor = new Color(0.3725f, 0.2f, 0.2235f);
    [System.NonSerialized] bool requireToken = false;
    [System.NonSerialized] bool hidden = false;
    HillPeopleTokenInterpreter tokenInterpreter = HillPeopleTokenInterpreter.GetInstance();

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
                if (requireToken)
                {
                    availabilityText.text = HillPeopleTokenInterpreter.GetInstance().GetConditionString(accessibilityCondition);
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
        location = contentSplit[1].Trim();
        unlockCondition = contentSplit[2].Trim();
        accessibilityCondition = contentSplit[3].Trim();
        costs = contentSplit[4].Trim();
        mechanicalOutcomes = contentSplit[5].Trim();

        availability = new List<bool>();

        for (int i = 6; i <= 33; i++)
        {
            if (contentSplit[i].Trim().Equals("0"))
            {
                availability.Add(false);
            } else
            {
                availability.Add(true);
            }
        }

        requireToken = tokenInterpreter.DoesRequireToken(accessibilityCondition);

        availabilityOverrideText = contentSplit[34].Trim();
        outcomeOverrideText = contentSplit[35].Trim();
        popUpText = contentSplit[36].Trim();
        popUpPicture = contentSplit[37].Trim();
        hiddenCondition = contentSplit[38].Trim();

        activityText.text = activity;
        costText.text = "Costs: " + costs;
        outcomeText.text = "Outcomes: " + mechanicalOutcomes;

        SetUnlockedOnCreate();

    }

    // Check the accessibility condition of this activity
    public bool isAccessible()
    {
        return FindObjectOfType<PlayerManager>().checkAccessibility(this);
    }

    public bool doesRequireToken()
    {
        return requireToken;
    }

    // Check if this activity is available at the current time frame
    public bool isAvailable()
    {
        int currentTimeFrame = FindObjectOfType<GameManager>().GetCurrentAvailabilitySlot();
        return availability[currentTimeFrame];
    }

    public bool isHidden()
    {
        hidden = FindObjectOfType<PlayerManager>().checkHiddenCondition(this);

        return hidden;
    }

    // get the nearest time frame that the activity will be come available (if isAvailable is true) or unavailable (if isAvailable is false)
    string GetNearestAvailable(bool isAvailable)
    {
        if (!availabilityOverrideText.Equals(""))
        {
            return availabilityOverrideText;
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
            if (!popUpText.Equals(""))
            {
                FindObjectOfType<PopUpController>().AddNotification(popUpText, popUpPicture);
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
