using UnityEngine;

public class ShopExclamationController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (!QuestManager.Instance.CanCompleteSomeQuest())
            gameObject.SetActive(false);
    }
}
