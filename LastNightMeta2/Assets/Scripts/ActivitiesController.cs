using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Control the activities that are shown whenever the player clicks on a Location (including the Special Events)
public class ActivitiesController : MonoBehaviour {
    public GameObject activitiesPanel;

    public GameObject verticalScrollBar;

    Vector3 locationPosition;
    Vector3 startingActivityPosition = new Vector3(0, 100, 0);
    List<Activity> currentActivities;

    /// <summary>
    /// Called whenever a location is clicked
    /// Gets all activities that can be shown to the player from that location into the panel
    /// then activate the panel
    /// </summary>
    /// <param name="location"></param>
    public void LocationClicked(Location location)
    {
        locationPosition = location.gameObject.transform.localPosition;

        gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();

        // Get all unlocked activities from the location
        //currentActivities = location.GetUnlockedUnhiddenActivities();
        currentActivities = location.GetActivitiesToShow();

        Debug.Log(currentActivities.Count + " current activities");
        for (int i = 0; i < currentActivities.Count; i++)
        {
            currentActivities[i].gameObject.SetActive(true);
            //currentActivities[i].gameObject.transform.localPosition = startingActivityPosition - (new Vector3(0, i * 150, 0));
            Debug.Log("Activated " + currentActivities[i].activity);
        }

        StartCoroutine(ActivatePanel());
    }

    /// <summary>
    /// The panel floats from the clicked location to the middle of the screen
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivatePanel()
    {
        float perc = 0;

        while (perc <= 1)
        {
            activitiesPanel.transform.localPosition = Vector3.Lerp(locationPosition, new Vector3(0, 0, 0), perc);
            activitiesPanel.transform.localScale = new Vector3(perc, perc, 1);
            perc += 0.1f;
            yield return null;
        }

        activitiesPanel.transform.localPosition = new Vector3(0, 0, 0);
        activitiesPanel.transform.localScale = new Vector3(1, 1, 1);
        verticalScrollBar.GetComponent<Scrollbar>().value = 1;

    }

    /// <summary>
    /// When clicked outside panel, deactivate it
    /// </summary>
    public void ClickedOutside()
    {
        StartCoroutine(DeactivatePanel());
    }

    /// <summary>
    /// Reverse the activate animation
    /// </summary>
    /// <returns></returns>
    IEnumerator DeactivatePanel()
    {
        float perc = 0;

        while (perc <= 1)
        {
            activitiesPanel.transform.localPosition = Vector3.Lerp(new Vector3(0, 0, 0), locationPosition, perc);
            activitiesPanel.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0,0,1), perc);
            perc += 0.1f;
            yield return null;
        }

        foreach (Activity activity in currentActivities)
        {
            activity.gameObject.SetActive(false);
            Debug.Log("Deactivated " + activity.activity);
        }

        currentActivities.Clear();

        gameObject.SetActive(false);

    }

    // Use this for initialization
    void Start () {
        currentActivities = new List<Activity>();
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
