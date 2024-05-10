using UnityEngine;
using UnityEngine.UI;

public class BackButtonController : MonoBehaviour
{
    private Button backButton;

    private void Awake()
    {
        backButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        MenuBackController.instance.backButton = backButton;

        //Debug.Log("BackButtonEnable");
        //Debug.Break();
    }

}
