using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Each activity is an Unlockable. Attached to each Activity object
public class Activity : Unlockable {
    public Text activityText, costText, outcomeText,availabilityText;
    //Availability text only says "Unavailable". Attribute name is somewhat misleading.

    [System.NonSerialized] public string activity;
    [System.NonSerialized] public string location;
    [System.NonSerialized] public string accessibilityCondition;
    [System.NonSerialized] public string costs;
    [System.NonSerialized] public string mechanicalOutcomes;
    [System.NonSerialized] List<bool> availability;
    [System.NonSerialized] Color unavailableColor = new Color(0.3725f, 0.2f, 0.2235f);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // If it's accessible and available at the current time frame, 
		if (isAccessible() && isAvailable())
        {
            // Disable the "Unavailable" text, set card to white
            availabilityText.gameObject.SetActive(false);
            GetComponent<Image>().color = Color.white;
        } else
        {
            // else, card is unavailable
            availabilityText.gameObject.SetActive(true);
            GetComponent<Image>().color = unavailableColor;
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

        //Debug.Log(availability.Count);

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

    // Check if this activity is available at the current time frame
    public bool isAvailable()
    {
        int currentTimeFrame = FindObjectOfType<GameManager>().GetCurrentTimeFrame();
        return availability[currentTimeFrame];
    }

    // When this activity is clicked
    public void ActivityClicked()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        GameManager gameManager = FindObjectOfType<GameManager>();

        // If the activity is accessible, available, and player has enough resource to pay for it
        if (isAccessible() && isAvailable() && playerManager.checkCost(this))
        {
            // To next time frame
            gameManager.IncreaseTime();
            // Apply costs and mechanical outcomes of the activity
            playerManager.applyChanges(this);
            // Close the activities panel
            FindObjectOfType<ActivitiesController>().ClickedOutside();
        }
    }
}
