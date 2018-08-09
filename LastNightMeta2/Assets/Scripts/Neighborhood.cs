using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Each neighborhood is an Unlockable. It also stores its coordinates, the name and its locations.
public class Neighborhood: Unlockable {
    Vector2 position;
    string neighborhoodName;
    public string background;
    List<Location> locations;
    
    /// <summary>
    /// Set the data of this neighborhood component
    /// Same as Location, should move the string formatting logic here instead of inside WorldManager
    /// </summary>
    /// <param name="newPosition"></param>
    /// <param name="newName"></param>
    /// <param name="unlock_condition"></param>
    public void SetNeighborhood(Vector2 newPosition, string newName, string unlock_condition, string background_name)
    {
        position = newPosition;
        neighborhoodName = newName;
        locations = new List<Location>();
        unlockCondition = unlock_condition;
        background = background_name;
        SetUnlockedOnCreate();

    }

    public Vector2 getPosition()
    {
        return position;
    }

    public string getName()
    {
        return neighborhoodName;
    }

    // Add a location to this neighborhood
    public void AddLocation(Location newLocation)
    {
        locations.Add(newLocation);
    }

    // Each update, enable or disable its own locations, based on whether it is the current neighborhood.
    // Could be changed/improved by putting this functionality in WorldManager.SetCurrentNeighborhood, so that it's not executed every frame
    public void Update()
    {
        WorldManager worldManager = GameObject.FindObjectOfType<WorldManager>();
        if (position.Equals(worldManager.getCurrentNeighborhoodPosition()))
        {
            foreach(Location location in locations)
            {
                if (!location.isHidden())
                {
                    location.gameObject.SetActive(true);
                } else
                {
                    location.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (Location location in locations)
            {
                location.gameObject.SetActive(false);
            }
        }
    }
}
