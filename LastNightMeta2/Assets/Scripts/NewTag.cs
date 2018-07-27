using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// This component is used to spawn a "New!" tag on the game object that it's attached to.
// It also keeps track of what caused it to spawn.
public class NewTag : MonoBehaviour, IPointerEnterHandler {
    GameObject newTag;
    GameObject spawner;

	// Use this for initialization
	void Start () {
		
	}
	
    // The recently unlocked game object that caused the "New!" tag to spawn
    public void SetSpawner(GameObject newSpawner)
    {
        spawner = newSpawner;
    }

    // Spawn a "New!" tag on the game object
    public void SpawnNewTag()
    {
        newTag = Instantiate(FindObjectOfType<GameManager>().NewTag);
        newTag.transform.SetParent(gameObject.transform);
        newTag.transform.SetAsLastSibling();
        newTag.transform.localPosition = new Vector3(0, 0, 0);
        newTag.transform.localScale = new Vector3(1, 1, 1);
        newTag.SetActive(true);
    }

	// Update is called once per frame
	void Update () {
		
	}

    // Destroy the new tag the first time the player mouses over this game object.
    // Add 1 to the number of new tags spawned by the spawner, to stop it from spawning other "New!" tags.
    public void OnPointerEnter(PointerEventData eventData)
    {
        Destroy(newTag);

        spawner.GetComponent<Unlockable>().IncreaseNewTags();
    }
}
