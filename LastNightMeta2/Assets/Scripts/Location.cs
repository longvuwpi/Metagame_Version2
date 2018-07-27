using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Each location is an Unlockable.
public class Location : Unlockable {
    // Title display
    public TextMeshProUGUI locationTitle;

    Vector2 coordinates;
    string locationName;
    string neighborhoodName;

    // All activities in this location
    protected List<Activity> activities = new List<Activity>();

    // Set the content of this location component
    public void setLocation(string location_Name, string neighborhood_Name, Vector2 vector_coordinates, string unlock_condition)
    {
        locationName = location_Name;
        neighborhoodName = neighborhood_Name;
        coordinates = vector_coordinates;

        locationTitle.text = locationName;
        gameObject.transform.localPosition = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width * coordinates.x / 2, transform.parent.GetComponent<RectTransform>().rect.height * coordinates.y / 2);

        unlockCondition = unlock_condition;

        SetUnlockedOnCreate();
    }

	// Use this for initialization
	void Start () {
        //gameObject.GetComponent<Button>().onClick.AddListener(LocationClicked);
    }

    // Update is called once per frame
    void Update () {

	}

    // When this location is clicked
    public void LocationClicked()
    {
        Debug.Log("clicked " + locationName);
        foreach (Activity activity in activities)
        {
            Debug.Log(activity.activity + "\n");
        }

        // Notify the activities controller
        FindObjectOfType<WorldManager>().activitiesPanel.transform.parent.gameObject.GetComponent<ActivitiesController>().LocationClicked(this);
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
    public List<Activity> GetUnlockedActivities()
    {
        List<Activity> result = new List<Activity>();

        foreach (Activity activity in activities)
        {
            if (activity.isUnlocked())
            {
                result.Add(activity);
            }
        }

        return result;
    }
}
