using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tutorial : MonoBehaviour
{
    protected string tutorialkey;

    private void Awake()
    {
        tutorialkey = gameObject.name;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tutorial"))
            TutorialMethod();
    }

    protected abstract void TutorialMethod();
}
