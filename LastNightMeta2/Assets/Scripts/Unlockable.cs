using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Neighborhoods, activities and locations are unlockables.
// Unlocked activities and locations have a "New!" tag on themselves.
// Unlocked neighborhoods have a "New!" tag on any arrow object that navigates to them.
public class Unlockable: MonoBehaviour {
    protected bool unlocked; //whether it's unlocked or not

    // how many "New!" tag this object has spawned and destroyed. If this object spawned a "New!" tag
    // but the player has not moused over it and the tag hasn't disappeared, the number is still 0
    protected int newTagsSpawned = 0;

    public string unlockCondition; // The unlock condition

    bool notificationPushed = false; // whether this Unlockable 

    // When the unlockable object is first created, its unlock condition will be checked.
    // If it's already unlocked at the start, it doesn't spawn "New!" tags.
    public void SetUnlockedOnCreate()
    {
        unlocked = FindObjectOfType<PlayerManager>().checkCondition(unlockCondition);

        if (unlocked)
        {
            newTagsSpawned = 1;
        }
        else
        {
            newTagsSpawned = 0;
        }
    }

    public bool isUnlocked()
    {
        return unlocked;
    }

    // Check if the unlock condition is satisfied.
    public void checkUnlockCondition()
    {
        if (!unlocked)
        {
            unlocked = FindObjectOfType<PlayerManager>().checkCondition(unlockCondition);
            // If this Unlockable is just unlocked, notify the player.
            if (unlocked)
            {
                PushNotification();

                // If this Unlockable is a Location or an Activity, spawn a new tag on self
                // If this Unlockable is a Neighborhood, the Worldmanager will spawn a new tag on the arrows that navigate to this Neighborhood.
                if (gameObject.GetComponent<Neighborhood>() == null)
                {
                    SpawnNewTagOnObject(gameObject);
                }
            }
        }
    }

    // Notify the player of what has just been unlocked
    void PushNotification()
    {
        if (!notificationPushed)
        {
            if (gameObject.GetComponent<Neighborhood>() != null)
            {
                FindObjectOfType<PopUpController>().AddNotification("New neighborhood unlocked: " + gameObject.GetComponent<Neighborhood>().getName());
            }
            else if (gameObject.GetComponent<Location>() != null)
            {
                FindObjectOfType<PopUpController>().AddNotification("New location unlocked: " + gameObject.GetComponent<Location>().getLocationName());
            }
            else if (gameObject.GetComponent<Activity>() != null)
            {
                FindObjectOfType<PopUpController>().AddNotification("New activity unlocked in " + gameObject.GetComponent<Activity>().location + ": "+ gameObject.GetComponent<Activity>().activity);
            }
            notificationPushed = true;
        }
    }

    // Spawn a "New!" tag on the specified game object
    // Also set this Unlockable to be the spawner of that "New!" tag
    public void SpawnNewTagOnObject(GameObject toSpawnOn)
    {
        toSpawnOn.AddComponent<NewTag>();
        toSpawnOn.GetComponent<NewTag>().SetSpawner(gameObject);
        toSpawnOn.GetComponent<NewTag>().SpawnNewTag();
        if (gameObject.GetComponent<Neighborhood>() != null)
        {
            foreach (Transform child in toSpawnOn.transform)
            {
                // If the "New!" tag is spawned by a Neighborhood on an Arrow object, mark it with "ArrowTag"
                // in case we need the "New!" tag to disappear when the player navigates away, and then reappear when the player navigates back
                if (child.gameObject.name.Contains("Tag"))
                {
                    child.gameObject.tag = "ArrowTag";
                }
            }
        }
    }

    // Number of new tags spawned by this Unlockable and then destroyed 
    public int getNewTagsSpawned()
    {
        return newTagsSpawned;
    }

    public void IncreaseNewTags()
    {
        newTagsSpawned += 1;
    }

    public void DecreaseNewTags()
    {
        newTagsSpawned -= 1;
    }
}
