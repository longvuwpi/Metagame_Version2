using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Each location is an unlockable.
/// When locked, it shows nothing when clicked.
/// Will need to be expanded to allow for mouseover on locations, when they're locked, to show the Lock Override Message
/// which tells the player what to do to unlock the location (and probably what is special about the location that makes
/// it appealing)
/// </summary>
public class Location : Unlockable {
    // Title display
    public TextMeshProUGUI locationTitle;

    Vector2 coordinates;
    string locationName;
    string neighborhoodName;
    string hideCondition;

    bool hidden;

    // All activities in this location
    protected List<Activity> activities = new List<Activity>();

    /// <summary>
    /// Set the content of this location component
    /// Should move the string splitting logic here (now it's in WorldManager) so that this method accepts the full string 
    /// and handle the format of the string inside the class
    /// </summary>
    /// <param name="location_Name"></param>
    /// <param name="neighborhood_Name"></param>
    /// <param name="vector_coordinates"></param>
    /// <param name="unlock_condition"></param>
    /// <param name="hide_condition"></param>
    public void setLocation(string location_Name, string neighborhood_Name, Vector2 vector_coordinates, string unlock_condition, string hide_condition)
    {
        locationName = location_Name;
        neighborhoodName = neighborhood_Name;
        coordinates = vector_coordinates;

        locationTitle.text = locationName;
        gameObject.transform.localPosition = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width * coordinates.x / 2, transform.parent.GetComponent<RectTransform>().rect.height * coordinates.y / 2);

        unlockCondition = unlock_condition;

        hideCondition = hide_condition;

        SetUnlockedOnCreate();
    }

	// Use this for initialization
	void Start () {
        //gameObject.GetComponent<Button>().onClick.AddListener(LocationClicked);
    }

    // Update is called once per frame
    void Update () {
	}

    /// <summary>
    /// When unlocked, location have a different highlighted color, and also a different icon
    /// </summary>
    override
    protected void ToDoWhenUnlocked()
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/location");
        ColorBlock colors = GetComponent<Button>().colors;
        colors.highlightedColor = Color.black;
        GetComponent<Button>().colors = colors;
    }

    /// <summary>
    /// Check if the location is hidden
    /// </summary>
    /// <returns></returns>
    public bool isHidden()
    {
        hidden = FindObjectOfType<PlayerManager>().checkHiddenCondition(hideCondition);

        return hidden;
    }

    /// <summary>
    /// When the location is clicked, if it's unlocked it sends appropriate activities to the Activities Panel for displaying
    /// </summary>
    public void LocationClicked()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("clicked " + locationName);
        foreach (Activity activity in activities)
        {
            Debug.Log(activity.activity + "\n");
        }

        if (unlocked)
        {
            // Notify the activities controller
            FindObjectOfType<WorldManager>().activitiesPanel.transform.parent.gameObject.GetComponent<ActivitiesController>().LocationClicked(this);
        }
    }

    public Vector2 getCoordinates()
    {
        return coordinates;
    }

    public string getLocationName()
    {
        return locationName;
    }

    public string getNeighborhoodName()
    {
        return neighborhoodName;
    }

    // Add an activity to the list
    public void AddActivity(Activity newActivity)
    {
        activities.Add(newActivity);
    }

    // Get all activities in this location
    public List<Activity> GetActivities()
    {
        return activities;
    }

    // Get all available activities in this location
    public List<Activity> GetAvailableActivities()
    {
        List<Activity> result = new List<Activity>();

        foreach (Activity activity in activities)
        {
            if (activity.isAvailable())
            {
                result.Add(activity);
            }
        }

        return result;
    }

    // Get all unlocked activities in this location
    public List<Activity> GetUnlockedUnhiddenActivities()
    {
        List<Activity> result = new List<Activity>();

        foreach (Activity activity in activities)
        {
            if (activity.isUnlocked() && (!activity.isHidden()))
            {
                result.Add(activity);
            }
        }

        return result;
    }

    /// <summary>
    /// Get activities to show
    /// In the 08/07 iteration of the metagame tutorial, we decided to show both locked and unlocked activities
    /// except for the hidden activities
    /// </summary>
    /// <returns></returns>
    public List<Activity> GetActivitiesToShow()
    {
        List<Activity> result = new List<Activity>();

        foreach (Activity activity in activities)
        {
            if (!activity.isHidden())
            {
                result.Add(activity);
            }
        }

        return result;
    }
}
