using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMineralController : MonoBehaviour
{
    [field: SerializeField]
    public Transform[] minerals {  get; private set; }
    public Dictionary<Transform, bool> activeMinerals;

    [SerializeField]
    private float selectionRadius;
    [SerializeField]
    private float selectionOffsetRadius;

    public float SelectionRadius { get; private set; }

    [field: SerializeField]
    public Image selectionIcon { private set; get; }
    [SerializeField]
    private float selectionIconAppearSpeed;
    [SerializeField]
    private float selectionIconHideSpeed;
    [SerializeField]
    private float hideBlinkEffectSpeed;
    private float blinkTime = 0;

    private float selectedIconShowProgress = 0;

    [SerializeField]
    private float selectedIconShowingTime;
    [SerializeField]
    private float selectedIconHidingTime;
    private float timeWaited = 0;

    private bool showing = true;

    //Esta variable es la usada para decidir si se puede hittear o no, este se podra hittear en cualquier momento excepto cuando este completamente oculto (con el alpha a 0)
    public bool hittableMineral {  get; private set; }

    public Transform selectedMineral { get; private set; }
    private Outline mineralOutline;

    private MineryMinigameController mineryController;


    // Start is called before the first frame update
    void Awake()
    {
        mineryController = GetComponentInParent<MineryMinigameController>();

        SelectionRadius = selectionRadius + (selectionOffsetRadius * selectionRadius);
        selectionIcon.rectTransform.sizeDelta += selectionIcon.rectTransform.sizeDelta * selectionRadius;
        activeMinerals = new Dictionary<Transform, bool>();
    }


    private void OnEnable()
    {
        selectionIcon.color = Color.white.WithAlpha(0);
        selectedIconShowProgress = 0;
        showing = false;
        hittableMineral = false;
        if (mineralOutline)
            mineralOutline.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        SelectionIconBehaviour();
    }

    private void SelectionIconBehaviour()
    {
        if (showing)
        {
            ShowSelectionIcon();
        }
        else
        {
            HideSelectIcon();
        }
    }

    private void ShowSelectionIcon()
    {
        if (selectedIconShowProgress < 1)
        {
            //Aumentar el alpha selectedIconShowProgress
            selectedIconShowProgress = Mathf.Clamp01(selectedIconShowProgress + selectionIconAppearSpeed * Time.deltaTime);
            selectionIcon.color = Color.white.WithAlpha(selectedIconShowProgress);
            mineralOutline.effectColor = mineralOutline.effectColor.WithAlpha(selectedIconShowProgress);

        }
        else
        {
            selectionIcon.color = Color.white.WithAlpha(1);
            mineralOutline.effectColor = mineralOutline.effectColor.WithAlpha(1);

            //Timer para que se deje de ver el mineral
            timeWaited += Time.deltaTime;
            if (timeWaited >= selectedIconShowingTime)
            {
                timeWaited = 0;
                showing = false;
            }
        }
    }

    private void HideSelectIcon()
    {
        if (selectedIconShowProgress > 0)
        {
            //Aumentar el alpha selectedIconShowProgress
            selectedIconShowProgress = Mathf.Clamp01(selectedIconShowProgress - selectionIconHideSpeed * Time.deltaTime);
            float blinkSpeed = (1 - selectedIconShowProgress) * hideBlinkEffectSpeed;
            blinkTime += Time.deltaTime;
            float pingPongProgress = Mathf.PingPong(blinkTime * blinkSpeed, 0.8f) + 0.2f;
            selectionIcon.color = Color.white.WithAlpha(pingPongProgress);
            mineralOutline.effectColor = mineralOutline.effectColor.WithAlpha(pingPongProgress);

            if (selectedIconShowProgress <= 0)
            {
                //Perder vida de la roca
                mineryController.currentMineral.currentRockHealth -= mineryController.mineralMissedRockDamage;
            }
        }
        else
        {
            selectionIcon.color = Color.white.WithAlpha(0);
            mineralOutline.effectColor = mineralOutline.effectColor.WithAlpha(0);
            hittableMineral = false;
            //Timer para que se vuelva a ver el mineral 
            timeWaited += Time.deltaTime;
            if (timeWaited >= selectedIconHidingTime)
            {
                timeWaited = 0;
                blinkTime = 0;
                showing = true;
                ChangeSelectedMineral();
                hittableMineral = true;
                if (mineralOutline)
                    mineralOutline.enabled = true;
            }
        }
    }

    public void ChangeSelectedMineral()
    {
        if(mineralOutline)
            mineralOutline.enabled = false;
        Transform randomMineral = minerals[Random.Range(0, minerals.Length)];

        if (activeMinerals.ContainsKey(randomMineral) && !activeMinerals[randomMineral])
        {
            ChangeSelectedMineral();
            return;
        }

        selectedMineral = randomMineral;
        selectionIcon.transform.position = selectedMineral.position;
        mineralOutline = selectedMineral.GetComponent<Outline>();
    }

    public void MineralMined()
    {
        selectionIcon.color = Color.white.WithAlpha(0);
        selectedIconShowProgress = 0;
        showing = false;
        timeWaited = selectedIconHidingTime * 0.5f;
        ChangeSelectedMineral();
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Transform item in minerals)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(item.position, selectionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(item.position, selectionRadius + (selectionOffsetRadius * selectionRadius));
        }


    }
}
