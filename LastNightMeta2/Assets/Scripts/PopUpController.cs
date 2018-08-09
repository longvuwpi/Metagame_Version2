using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages notifying the player whenever something gets unlocked
public class PopUpController : MonoBehaviour {
    public GameObject notification;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    // it's a FIFO queue of notifications
    // If list is not empty, the first notification in the list will show up and be removed from the list
    List<GameObject> notificationObjects = new List<GameObject>();

    GameObject currentIn;

    [System.NonSerialized] Vector3 startNotiPos = new Vector2(0, -800);
    [System.NonSerialized] Vector3 endNotiPos = new Vector2(0, 800);
    bool animIn = false;
    bool animOut = false;
    float halfLerpTime = 0.5f;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        // If there are still queued notifications left
        if (notificationObjects.Count > 0)
        {
            gameObject.transform.SetAsLastSibling();
            if (!animIn)
            {
                // the first notification is removed from the list and slides in
                currentIn = notificationObjects[0];
                notificationObjects.RemoveAt(0);
                StartCoroutine(LerpIn(currentIn));
            }
        }
        else
        {
            if ((!animOut) && (!animIn))
            {
                // Once everything has finished sliding in and out, hide the pop up panel
                gameObject.transform.SetAsFirstSibling();
            }
        }
	}

    // Notification slides in from out of the screen
    IEnumerator LerpIn(GameObject notiClone)
    {
        Debug.Log("Started " + notiClone.GetComponentInChildren<Text>().text);

        animIn = true;
        float time = 0;
        float perc = 0;
        float firstTime = Time.realtimeSinceStartup;

        while (time <= halfLerpTime)
        {
            time = Time.realtimeSinceStartup - firstTime;
            perc = Mathf.Clamp01(time/halfLerpTime);
            Vector3 tempPos = Vector3.Lerp(startNotiPos, new Vector3(0, 0, 0), inCurve.Evaluate(perc));
            notiClone.transform.localPosition = tempPos;
            yield return null;
        }

        notiClone.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Once clicked, the current notification will slide out and another notification will slide in (if there's any left)
    public void NotificationClicked()
    {

        StopCoroutine(LerpIn(currentIn));
        currentIn.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        StartCoroutine(LerpOut(currentIn));

        animIn = false;
    }

    // Notification slides out of the screen
    IEnumerator LerpOut(GameObject notiClone)
    {
        animOut = true;

        Debug.Log("Started");
        float time = 0;
        float perc = 0;
        float firstTime = Time.realtimeSinceStartup;

        while (time <= halfLerpTime)
        {
            time = Time.realtimeSinceStartup - firstTime;
            perc = Mathf.Clamp01(time/halfLerpTime);
            Vector3 tempPos = Vector3.Lerp(new Vector3(0,0,0), endNotiPos, outCurve.Evaluate(perc));
            notiClone.transform.localPosition = tempPos;

            yield return null;
        }

        Destroy(notiClone);
        animOut = animIn;
    }

    // Add a notification without a pop up image
    public void AddNotification(string newNoti)
    {
        AddNotification(newNoti, "");
    }

    // Add a notification with the specified pop up image
    public void AddNotification(string newNoti, string popUpImage)
    {
        // Create a new notification
        GameObject notiClone = Instantiate(notification);
        // Set the text
        notiClone.GetComponentInChildren<Text>().text = newNoti;
        notiClone.transform.SetParent(gameObject.transform);
        //notiClone.transform.SetAsFirstSibling();
        notiClone.transform.localScale = new Vector3(1, 1, 1);
        notiClone.transform.localPosition = startNotiPos;
        
        // Set the pop up image
        if (popUpImage.Equals(""))
        {
            foreach (Image image in notiClone.GetComponentsInChildren<Image>())
            {
                if (image.gameObject.name == "Image")
                {
                    Destroy(image.gameObject);
                }
            }
        }
        else
        {
            foreach (Image image in notiClone.GetComponentsInChildren<Image>())
            {
                if (image.gameObject.name == "Image")
                {
                    image.sprite = Resources.Load<Sprite>("Images/PopUpImages/" + popUpImage);
                }
            }
        }
        notiClone.SetActive(true);

        notificationObjects.Add(notiClone);
        Debug.Log("Added " + newNoti);

    }

}
