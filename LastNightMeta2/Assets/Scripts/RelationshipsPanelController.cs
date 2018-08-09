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
    bool inAnim; // whether the panel and button is in the middle of expanding or collapsing

    [System.NonSerialized] float width;
    Vector3 startingRelationshipPosition = new Vector3(0, 200, 0);

    // Use this for initialization
    void Start () {
        on = false;
        inAnim = false;
        width = RelationshipsPanel.GetComponent<RectTransform>().sizeDelta.x;
        //Debug.Log("X" + width);

        //Move the relationship panel to the edge of the screen
        RelationshipsPanel.transform.localPosition += new Vector3(width, 0,0);

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// create relationships on the panel
    /// </summary>
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

    /// <summary>
    /// When the relationship button is clicked, expand the panel if not currently expanded. Else close the panel
    /// needs fixing so that the bug where both the buttons and the relationships panel moves more and more to the left stops happening
    /// </summary>
    public void buttonClicked()
    {
        if (!inAnim)
        {
            if (on)
            {
                Collapse();
            }
            else
            {
                RelationshipsButton.transform.SetAsLastSibling();
                Expand();
            }
        }
    }

    /// <summary>
    /// enable the game object, "expand" the relationship panel
    /// </summary>
    public void Expand()
    {
        gameObject.SetActive(true);
        StartCoroutine(CoExpand());
    }

    // Expand Animation
    IEnumerator CoExpand()
    {
        inAnim = true;
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
        inAnim = false;
        gameObject.transform.SetAsLastSibling();
    }

    // close the panel
    public void Collapse()
    {
        StartCoroutine(CoCollapse());
    }

    // Close Animation
    IEnumerator CoCollapse()
    {
        inAnim = true;
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
        inAnim = false;
        gameObject.SetActive(false);
    }
}
