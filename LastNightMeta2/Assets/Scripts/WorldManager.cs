using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.EventSystems;

// WorldManager lets us select which spreadsheet to use in the beginning. Also keeps track of all neighborhoods, all locations, 
// special events and where the player currently is.
public class WorldManager : MonoBehaviour {
    public Image background;
    public TextMeshProUGUI neighborhoodTitle;
    public Button Left, Right, Up, Down;
    public GameObject locationPrefab;
    public GameObject activityPrefab;
    public GameObject activitiesPanel;
    public GameObject activitiesContent;
    public GameObject spreadsheetSelection;
    public SpecialEventsLocation specialEvents;

    string spreadsheet_primary = "https://docs.google.com/spreadsheets/d/1VO0ETqqW03WUyQsbS5gwJcpyELVSgZ0qz7JuGqfVTh0/export?format=csv&gid=";
    string spreadsheet_new = "https://docs.google.com/spreadsheets/d/1Y8yyIOYjkCv6R-TGaAHU85d-J1VBes0gKGYdbXCldyI/export?format=csv&gid=";
    string spreadsheet_ichiro = "https://docs.google.com/spreadsheets/d/15afgZ3kx4NzncckWuEOtIY8cUNWiWRihqsbKZagSYS0/export?format=csv&gid=";
    string spreadsheet_hillqueen = "https://docs.google.com/spreadsheets/d/1G33taEH2jayLIpM6-soOe514F2U3AaJYvE0GZNjJOgg/export?format=csv&gid=";
    string neighborhood_gid = "742526018";
    string location_gid = "1254739945";
    string activities_gid = "0";
    bool spreadsheet_selected = false;
    string spreadsheet;

    Dictionary<Vector2, Neighborhood> neighborhoodMap;
    Vector2 currentNeighborhoodPosition;
    List<Location> allLocations;

    Vector2 upDirection = new Vector2(0, -1);
    Vector2 downDirection = new Vector2(0, 1);
    Vector2 leftDirection = new Vector2(-1, 0);
    Vector2 rightDirection = new Vector2(1, 0);

    // Use this for initialization
    void Start () {
        // Disable direction buttons
        Up.gameObject.SetActive(false);
        Down.gameObject.SetActive(false);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);

