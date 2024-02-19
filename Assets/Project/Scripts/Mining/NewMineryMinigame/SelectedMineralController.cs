using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMineralController : MonoBehaviour
{
    [field: SerializeField]
    public Transform[] minerals {  get; private set; }

    [SerializeField]
    private float selectionRadius;
    [SerializeField]
    private float selectionOffsetRadius;

    public float SelectionRadius { get; private set; }

    [SerializeField]
    private Image selectionIcon;
    [SerializeField]
    private float selectionIconAppearSpeed;
    [SerializeField]
    private float selectionIconHideSpeed;
    private float selectedIconShowProgress = 0;

    [SerializeField]
    private float selectedIconShowingTime;
    [SerializeField]
    private float selectedIconHidingTime;

    private bool showing = true;

    //Esta variable es la usada para decidir si se puede hittear o no, este se podra hittear en cualquier momento excepto cuando este completamente oculto (con el alpha a 0)
    public bool hittableMineral {  get; private set; }

    public Transform selectedMineral { get; private set; }
    private Outline mineralOutline;
    // Start is called before the first frame update
    void Start()
    {
        SelectionRadius = selectionRadius + (selectionOffsetRadius * selectionRadius);
        selectionIcon.rectTransform.sizeDelta += selectionIcon.rectTransform.sizeDelta * selectionRadius;
        selectionIcon.color = new Color(1, 1, 1, 0);
        
        ChangeSelectedMineral();
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

            selectionIcon.color = new Color(1, 1, 1, selectedIconShowProgress);
            mineralOutline.effectColor = new Color(
                mineralOutline.effectColor.r,
                mineralOutline.effectColor.g,
                mineralOutline.effectColor.b,
                selectedIconShowProgress
                );

            if (selectedIconShowProgress >= 1)
            {
                //Invoke de funcion que cambie el bool showing
                Invoke("ChangeShowing", selectedIconHidingTime);
            }
        }
    }

    private void HideSelectIcon()
    {
        if (selectedIconShowProgress > 0)
        {
            //Aumentar el alpha selectedIconShowProgress
            selectedIconShowProgress = Mathf.Clamp01(selectedIconShowProgress - selectionIconHideSpeed * Time.deltaTime);

            selectionIcon.color = new Color(1, 1, 1, selectedIconShowProgress);
            mineralOutline.effectColor = new Color(
                mineralOutline.effectColor.r,
                mineralOutline.effectColor.g,
                mineralOutline.effectColor.b,
                selectedIconShowProgress
                );

            if (selectedIconShowProgress <= 0)
            {
                //Invoke de funcion que cambie el bool showing
                Invoke("ChangeShowing", selectedIconShowingTime);
                hittableMineral = false;
            }
        }
    }

    private void ChangeShowing()
    {
        showing = !showing;
        
        if (showing)
        {
            ChangeSelectedMineral();
            hittableMineral = true;
        }
    }

    private void ChangeSelectedMineral()
    {
        if(mineralOutline)
            mineralOutline.enabled = false;
        selectedMineral = minerals[Random.Range(0, minerals.Length)];
        selectionIcon.transform.position = selectedMineral.position;
        mineralOutline = selectedMineral.GetComponent<Outline>();
        mineralOutline.enabled = true;
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
