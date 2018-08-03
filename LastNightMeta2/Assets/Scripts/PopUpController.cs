using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages notifying the player whenever something gets unlocked
public class PopUpController : MonoBehaviour {
    public GameObject notification;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    List<GameObject> notificationObjects = new List<GameObject>();
    GameObject currentIn;
    List<string> notifications = new List<string>();
    List<string> popUpImages = new List<string>();

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
                //if (currentIn == null)
                //{
                //    currentIn = notificationObjects[0];
                //    notificationObjects.RemoveAt(0);
                //    StartCoroutine(LerpIn(currentIn));
                //}
                //else if (!currentIn.Equals(notificationObjects[notificationObjects.Count-1]))
                //{
                //    int index = notificationObjects.IndexOf(currentIn);
                //    currentIn = notificationObjects[index + 1];
                //    StartCoroutine(LerpIn(currentIn));
                //}
                Debug.Log("count " + notificationObjects.Count);
                currentIn = notificationObjects[0];
                Debug.Log(currentIn.GetComponentInChildren<Text>().text);
                notificationObjects.RemoveAt(0);
                Debug.Log("after remove " + currentIn.GetComponentInChildren<Text>().text);
                StartCoroutine(LerpIn(currentIn));
            }
        }
        else
        {
            if (!animOut)
            {
                // Once everything has finished sliding in and out, hide the pop up panel
                gameObject.transform.SetAsFirstSibling();
            }
        }
	}

    // Notification slides in from out of the screen
    IEnumerator LerpIn(GameObject notiClone)
    {
        

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
        //animIn = false;
    }

    // Once clicked, the current notification will slide out and another notification will slide in (if there's any left)
    public void NotificationClicked()
    {

        StopCoroutine(LerpIn(currentIn));
        currentIn.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        StartCoroutine(LerpOut(currentIn));

        animIn = false;

        Debug.Log("clicked");
        Debug.Log(animIn ? "still in anim" : "not in anim");
        Debug.Log("notifications count " + notifications.Count);
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

    // Add a notification 
    public void AddNotification(string newNoti)
    {
        AddNotification(newNoti, "");
    }

    public void AddNotification(string newNoti, string popUpImage)
    {
        // Slide a notification in if nothing is sliding in yet
        GameObject notiClone = Instantiate(notification);
        notiClone.GetComponentInChildren<Text>().text = newNoti;
        notiClone.transform.SetParent(gameObject.transform);
        //notiClone.transform.SetAsFirstSibling();
        notiClone.transform.localScale = new Vector3(1, 1, 1);
        notiClone.transform.localPosition = startNotiPos;
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
            notiClone.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/PopUpImages" + popUpImage);
        }
        notiClone.SetActive(true);

        notificationObjects.Add(notiClone);
        Debug.Log("Added " + newNoti);
        //StartCoroutine(LerpIn(notiClone));


    }

}
