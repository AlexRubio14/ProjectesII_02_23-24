using TMPro;
using UnityEngine;

public class TpNameTrigger : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textTp;

    [SerializeField]
    private float fadeTime;
    private float currentFadeTime;

    private bool playerInTrigger;

    private SelectTpController selectTp;
    private void Awake()
    {
        textTp.text = " ";
        playerInTrigger = false;
        currentFadeTime = 0;
        selectTp = GetComponent<SelectTpController>();
    }

    private void Update()
    {
        FadeText();
    }

    private void FadeText()
    {
        if (playerInTrigger && currentFadeTime < fadeTime)
        {
            currentFadeTime += Time.deltaTime;
            textTp.color = new Color(textTp.color.r, textTp.color.g, textTp.color.b, currentFadeTime);
        }
        else if (!playerInTrigger && currentFadeTime > 0)
        {
            currentFadeTime -= Time.deltaTime;
            textTp.color = new Color(textTp.color.r, textTp.color.g, textTp.color.b, currentFadeTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            textTp.text = selectTp.tp.zoneName;
            playerInTrigger = true;
            currentFadeTime = 0; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            currentFadeTime = fadeTime;
        }
    }
}
