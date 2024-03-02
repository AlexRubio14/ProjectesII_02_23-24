using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementTutorial : Tutorial
{

    [Space, SerializeField]
    private Image rotateImage;
    [SerializeField]
    private Image accelerateImage;
    [SerializeField]
    private Sprite[] rotateSprites;
    [SerializeField]
    private Sprite[] accelerateSprites;

    [Space, SerializeField]
    private float maxDistanceFromPlayer;
    private Vector2 playerStartPos;

    protected override void TutorialMethod()
    {
        
    }

    public void StartTutorial()
    {
        rotateImage.sprite = rotateSprites[(int)InputController.Instance.GetControllerType()];
        accelerateImage.sprite = accelerateSprites[(int)InputController.Instance.GetControllerType()];
        playerStartPos = PlayerManager.Instance.player.transform.position;
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(playerStartPos, PlayerManager.Instance.player.transform.position) >= maxDistanceFromPlayer)
            EndTutorial();
        
    }

    protected override void EndTutorial()
    {
        gameObject.SetActive(false);
    }




}
