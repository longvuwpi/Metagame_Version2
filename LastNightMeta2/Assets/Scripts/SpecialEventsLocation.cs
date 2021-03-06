﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialEventsLocation : Location {
    public TextMeshProUGUI availableEventsText;

    void Update()
    {
        string target = "";
        foreach (Activity activity in GetAvailableActivities())
        {
            target += activity.activity + "\n";
        }

        availableEventsText.text = target;
    }
}
