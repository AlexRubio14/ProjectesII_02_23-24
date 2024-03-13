using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss1Controller : BossController
{
    [Space, Header("Boss 1"), SerializeField]
    private Transform head;
    [SerializeField]
    private Boss1BodyController[] tail;
    [SerializeField]
    private Transform arenaMiddlePos;
    private CircleCollider2D[] collisions;



    [Space, Header("Jump Wall To Wall"), SerializeField]
    private int totalJumpsPerAttack;
    private int jumpsRemaining;
    [SerializeField]
    private float spawnOffset;
    [SerializeField]
    private float jumpSpeed;
    private Vector2 jumpDirection;
    [SerializeField]
    private float jumpRotationSpeed;
    [SerializeField]
    private float minSpawnDot;
    [SerializeField]
    private float maxJumpDistance;
    private Vector2 lastJumpDir = Vector2.up;

    private Action jumpWallToWallStart;
    private Action jumpWallToWallUpdate;

    [Space, Header("Spin"), SerializeField]
    private float spinSpeed;
    [SerializeField]
    private float spinRotationSpeed;
    [SerializeField]
    private float spinHeadRotationSpeed;
    private Vector2 spinDirection;

    [SerializeField]
    private float spinDuration;
    private float spinTimeWaited;

    [SerializeField]
    private float spinStunDuration;
    private float spinStunTimeWaited;

    private Vector2 exitDirection;
    private bool canSpin;

    private Action spinStart;
    private Action spinUpdate;

    [Space, Header("Suction"), SerializeField]
    private GameObject windBlow;
    private ParticleSystem windBlowEmitter;
    private BoxCollider2D windBlowTrigger;
    [SerializeField]
    private float maxSuctionPositionDistance;
    [SerializeField]
    private float minSuctionPositionDistance;
    private Vector2 suctionDirection;
    [SerializeField]
    private float suctionMoveSpeed;

    [SerializeField]
    private float suctionDuration;
    private float suctionTimeWaited;

    private Action suctionStart;
    private Action suctionUpdate;


    private Animator animator;



    protected void Awake()
    {
        rb2d = head.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.enabled = false;

        collisions = new CircleCollider2D[tail.Length + 1];

        collisions[0] = head.GetComponent<CircleCollider2D>();

        for (int i = 1; i < collisions.Length; i++)
            collisions[i] = tail[i - 1].GetComponent<CircleCollider2D>();

        windBlowEmitter = windBlow.GetComponentInChildren<ParticleSystem>();
        windBlowTrigger = windBlow.GetComponent<BoxCollider2D>();

        SetSuctionWindActive(false);
    }

    private void Update()
    {
            
        if (onUpdatePhaseAttacks[currentPhase][currentAttackID] != null)
            onUpdatePhaseAttacks[currentPhase][currentAttackID]();

        CheckPhase();
    }

    protected override void SetupPhaseAttacks()
    {
        //Fase 1
        List<Action> startActions = new List<Action>();
        List<Action> updateActions = new List<Action>();

        jumpWallToWallStart += StartJumpWallToWall;
        startActions.Add(jumpWallToWallStart);
        jumpWallToWallUpdate += UpdateJumpWallToWall;
        updateActions.Add(jumpWallToWallUpdate);

        spinStart += StartSpin;
        startActions.Add(spinStart);
        spinUpdate += UpdateSpin;
        updateActions.Add(spinUpdate);

        suctionStart += StartSuction;
        startActions.Add(suctionStart);
        suctionUpdate += UpdateSuction;
        updateActions.Add(suctionUpdate);

        onStartPhaseAttacks.Add(Phase.PHASE_1, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.PHASE_1, updateActions.ToArray());

        //Fase 2
        startActions = new List<Action>();
        updateActions = new List<Action>();

        onStartPhaseAttacks.Add(Phase.PHASE_2, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.PHASE_2, updateActions.ToArray());

        //Cuando este muerto
        startActions = new List<Action>();
        updateActions = new List<Action>();

        onStartPhaseAttacks.Add(Phase.DEAD, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.DEAD, updateActions.ToArray());

    }

    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;

        UpdateHealthBar();
    }

    #region Jump Wall To Wall 
    protected void StartJumpWallToWall()
    {
        jumpsRemaining = totalJumpsPerAttack;

        ResetJumpValues();
    }

    protected void ResetJumpValues()
    {
        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");


        Vector2 spawnDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;

        if (Vector2.Dot(spawnDir, lastJumpDir) > minSpawnDot)
        {
            StartJumpWallToWall();
            return;
        }

        lastJumpDir = spawnDir;
        head.transform.position = (Vector2)arenaMiddlePos.position + spawnDir * spawnOffset;

        foreach (Boss1BodyController item in tail)
            item.ResetTailPos();



        jumpDirection = (PlayerManager.Instance.player.transform.position - head.transform.position).normalized;
        rb2d.velocity = jumpDirection * jumpSpeed;
        head.right = jumpDirection;
    }

    protected void UpdateJumpWallToWall()
    {
        //Calcular un poco de rotacion para que se acerque muy poco a poco al player
        Vector2 playerDir = (PlayerManager.Instance.player.transform.position - head.transform.position).normalized;
        jumpDirection = Vector2.Lerp(jumpDirection, playerDir, Time.deltaTime * jumpRotationSpeed).normalized;

        //mover al bicho y rotarlo hacia la direccion
        rb2d.velocity = jumpDirection * jumpSpeed;
        head.right = jumpDirection;

        //Comprobar si esta suficientemente lejos del area para hacer otro ataque
        if (Vector2.Distance(head.position, arenaMiddlePos.position) > maxJumpDistance)
        {
            rb2d.velocity = Vector2.zero;
            jumpsRemaining--;

            if (jumpsRemaining <= 0)
                GenerateRandomAttack();
            else
                ResetJumpValues();
            

            
        }
    }

    #endregion

    #region Spin
    private void StartSpin()
    {
        animator.enabled = true;
        animator.SetTrigger("SpinSpawnAnim");

        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");
        

        head.transform.position = (Vector2)arenaMiddlePos.position + Vector2.down * spawnOffset;

        foreach (Boss1BodyController item in tail)
            item.ResetTailPos();
        

        spinTimeWaited = 0;
        spinStunTimeWaited = 0;

        canSpin = false;

        exitDirection.x = UnityEngine.Random.Range(-1f, 1f);
        exitDirection.y = UnityEngine.Random.Range(-1f, 1f);


    }

    public void OnSpinSpawnAnimationEnd()
    {
        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("Boss");
        

        spinDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        canSpin = true;
        animator.enabled = false;

        foreach (Boss1BodyController item in tail)
            item.SetLowFollowSpeed();
        
    }

    private void UpdateSpin()
    {
        if (!canSpin)
            return;


        //Comprobar el tiempo que lleva girando
        spinTimeWaited += Time.deltaTime;

        if (spinTimeWaited >= spinDuration)
        {

            //Espera stuneado
            spinStunTimeWaited += Time.deltaTime;
            if (spinStunTimeWaited >= spinStunDuration)
            {
                //Volver a la posicion de inicio 


                foreach (CircleCollider2D item in collisions)
                    item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");
                rb2d.angularVelocity = 0;
                head.right = Vector2.Lerp(head.right, exitDirection, Time.deltaTime);
                rb2d.velocity = head.right * spinSpeed * 3;

                if (Vector2.Distance(head.position, arenaMiddlePos.position) > maxJumpDistance)
                {
                    GenerateRandomAttack();
                    foreach (Boss1BodyController item in tail)
                        item.SetNormalFollowSpeed();
                }
                
            }
            return;
        }


        //Girar la cabeza
        head.rotation *= Quaternion.Euler(0,0, spinHeadRotationSpeed * Time.deltaTime);

        //Mover hacia la direccion que este siguiendo
        rb2d.velocity = spinDirection * spinSpeed;
        

    }

    public void ChangeSpinDirection(Vector2 _collisionNormal)
    {

        if (!canSpin)
            return;
        
        float minimumDot = 0.6f;

        Vector2 targetDirection = (spinDirection + _collisionNormal).normalized;

        Vector2 playerDirection = (PlayerManager.Instance.player.transform.position - head.position).normalized;
        //Si el player no esta en otra direccion y esta suficientemente lejos de la pared
        if (Vector2.Dot(playerDirection, targetDirection) > minimumDot)
            targetDirection = (targetDirection + playerDirection).normalized;

        spinDirection = targetDirection;
    }

    #endregion

    #region Suction
    protected void StartSuction()
    {
        Debug.Log("Empieza a chuclar");
        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");

        SetSuctionWindActive(false);

        int randomValue = UnityEngine.Random.Range(-1, 2);
        int randomPos = UnityEngine.Random.Range(0, 2);

        suctionDirection = new Vector2(randomValue * randomPos, randomValue * (1 - randomPos)).normalized;

        if (suctionDirection == Vector2.zero)
            suctionDirection = Vector2.left;
        head.position = (Vector2)arenaMiddlePos.position + suctionDirection * minSuctionPositionDistance;

        foreach (Boss1BodyController item in tail)
            item.ResetTailPos();

        head.right = -suctionDirection;

        rb2d.velocity = Vector2.zero;

        suctionTimeWaited = 0;
    }

    protected void UpdateSuction()
    {
        Debug.Log("Esta chuclando");

        suctionTimeWaited += Time.deltaTime;

        rb2d.velocity = Vector2.zero;


        Vector2 nextPos;
        if (suctionTimeWaited < suctionDuration)
        {
            nextPos = Vector2.Lerp(head.position, (Vector2)arenaMiddlePos.position + suctionDirection * maxSuctionPositionDistance, Time.deltaTime * suctionMoveSpeed);
            
            if (!windBlowTrigger.enabled && Vector2.Distance(head.position, (Vector2)arenaMiddlePos.position + suctionDirection * maxSuctionPositionDistance) <= 3f)
                SetSuctionWindActive(true);
        }
        else
        {
            if (windBlowTrigger.enabled)
                SetSuctionWindActive(false);

            nextPos = Vector2.Lerp(head.position, (Vector2)arenaMiddlePos.position + suctionDirection * minSuctionPositionDistance, Time.deltaTime * suctionMoveSpeed / 2.5f);
            if (Vector2.Distance(head.position, (Vector2)arenaMiddlePos.position + suctionDirection * minSuctionPositionDistance) <= 3f)
            {
                GenerateRandomAttack();
                return;
            }
        }

        head.position = nextPos;
        head.right = (PlayerManager.Instance.player.transform.position - head.position).normalized;

    }
    
    private void SetSuctionWindActive(bool _active)
    {
        if (_active)
        {
            windBlowEmitter.Play();
            windBlowTrigger.enabled = true;
        }
        else
        {
            windBlowEmitter.Stop();
            windBlowTrigger.enabled = false;
        }
    }

    #endregion


}
