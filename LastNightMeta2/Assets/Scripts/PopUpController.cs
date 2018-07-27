using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages notifying the player whenever something gets unlocked
public class PopUpController : MonoBehaviour {
    public GameObject notification;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
    List<string> notifications = new List<string>();
    Vector3 startNotiPos = new Vector2(0, -600);
    Vector3 endNotiPos = new Vector2(0, 600);
    bool animIn = false;
    bool animOut = false;
    float halfLerpTime = 0.5f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // If there are still queued notifications left
        if (notifications.Count > 0)
        {
            gameObject.transform.SetAsLastSibling();
            if (!animIn)
            {
                // Slide a notification in if nothing is sliding in yet
                GameObject notiClone = Instantiate(notification);
                notiClone.GetComponentInChildren<Text>().text = notifications[0];
                StartCoroutine(LerpIn(notiClone));
                notifications.RemoveAt(0);
                animIn = true;
            }
        }
        else
        {
            if ((!animIn) && (!animOut))
            {
                // Once everything has finished sliding in and out, hide the pop up panel
                gameObject.transform.SetAsFirstSibling();
            }
        }
	}

    // Notification slides in from out of the screen
    IEnumerator LerpIn(GameObject notiClone)
    {
        notiClone.transform.SetParent(gameObject.transform);
        notiClone.transform.SetAsFirstSibling();
        notiClone.transform.localScale = new Vector3(1, 1, 1);
        notiClone.transform.localPosition = startNotiPos;
        notiClone.SetActive(true);

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
        //animIn = false;
    }

    // Once clicked, the current notification will slide out and another notification will slide in (if there's any left)
    public void NotificationClicked()
    {
        Debug.Log("clicked");
        animIn = false;
        //Debug.Log(transform.GetChild(0).gameObject.name);
        //StopAllCoroutines();
        StartCoroutine(LerpOut(transform.GetChild(0).gameObject));
    }

    // Notification slides out of the screen
    IEnumerator LerpOut(GameObject notiClone)
    {
        Debug.Log("Started");
        animOut = true;
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
        animOut = false;
    }

    // Add a notification 
    public void AddNotification(string newNoti)
    {
        notifications.Add(newNoti);
    }
}
