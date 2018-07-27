using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For the relationship objects displayed in the Relationships Panel
public class Relationship : MonoBehaviour {
    public Text personName;
    public Text progress;
    public GameObject progressMask;

    float maxLength = 509.13f;
    string relationshipName;

    public void SetRelationship(string name)
    {
        relationshipName = name;
        personName.text = name;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Set the correct text and the correct size of the progress bar based on the relationship value
        int value = FindObjectOfType<PlayerManager>().GetRelationships()[relationshipName];
        float y = progressMask.GetComponent<RectTransform>().sizeDelta.y;
        progressMask.GetComponent<RectTransform>().sizeDelta = new Vector2(maxLength * value / 100, y);

        progress.text = value + "%";
    }
}
