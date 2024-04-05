using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Controller : BossController
{
    [Space, Header("Boss 1"), SerializeField]
    private Transform head;
    [SerializeField]
    private Boss1BodyController[] tail;
    [SerializeField]
    private Transform arenaMiddlePos;
    private CircleCollider2D[] collisions;
    [SerializeField]
    private Sprite normalHeadSprite;
    [SerializeField]
    private Sprite stunnedHeadSprite;
    [SerializeField]
    private Sprite deadHeadSprite;
    private SpriteRenderer headSR;
    [SerializeField]
    private float outOfZoneDistance;
    [SerializeField]
    private ParticleSystem spawnTrackerParticles;
    [SerializeField]
    private LayerMask trackerParticlesLayer;
    [SerializeField]
    private GameObject bubblePrefab;
    [SerializeField]
    private GameObject crystralDrop;

    [Space, Header("Dash Wall To Wall"), SerializeField]
    private int totalDashesPerAttack;
    private int dashesRemaining;
    [SerializeField]
    private float spawnOffset;
    [SerializeField]
    private float dashSpeed;
    private Vector2 dashDirection;
    [SerializeField]
    private float dashRotationSpeed;
    [SerializeField]
    private float minSpawnDot;
    private Vector2 lastDashDir = Vector2.up;
    [SerializeField]
    private float maxDistanceParticleTracker;

    private Action dashStart;
    private Action dashUpdate;

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

    [SerializeField]
    private float bubbleSpinCD;
    private float bubbleSpinTimeWaited;

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
    [SerializeField]
    private float suctionBubbleSpawnTime;
    private float suctionBubbleTimeWatied;


    private Action suctionStart;
    private Action suctionUpdate;

    
    private Vector2 dieExitDirection;


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

        headSR = head.GetComponent<SpriteRenderer>();
        
        SetSuctionWindActive(false);
    }

    private void Update()
    {
            
        if (onUpdatePhaseAttacks[currentPhase][currentAttackID] != null && !animator.enabled)
            onUpdatePhaseAttacks[currentPhase][currentAttackID]();

        CheckIfDead();
    }

    protected override void SetupPhaseAttacks()
    {
        //Fase 1
        List<Action> startActions = new List<Action>();
        List<Action> updateActions = new List<Action>();

        //dashStart += StartDashWallToWall;
        //startActions.Add(dashStart);
        //dashUpdate += UpdateDashWallToWall;
        //updateActions.Add(dashUpdate);

        spinStart += StartSpin;
        startActions.Add(spinStart);
        spinUpdate += UpdateSpin;
        updateActions.Add(spinUpdate);

        //suctionStart += StartSuction;
        //startActions.Add(suctionStart);
        //suctionUpdate += UpdateSuction;
        //updateActions.Add(suctionUpdate);

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


        onDieStart += StartDie;
        startActions.Add(onDieStart); 
        onDieUpdate += UpdateDie;
        updateActions.Add(onDieUpdate);

        onStartPhaseAttacks.Add(Phase.DEAD, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.DEAD, updateActions.ToArray());

    }

    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;

        UpdateHealthBar();
    }

    #region Dash Wall To Wall 
    protected void StartDashWallToWall()
    {
        Debug.Log("START");
        dashesRemaining = totalDashesPerAttack;

        ResetDashValues();

    }

    protected void ResetDashValues()
    {
        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");


        Vector2 spawnDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;

        if (Vector2.Dot(spawnDir, lastDashDir) > minSpawnDot)
        {
            ResetDashValues();
            return;
        }

        lastDashDir = spawnDir;
        head.transform.position = (Vector2)arenaMiddlePos.position + spawnDir * spawnOffset;

        foreach (Boss1BodyController item in tail)
            item.ResetTailPos();



        dashDirection = (PlayerManager.Instance.player.transform.position - head.transform.position).normalized;
        rb2d.velocity = dashDirection * dashSpeed;
        head.right = dashDirection;

        CalculateRayTrackerParticles();
    }

    protected void UpdateDashWallToWall()
    {
        //Calcular un poco de rotacion para que se acerque muy poco a poco al player
        Vector2 playerDir = (PlayerManager.Instance.player.transform.position - head.transform.position).normalized;
        dashDirection = Vector2.Lerp(dashDirection, playerDir, Time.deltaTime * dashRotationSpeed).normalized;

        //mover al bicho y rotarlo hacia la direccion
        rb2d.velocity = dashDirection * dashSpeed;
        head.right = dashDirection;

        float distanceFromMiddlePos = Vector2.Distance(head.position, arenaMiddlePos.position);

        if (distanceFromMiddlePos <= spawnOffset / 1.5f)
            CameraController.Instance.AddLowTrauma();


        //Comprobar si esta dentro del circulo
        if (distanceFromMiddlePos <= maxDistanceParticleTracker)
            ShowTrackerPositionParticles(false, Vector2.zero, Vector2.zero);


        //Comprobar si esta suficientemente lejos del area para hacer otro ataque
        if (distanceFromMiddlePos <= outOfZoneDistance)
            return;

        rb2d.velocity = Vector2.zero;
        dashesRemaining--;

        if (dashesRemaining <= 0)
            GenerateRandomAttack();
        else
            ResetDashValues();

    }

    #endregion

    #region Spin
    private void StartSpin()
    {
        head.position = arenaMiddlePos.position + Vector3.down * 30;
        foreach (Boss1BodyController item in tail)
            item.ResetTailPos();

        rb2d.velocity = Vector2.zero;


        animator.enabled = true;
        animator.SetTrigger("SpinSpawnAnim");

        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");



        bubbleSpinTimeWaited = bubbleSpinCD;

        spinTimeWaited = 0;
        spinStunTimeWaited = 0;

        exitDirection.x = UnityEngine.Random.Range(-1f, 1f);
        exitDirection.y = UnityEngine.Random.Range(-1f, 1f);

        CalculateRayTrackerParticles();
    }

    public void SetTailLowFollowSpeed()
    {
        foreach (Boss1BodyController item in tail)
            item.SetLowFollowSpeed();
    }
    public void OnSpinSpawnAnimationEnd()
    {
        spinDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        animator.enabled = false;

        if (currentPhase == Phase.DEAD)
            return;

        foreach (CircleCollider2D item in collisions)
            item.gameObject.layer = LayerMask.NameToLayer("Boss");

        ShowTrackerPositionParticles(false, Vector2.zero, Vector2.zero);

        spinTimeWaited = 0;
        spinStunTimeWaited = 0;
    }

    private void UpdateSpin()
    {
        bubbleSpinTimeWaited += Time.deltaTime;

        //Comprobar el tiempo que lleva girando
        spinTimeWaited += Time.deltaTime;

        if (spinTimeWaited >= spinDuration)
        {

            //Espera stuneado
            spinStunTimeWaited += Time.deltaTime;
            headSR.sprite = stunnedHeadSprite;

            if (spinStunTimeWaited >= spinStunDuration)
            {
                headSR.sprite = normalHeadSprite;

                //Volver a la posicion de inicio 
                foreach (CircleCollider2D item in collisions)
                    item.gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");
                rb2d.angularVelocity = 0;
                head.right = Vector2.Lerp(head.right, exitDirection, Time.deltaTime * 3f).normalized;
                rb2d.velocity = head.right * spinSpeed * 0.75f;

                if (Vector2.Distance(head.position, arenaMiddlePos.position) > outOfZoneDistance)
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
    public void ChangeSpinDirection(Collision2D _collision, Vector2 _currentPos)
    {        
        float minimumDot = 0.6f;

        Vector2 targetDirection = (spinDirection + _collision.contacts[0].normal).normalized;

        Vector2 playerDirection = (PlayerManager.Instance.player.transform.position - head.position).normalized;
        //Si el player no esta en otra direccion y esta suficientemente lejos de la pared
        if (Vector2.Dot(playerDirection, targetDirection) > minimumDot)
            targetDirection = (targetDirection + playerDirection).normalized;

        spinDirection = targetDirection;

        CameraController.Instance.AddMediumTrauma();

        if (bubbleSpinCD > bubbleSpinTimeWaited || spinTimeWaited >= spinDuration)
            return;
        bubbleSpinTimeWaited = 0;
        //Instanciar una burbuja
        float spawnBubbleOffset = 1.3f;
        Vector2 pointToCollisionObject = (_currentPos - _collision.contacts[0].point).normalized;

        Vector2 spawnPoint = _collision.contacts[0].point + pointToCollisionObject * spawnBubbleOffset;
        Rigidbody2D bubbleRb2d = Instantiate(bubblePrefab, spawnPoint, Quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        float bubbleSpawnForce = 150f;

        bubbleRb2d.AddForce(randomDirection * bubbleSpawnForce, ForceMode2D.Impulse);

    }

    #endregion

    #region Suction
    protected void StartSuction()
    {
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

        rb2d.isKinematic = true;

        suctionTimeWaited = 0;

        suctionBubbleTimeWatied = 0;

        CalculateRayTrackerParticles();
    }

    protected void UpdateSuction()
    {
        suctionTimeWaited += Time.deltaTime;

        rb2d.velocity = Vector2.zero;

        CheckIfActivateWindBlow();
        InstantiateBubblesDuringSuction();

        if (suctionTimeWaited < suctionDuration)
        {
            Vector2 targetPosition = (Vector2)arenaMiddlePos.position + suctionDirection * maxSuctionPositionDistance;
            MoveHeadSuction(targetPosition, suctionMoveSpeed);
            CameraController.Instance.AddLowTrauma();
        }
        else
        {
            Vector2 exitPosition = (Vector2)arenaMiddlePos.position + suctionDirection * minSuctionPositionDistance;
            MoveHeadSuction(exitPosition, suctionMoveSpeed / 1.5f);
            CheckIfStopSuction(exitPosition);
        }

    }
    
    private void MoveHeadSuction(Vector2 _targetPos, float _suctionSpeed)
    {
        Vector2 nextPos = Vector2.Lerp(head.position, _targetPos, Time.deltaTime * _suctionSpeed);
        head.position = nextPos;
        head.right = (PlayerManager.Instance.player.transform.position - head.position).normalized;
    }
    private void InstantiateBubblesDuringSuction()
    {
        suctionBubbleTimeWatied += Time.deltaTime;

        if (suctionBubbleTimeWatied <= suctionBubbleSpawnTime)
            return;

        suctionBubbleTimeWatied = 0;
        Vector2 rayDirection = (PlayerManager.Instance.player.transform.position - head.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(head.position, rayDirection, 100, trackerParticlesLayer);

        float offset = 1.5f;
        Vector2 spawnPos = hit.point - rayDirection * offset; 

        Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
    }

    private void CheckIfActivateWindBlow()
    {
        if (!windBlowTrigger.enabled && suctionTimeWaited < suctionDuration && !windBlowTrigger.enabled && Vector2.Distance(head.position, (Vector2)arenaMiddlePos.position + suctionDirection * maxSuctionPositionDistance) <= 3f)
            SetSuctionWindActive(true);
        else if (windBlowTrigger.enabled && suctionTimeWaited >= suctionDuration)
            SetSuctionWindActive(false);
    }

    private void CheckIfStopSuction(Vector2 _exitPos)
    {
        if (suctionTimeWaited >= suctionDuration && Vector2.Distance(head.position, _exitPos) <= 3f)
        {
            rb2d.isKinematic = false;
            GenerateRandomAttack();
        }
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

    #region Die
    protected override void StartDie()
    {
        headSR.sprite = deadHeadSprite;

        head.GetComponent<CircleCollider2D>().enabled = false;
        foreach (CircleCollider2D item in collisions)
            item.enabled = false;
        



        rb2d.isKinematic = false;

        SetSuctionWindActive(false);

        foreach (Boss1BodyController item in tail)
            item.SetNormalFollowSpeed();

        if (rb2d.velocity == Vector2.zero)
            dieExitDirection = head.right;
        else
            dieExitDirection = rb2d.velocity.normalized;

        ShowTrackerPositionParticles(false, Vector2.zero, Vector2.zero);
    }
    protected override void UpdateDie()
    {
        head.right = Vector2.Lerp(head.right, dieExitDirection, Time.deltaTime * 3f).normalized;
        rb2d.velocity = head.right * spinSpeed * 0.75f;

        if (Vector2.Distance(head.position, arenaMiddlePos.position) > outOfZoneDistance)
        {
            rb2d.velocity = Vector2.zero;
            animator.enabled = true;
            animator.SetTrigger("Dead");
            foreach (Boss1BodyController item in tail)
                item.ResetTailPos();
            
        }
    }

    public void ExplodeBodyPart(int _tailID)
    {
        int currentID = _tailID - 1;

        if (currentID == -1) //Es la cabeza
            head.GetComponent<Boss1BodyController>().ExplodeBodyPart();
        else //Es el cuerpo
            tail[currentID].ExplodeBodyPart();

    }

    public void CreateCrystalDrop()
    {

        crystralDrop.GetComponent<CircleCollider2D>().enabled = true;
        crystralDrop.GetComponentInChildren<CircleCollider2D>().enabled = true;

        Rigidbody2D crystalRb2d =  crystralDrop.GetComponent<Rigidbody2D>();

        crystalRb2d.bodyType = RigidbodyType2D.Dynamic;
        float impulseForce = 10;
        crystalRb2d.AddForce(transform.up * impulseForce, ForceMode2D.Impulse);

        CameraController.Instance.AddHighTrauma();
    }

    public void StopParticles()
    {
        spawnTrackerParticles.Stop();
    }
    #endregion

    private void CalculateRayTrackerParticles()
    {
        Vector2 rayDirection = (head.position - arenaMiddlePos.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(arenaMiddlePos.position, rayDirection, 100, trackerParticlesLayer);

        ShowTrackerPositionParticles(true, hit.point, hit.normal);
    }

    private void ShowTrackerPositionParticles(bool _show, Vector2 _particlePos , Vector2 _particlesRotation)
    {
        if (_show)
        {
            spawnTrackerParticles.transform.position = _particlePos;
            spawnTrackerParticles.transform.up = _particlesRotation;
            spawnTrackerParticles.Play();
        }
        else
            spawnTrackerParticles.Stop();
    }

}