        // Get neighborhoods from the spreadsheet
        StartCoroutine(GrabNeighborhoods());
	}
	
    //Determine which spreadsheet is used. Primary Metadata button and Ichiro Metadata button will call this
    public void SpreadsheetSelected(bool isPrimary)
    {
        if (isPrimary)
        {
            spreadsheet = spreadsheet_hillqueen;
            FindObjectOfType<PopUpController>().AddNotification("Test1", "mulchtea01");
            FindObjectOfType<PopUpController>().AddNotification("Test2", "drones01");
        }
        else
        {
            spreadsheet = spreadsheet_ichiro;
        }

        spreadsheet_selected = true;
        Destroy(spreadsheetSelection);
    }

    // Pull neighborhoods
    IEnumerator GrabNeighborhoods()
    {
        // wait until a spreadsheet is selected
        while (!spreadsheet_selected)
        {
            yield return null;
        }

        // Download the neighborhood tab from the spreadsheet
        UnityWebRequest wr = UnityWebRequest.Get(spreadsheet + neighborhood_gid);
        yield return wr.SendWebRequest();
        while (!wr.downloadHandler.isDone)
        {
            yield return null;
        }

        Debug.Log("Download done");
        string fileBody = wr.downloadHandler.text;
        Debug.Log(fileBody);

        // Split by line. each line is a neighborhood
        string[] dataSplit = fileBody.Split('\n');
        List<string> neighborhoods = new List<string>(dataSplit);
        // Remove the header line
        neighborhoods.RemoveAt(0);

        neighborhoodMap = new Dictionary<Vector2, Neighborhood>();

        // For each line, create a neighborhood object, attach the neighborhood script to it and set the data
        foreach (string neighborhoodString in neighborhoods)
        {
            string[] contentSplit = neighborhoodString.Split(',');
            string neighbor_name = contentSplit[0].Trim();
            int x_coord = int.Parse(contentSplit[1].Trim());
            int y_coord = int.Parse(contentSplit[2].Trim());
            string unlock_condition = contentSplit[3].Trim();

            Vector2 position = new Vector2(x_coord, y_coord);
            GameObject newNeighborhoodObject = new GameObject();
            newNeighborhoodObject.AddComponent<Neighborhood>();
            newNeighborhoodObject.GetComponent<Neighborhood>().SetNeighborhood(position, neighbor_name, unlock_condition);

            // Add the created neighborhood to the dictionary
            neighborhoodMap.Add(position, newNeighborhoodObject.GetComponent<Neighborhood>());
        }

        // After getting the neighborhoods, pull the locations from spreadsheet
        StartCoroutine(GetLocations());
        SetCurrentNeighborhood(new Vector2(0, 0));
    }

    IEnumerator GetLocations()
    {
        // Download the locations tab from the spreadsheet
        UnityWebRequest wr = UnityWebRequest.Get(spreadsheet + location_gid);
        yield return wr.SendWebRequest();
        while (!wr.downloadHandler.isDone)
        {
            yield return null;
        }

        Debug.Log("Download done");
        string fileBody = wr.downloadHandler.text;
        Debug.Log(fileBody);

        // Split by line. Each line is a location
        string[] dataSplit = fileBody.Split('\n');

        allLocations = new List<Location>();

        List<string> locations = new List<string>(dataSplit);
        // Remove the header line
        locations.RemoveAt(0);

        // For each line, create a location object and set the data
        foreach(string location_string in locations)
        {
            GameObject newLocation = Instantiate(locationPrefab);
            newLocation.transform.SetParent(gameObject.transform);
            newLocation.transform.localScale = new Vector3(1, 1, 1);

            string[] contentSplit = location_string.Split(',');

            string location_name = contentSplit[0].Trim();
            string neighborhood_name = contentSplit[1].Trim();
            Vector2 coordinates = new Vector2(float.Parse(contentSplit[2].Trim()), float.Parse(contentSplit[3].Trim()));
            string unlock_condition = contentSplit[4].Trim();
            newLocation.GetComponent<Location>().setLocation(location_name, neighborhood_name, coordinates, unlock_condition);

            // Keep track of all the locations.
            allLocations.Add(newLocation.GetComponent<Location>());

            // Add the location to the correct neighborhood
            foreach (Neighborhood neighborhood in neighborhoodMap.Values)
            {
                if (neighborhood.getName().Equals(neighborhood_name))
                {
                    neighborhood.AddLocation(newLocation.GetComponent<Location>());
                    break;
                }
            }
        }

        // Pull activities last
        StartCoroutine(GetActivities());
    }

    IEnumerator GetActivities()
    {
        // Download the activities tab on the spreadsheet
        UnityWebRequest wr = UnityWebRequest.Get(spreadsheet + activities_gid);
        yield return wr.SendWebRequest();
        while (!wr.downloadHandler.isDone)
        {
            yield return null;
        }

        Debug.Log("Download done");
        string fileBody = wr.downloadHandler.text;
        Debug.Log(fileBody);
        
        // Split by line. Each line is an activity
        string[] dataSplit = fileBody.Split('\n');
        List<string> activities = new List<string>(dataSplit);
        
        // Remove the header lines
        activities.RemoveRange(0, 29);

        // For each line, create a new activity object, set the activity content
        foreach (string activity_string in activities)
        {
            GameObject newActivityObject = Instantiate(activityPrefab);
            //newActivityObject.transform.SetParent(activitiesPanel.transform);
            newActivityObject.transform.SetParent(activitiesContent.transform);
            newActivityObject.transform.localPosition = new Vector3(0, 0, 0);
            newActivityObject.transform.localScale = new Vector3(1, 1, 1);

            Activity newActivity = newActivityObject.GetComponent<Activity>();

            newActivity.SetContent(activity_string);

            // Add the created activity to the correct location
            if (newActivity.location.Equals("Random"))
            {
                specialEvents.AddActivity(newActivity);
            }
            else
            {
                foreach (Location location in allLocations)
                {

                    if (location.getLocationName().Equals(newActivity.location))
                    {
                        location.AddActivity(newActivity);
                        break;
                    }
                }
            }

            newActivityObject.SetActive(false);
        }
    }

    // Move to the neighborhood with the specified (x,y) coordinates
    void SetCurrentNeighborhood(Vector2 newPosition)
    {
        EventSystem.current.SetSelectedGameObject(null);

        // If there are "New!" tags on the arrows, remove them. They will need to be spawned again when the player navigates back
        // to the previous neighborhood.
        foreach (GameObject tag in GameObject.FindGameObjectsWithTag("ArrowTag"))
        {
            Destroy(tag);
        }
        Destroy(Up.GetComponent<NewTag>());
        Destroy(Down.GetComponent<NewTag>());
        Destroy(Left.GetComponent<NewTag>());
        Destroy(Right.GetComponent<NewTag>());

        // Set the neighborhood name, background, and the current position
        Neighborhood toSet = neighborhoodMap[newPosition];
        string name = toSet.getName();
        neighborhoodTitle.text = name;
        background.sprite = Resources.Load<Sprite>("Images/" + name);
        currentNeighborhoodPosition = newPosition;
    }

    // Update is called once per frame
    void Update () {
        RunThroughWorld();
        CheckNeighbors();
	}

    // Check the unlock conditions of everything in the world (neighborhoods, locations and activities)
    void RunThroughWorld()
    {
        if (neighborhoodMap != null)
        {
            foreach (Neighborhood neighborhood in neighborhoodMap.Values)
            {
                neighborhood.checkUnlockCondition();
            }
        }

        if (allLocations != null)
        {
            foreach (Location location in allLocations)
            {
                location.checkUnlockCondition();

                if (location.isUnlocked())
                {
                    foreach (Activity activity in location.GetActivities())
                    {
                        activity.checkUnlockCondition();
                    }
                }
            }
        }
    }

    // Call Update() on each neighborhood
    // Not needed anymore since Neighborhoods were made MonoBehaviours for the Locked/Unlocked functionality.
    void UpdateNeighborhoods()
    {
        if (neighborhoodMap != null)
        {
            foreach (Neighborhood neighborhood in neighborhoodMap.Values)
            {
                neighborhood.Update();
            }
        }
    }

    // Check adjacent neighborhoods. Enable arrow buttons if there is a neighborhood to navigate to in the corresponding direction.
    // Also spawn "New!" tags on the arrow button if the adjacent neighborhood has just been unlocked.
    void CheckNeighbors()
    {
        if (neighborhoodMap != null)
        {
            Vector2 upPosition = currentNeighborhoodPosition + upDirection;
            Vector2 downPosition = currentNeighborhoodPosition + downDirection;
            Vector2 leftPosition = currentNeighborhoodPosition + leftDirection;
            Vector2 rightPosition = currentNeighborhoodPosition + rightDirection;

            if (neighborhoodMap.ContainsKey(upPosition) && neighborhoodMap[upPosition].isUnlocked())
            {
                Up.gameObject.SetActive(true);
                if ((neighborhoodMap[upPosition].getNewTagsSpawned() == 0) && (Up.gameObject.GetComponentInChildren<NewTag>() == null)) {
                    neighborhoodMap[upPosition].SpawnNewTagOnObject(Up.gameObject);
                }
            } else
            {
                Up.gameObject.SetActive(false);
            }

            if (neighborhoodMap.ContainsKey(downPosition) && neighborhoodMap[downPosition].isUnlocked())
            {
                Down.gameObject.SetActive(true);
                if ((neighborhoodMap[downPosition].getNewTagsSpawned() == 0) && (Down.gameObject.GetComponentInChildren<NewTag>() == null))
                {
                    neighborhoodMap[downPosition].SpawnNewTagOnObject(Down.gameObject);
                }
            }
            else
            {
                Down.gameObject.SetActive(false);
            }
            
            if (neighborhoodMap.ContainsKey(leftPosition) && neighborhoodMap[leftPosition].isUnlocked())
            {
                Left.gameObject.SetActive(true);
                if ((neighborhoodMap[leftPosition].getNewTagsSpawned() == 0) && (Left.gameObject.GetComponentInChildren<NewTag>() == null))
                {
                    neighborhoodMap[leftPosition].SpawnNewTagOnObject(Left.gameObject);
                }
            }
            else
            {
                Left.gameObject.SetActive(false);
            }

            if (neighborhoodMap.ContainsKey(rightPosition) && neighborhoodMap[rightPosition].isUnlocked())
            {
                Right.gameObject.SetActive(true);
                if ((neighborhoodMap[rightPosition].getNewTagsSpawned() == 0) && (Right.gameObject.GetComponentInChildren<NewTag>() == null))
                {
                    neighborhoodMap[rightPosition].SpawnNewTagOnObject(Right.gameObject);
                }
            }
            else
            {
                Right.gameObject.SetActive(false);
            }
        }
    }

    // Called when Up button is clicked
    public void GoUp()
    {
        SetCurrentNeighborhood(currentNeighborhoodPosition + upDirection);
    }

    // Called when Down button is clicked
    public void GoDown()
    {
        SetCurrentNeighborhood(currentNeighborhoodPosition + downDirection);
    }

    // Called when Left button is clicked
    public void GoLeft()
    {
        SetCurrentNeighborhood(currentNeighborhoodPosition + leftDirection);
    }

    // Called when Right button is clicked
    public void GoRight()
    {
        SetCurrentNeighborhood(currentNeighborhoodPosition + rightDirection);
    }

    // Return the current neighborhood's coordinates
    public Vector2 getCurrentNeighborhoodPosition()
    {
        return currentNeighborhoodPosition;
    }
}
