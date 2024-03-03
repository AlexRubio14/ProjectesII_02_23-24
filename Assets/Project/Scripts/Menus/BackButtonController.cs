using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButtonController : MonoBehaviour
{
    private Button backButton;

    private void Start()
    {
        backButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        StartCoroutine(AddButton());
    }


    private IEnumerator AddButton()
    {
        yield return new WaitForEndOfFrame();
        MenuBackController.instance.backButton = backButton;
    }
}
