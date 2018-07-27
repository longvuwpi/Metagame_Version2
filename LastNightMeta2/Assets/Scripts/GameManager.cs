using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// For any game logic that doesn't belong to the player or the overworld
// right now only keeps track of the time, sets the background corresponding to the current time frame
public class GameManager : MonoBehaviour {
    public TextMeshProUGUI timeText;
    public Image dayMask, nightMask;
    public GameObject NewTag;

    List<string> days = new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    List<string> timeFrames = new List<string>() { "Morning", "Afternoon", "Evening", "Night" };
    int currentDayOfWeek = 0;
    int currentDay = 1;
    int currentTimeFrame = 0;
    int totalDays = 10;

    [System.NonSerialized] Color dayColor = new Color(1, 1, 1, 0.19f);
    [System.NonSerialized] Color afternoonColor = new Color(1, 1, 1, 0.33f);
    [System.NonSerialized] Color eveningColor = new Color(1, 1, 1, 0.7f);
    [System.NonSerialized] Color nightColor = new Color(1, 1, 1, 0.85f);
    [System.NonSerialized] Color notVisible = new Color(1, 1, 1, 0);

    // Use this for initialization
    void Start () {
        nightMask.color = notVisible;
        dayMask.color = dayColor;
	}
	
	// Update is called once per frame
	void Update () {
        timeText.text = "Day " + currentDay.ToString() + " of " + totalDays.ToString() + " - " + days[currentDayOfWeek] + " " + timeFrames[currentTimeFrame];
	}

    public int GetCurrentTimeFrame()
    {
        return (currentDayOfWeek * 4 + currentTimeFrame);
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    // Move to the next correct time frame, day, day of the week
    public void IncreaseTime()
    {
        FindObjectOfType<PlayerManager>().decreaseMultiplierDuration();

        currentTimeFrame++;
        if (currentTimeFrame > 3)
        {
            currentTimeFrame = 0;
            currentDay++;
            currentDayOfWeek++;
            if (currentDayOfWeek > 6)
            {
                currentDayOfWeek = 0;
            }
        }

        switch(currentTimeFrame)
        {
            case 0:
                StartCoroutine(TurnDay());
                break;
            case 1:
                StartCoroutine(TurnAfternoon());
                break;
            case 2:
                StartCoroutine(TurnEvening());
                break;
            case 3:
                StartCoroutine(TurnNight());
                break;
        }
    }

    // Set background colors for different time of the day
    //-----------------------------------------------------

    IEnumerator TurnDay()
    {
        float perc = 0;
        while (perc <= 1)
        {
            nightMask.color = Color.Lerp(nightColor, notVisible, perc);
            dayMask.color = Color.Lerp(notVisible, dayColor, perc);
            perc += 0.05f;
            yield return null;
        }

        nightMask.color = notVisible;
        dayMask.color = dayColor;
    }

    IEnumerator TurnAfternoon()
    {
        float perc = 0;
        while (perc <= 1)
        {
            dayMask.color = Color.Lerp(dayColor, afternoonColor, perc);
            perc += 0.05f;
            yield return null;
        }

        dayMask.color = afternoonColor;
    }

    IEnumerator TurnEvening()
    {
        float perc = 0;
        while (perc <= 1)
        {
            nightMask.color = Color.Lerp(notVisible, eveningColor, perc);
            dayMask.color = Color.Lerp(afternoonColor, notVisible, perc);
            perc += 0.05f;
            yield return null;
        }

        nightMask.color = eveningColor;
        dayMask.color = notVisible;
    }

    IEnumerator TurnNight()
    {
        float perc = 0;
        while (perc <= 1)
        {
            nightMask.color = Color.Lerp(eveningColor, nightColor, perc);
            perc += 0.05f;
            yield return null;
        }

        nightMask.color = nightColor;
    }

}
