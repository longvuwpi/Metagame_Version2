using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Control the relationships panel
public class RelationshipsPanelController : MonoBehaviour {
    public GameObject RelationshipsPanel;
    public GameObject RelationshipPrefab;
    public Button RelationshipsButton;
    public GameObject buttonArrow;

    bool on; // The state, on means the panel is being shown. Off means it's not shown

    [System.NonSerialized] float width;
    Vector3 startingRelationshipPosition = new Vector3(0, 200, 0);

    // Use this for initialization
    void Start () {
        on = false;
        width = RelationshipsPanel.GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log("X" + width);
        RelationshipsPanel.transform.localPosition += new Vector3(width, 0,0);
        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
		
	}

    // Create relationship objects in the panel
    public void PopulateRelationships()
    {
        int i = 0;
        foreach (KeyValuePair<string, int> relationship in FindObjectOfType<PlayerManager>().GetRelationships())
        {
            GameObject newRelationshipObject = Instantiate(RelationshipPrefab);
            newRelationshipObject.transform.SetParent(RelationshipsPanel.transform);
            newRelationshipObject.transform.localScale = new Vector3(1, 1, 1);
            newRelationshipObject.transform.localPosition = startingRelationshipPosition - new Vector3(0, i * 80, 0);

            newRelationshipObject.GetComponent<Relationship>().SetRelationship(relationship.Key);

            newRelationshipObject.gameObject.SetActive(true);
            i++;
        }

    }

    // When the relationship button is clicked, expand the panel if not currently expanded. Else close the panel
    public void buttonClicked()
    {
        if (on)
        {
            Collapse();
        } else
        {
            RelationshipsButton.transform.SetAsLastSibling();
            Expand();
        }
    }

    public void Expand()
    {
        gameObject.SetActive(true);
        StartCoroutine(CoExpand());
    }

    // Expand Animation
    IEnumerator CoExpand()
    {
        Debug.Log("Width " + width);

        float perc = 0;
        Vector3 originalPanelPosition = RelationshipsPanel.transform.localPosition;
        Vector3 originalButtonPosition = RelationshipsButton.transform.localPosition;
        Vector3 targetPanelPosition = RelationshipsPanel.transform.localPosition - new Vector3(width, 0, 0);
        Vector3 targetButtonPosition = originalButtonPosition - new Vector3(width, 0, 0);
        Quaternion targetArrowRotation = Quaternion.Euler(0, 180, 0);

        Debug.Log("Button from " + originalButtonPosition.x + " to " + targetButtonPosition.x);

        while (perc <= 1)
        {
            RelationshipsPanel.transform.localPosition = Vector3.Lerp(originalPanelPosition, targetPanelPosition, perc);
            //RelationshipsButton.transform.localPosition = Vector3.Lerp(originalButtonPosition, targetButtonPosition, perc);
            RelationshipsButton.transform.localPosition = Vector3.Lerp(originalButtonPosition, targetButtonPosition, perc);
            buttonArrow.transform.localRotation = Quaternion.Slerp(Quaternion.identity, targetArrowRotation, perc);

            perc += 0.05f;
            yield return null;
        }

        RelationshipsPanel.transform.localPosition = targetPanelPosition;
        RelationshipsButton.transform.localPosition = targetButtonPosition;
        buttonArrow.transform.localRotation = targetArrowRotation;
        on = true;
        gameObject.transform.SetAsLastSibling();
    }

    public void Collapse()
    {
        StartCoroutine(CoCollapse());
    }

    // Close Animation
    IEnumerator CoCollapse()
    {

        float perc = 0;
        Vector3 originalPanelPosition = RelationshipsPanel.transform.localPosition;
        Vector3 originalButtonPosition = RelationshipsButton.transform.localPosition;
        Vector3 targetPanelPosition = RelationshipsPanel.transform.localPosition + new Vector3(width, 0, 0);
        Vector3 targetButtonPosition = RelationshipsButton.transform.localPosition + new Vector3(width, 0, 0);
        Quaternion targetArrowRotation = Quaternion.Euler(0, 180, 0);

        while (perc <= 1)
        {
            RelationshipsPanel.transform.localPosition = Vector3.Lerp(originalPanelPosition, targetPanelPosition, perc);
            RelationshipsButton.transform.localPosition = Vector3.Lerp(originalButtonPosition, targetButtonPosition, perc);
            buttonArrow.transform.localRotation = Quaternion.Slerp(targetArrowRotation, Quaternion.identity, perc);

            perc += 0.05f;
            yield return null;
        }

        RelationshipsPanel.transform.localPosition = targetPanelPosition;
        RelationshipsButton.transform.localPosition = targetButtonPosition;
        buttonArrow.transform.localRotation = Quaternion.identity;
        on = false;
        gameObject.SetActive(false);
    }
}
