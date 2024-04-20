using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AYellowpaper.SerializedCollections;
using UnityEngine.InputSystem;
using System.Xml;

public class QuestCanvasController : MonoBehaviour
{
    [Header("Input"), SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;

    [Space, Header("Quest Canvas"), SerializeField]
    private GameObject needImagePrefab;
    [SerializeField]
    private GameObject needTextPrefab;

    [Space, SerializeField]
    private TextMeshProUGUI questTitleText;
    [SerializeField]
    private LayoutGroup layoutQuest;

    [Space, SerializeField]
    private GameObject noSelectecQuestText;

    [Space, SerializeField]
    private GameObject canCompleteQuestText;

    private List<Image> prizeImages;
    private List<TextMeshProUGUI> prizeTexts;

    [Space, SerializeField]
    private Color defaultTextColor;
    [SerializeField]
    private Color doneTextColor;

    private QuestObject currentQuest;

    [Space, SerializeField]
    private float timeToWaitForFloat;



    // Start is called before the first frame update
    void Start()
    {
        prizeImages = new List<Image>();
        prizeTexts = new List<TextMeshProUGUI>();

        SetupQuestCanvas(QuestManager.Instance.GetSelectedQuest());
    }

    private void OnEnable()
    {
        InventoryManager.Instance.obtainItemAction += UpdateQuestCanvas;
        InputSystem.onDeviceChange += UpdateInputImages;
        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);
    }

    private void OnDisable()
    {
        InventoryManager.Instance.obtainItemAction -= UpdateQuestCanvas;
        InputSystem.onDeviceChange -= UpdateInputImages;
    }

    public void SetupQuestCanvas(QuestObject _quest)
    {
        currentQuest = _quest;

        questTitleText.gameObject.SetActive(currentQuest);
        layoutQuest.gameObject.SetActive(currentQuest);
        noSelectecQuestText.SetActive(!currentQuest);

        //Comprobamos si tenemos que mostrar el texto de si hay alguna quest para completar
        canCompleteQuestText.SetActive(QuestManager.Instance.CanCompleteSomeQuest());

        if (!currentQuest)
            return;

        questTitleText.text = currentQuest.questName;

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            Image newImage = Instantiate(needImagePrefab, layoutQuest.transform).GetComponent<Image>();
            prizeImages.Add(newImage);
            newImage.sprite = item.Key.PickableSprite;
            TextMeshProUGUI newText = Instantiate(needTextPrefab, layoutQuest.transform).GetComponent<TextMeshProUGUI>();
            prizeTexts.Add(newText);
            SetMineralsTextColor(newText, InventoryManager.Instance.GetTotalItemAmount(item.Key), item.Value);
        }

        UpdateQuestCanvas();
    }
    
    private void UpdateQuestCanvas(ItemObject _itemType = null, short _itemAmount = 0)
    {
        
        canCompleteQuestText.SetActive(QuestManager.Instance.CanCompleteSomeQuest());

        if (!currentQuest)
            return;

        int index = 0;

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {

            if(item.Key == _itemType)
            {
                for(int i = 0; i < prizeImages.Count; i++)
                {
                    if (prizeImages[i].sprite == _itemType.PickableSprite) 
                    {
                        StopCoroutine("StopFloating");

                        ImageFloatEffect temp = prizeImages[i].GetComponent<ImageFloatEffect>();
                        temp.canFloat = true;
                        StartCoroutine("StopFloating", temp);
                    }
                }
            }

            prizeTexts[index].text = InventoryManager.Instance.GetTotalItemAmount(item.Key) + " / " + item.Value;
            SetMineralsTextColor(prizeTexts[index], InventoryManager.Instance.GetTotalItemAmount(item.Key), item.Value);

            index++;
        }

        //Comprobamos si tenemos que mostrar el texto de si hay alguna quest para completar
    }

    private void SetMineralsTextColor(TextMeshProUGUI _text, short _inventoryAmount, short _neededAmount)
    {
        if (_inventoryAmount >= _neededAmount)
        {
            //Tenemos todos los minerales cambiar el color a Verde
            _text.color = doneTextColor;
        }
        else
        {
            //No tenemos los materiales necesarios cambiar el color a Amarillo
            _text.color = defaultTextColor;
        }
    }

    private IEnumerator StopFloating(ImageFloatEffect imageFloatEffect)
    {
        yield return new WaitForSeconds(timeToWaitForFloat);
        imageFloatEffect.canFloat = false;
    }

    public void RemoveCurrentQuest()
    {
        for (int i = 0; i < prizeImages.Count; i++)
        {
            Destroy(prizeImages[i].gameObject);
            Destroy(prizeTexts[i].gameObject);
        }

        prizeImages.Clear();
        prizeTexts.Clear();
        
    }

    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }

}
