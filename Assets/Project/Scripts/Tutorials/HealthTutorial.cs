using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTutorial : Tutorial
{
    protected override void TutorialMethod()
    {
        if (PlayerPrefs.HasKey(tutorialkey))
            return;

        PlayerPrefs.SetInt(tutorialkey, 1);

        StartHealthTutorial();
    }

    private void StartHealthTutorial()
    {

    }
}
