using System;
using System.Collections.Generic;
using UnityEngine;


public class Boss3Controller : BossController
{
    [Serializable]
    struct EnvironmentPool
    {
        public Pool[] environmentPool;
        public int[] lastEnvironmentId { set; get; }
    }

    [Serializable]
    struct Pool
    {
        public GameObject[] objectsPool;
        public float maxDistanceToDisable;

        public GameObject GetDisabledItem()
        {
            foreach (GameObject item in objectsPool)
            {
                if (!item.activeInHierarchy)    
                    return item;
            }

            return null;
        }

        public void CheckDisableDistance(Vector2 _objectToCheck)
        {
            foreach (GameObject item in objectsPool)
            {
                if (item.activeInHierarchy && Vector2.Distance(_objectToCheck, item.transform.position) > maxDistanceToDisable)
                    item.SetActive(false);
            }
        }

    }

    [Serializable]
    struct ThrowItem
    {
        public Pool pool;
        public float itemsToThrow;
        public Vector2 throwOffset;
        public Vector2 throwForce;
        public float rotationForce;
        public List<GameObject> ThrowCurrentItem(Vector2 _spawnPoint)
        {
            List<GameObject> usedItems = new List<GameObject>();
            for (int i = 0; i < itemsToThrow; i++)
            {
                Vector2 direction = (Vector2.left + new Vector2(0, -UnityEngine.Random.Range(throwOffset.x, throwOffset.y)) * i).normalized;
                GameObject itemToThrow = pool.GetDisabledItem();
                if (!itemToThrow)
                    return usedItems;
                itemToThrow.SetActive(true);
                itemToThrow.transform.position = _spawnPoint;
                Rigidbody2D itemRb2d = itemToThrow.GetComponent<Rigidbody2D>();
                float force = UnityEngine.Random.Range(throwForce.x, throwForce.y);
                itemRb2d.AddForce(direction * force, ForceMode2D.Impulse);
                itemRb2d.angularVelocity = UnityEngine.Random.Range(-rotationForce, rotationForce);

                usedItems.Add(itemToThrow);
            }

            return usedItems;
        }
    }

    [Header("Movement"), SerializeField]
    private float movementSpeed;

    [Header("Environment"), SerializeField]
    private EnvironmentPool normalEnvironmentPool;
    [SerializeField]
    private EnvironmentPool dangerEnvironmentPool;
    [Space, SerializeField]
    private float environmentOffset;
    [SerializeField]
    private Transform enviromentStarterPosition;
    private Vector2 nextEnvironmentPosition;
    [SerializeField]
    private Vector2Int minMaxDangerEnvironmentSpawn;
    private int dangerEnvironmentToSkip;
    private int dangerEnvironmentSkiped;

    [Space, SerializeField]
    private float maxDistanceToDisableEnvironment;
    
    private enum ThrowTypes { NONE, BOULDER, BREAKABLE_ROCKS, BUBBLES };

    [Header("Throw"), SerializeField]
    private Transform handPosition;
    [SerializeField]
    private float timeToThrow;
    private float timeToThrowWaited = 0;

    private ThrowTypes[] lastThrowType;

    [Header("Boulder"), SerializeField]
    private ThrowItem boulders;

    [Header("BreakableRocks"), SerializeField]
    private ThrowItem breakableRocks;

    [Header("Bubbles"), SerializeField]
    private ThrowItem bubbles;

    private Action behaviourActionStart;
    private Action behaviourActionUpdate;


    [Space, Header("Damage"), SerializeField]
    private Color hitColor;
    [SerializeField]
    private float hitColorLerpSpeed;
    private float hitColorLerpProcess = 1;

    private Vector2 dieExitDirection;
    [SerializeField]
    private float diedRotationSpeed;
    [SerializeField]
    private float diedExitSpeed;
    [SerializeField]
    private Transform cameraLookObj;
    [SerializeField]
    private Transform behindAvalanche;
    [SerializeField]
    private GameObject limitZone;
    [SerializeField]
    private GameObject dropItem;

    private SpriteRenderer bodySR;
    private SpriteRenderer[] limbsSR;


    private Animator animator;


    [Space, Header("Audio"), SerializeField]
    private AudioClip[] damageClips;
    [SerializeField]
    private AudioClip[] crawlClips;
    [Space, SerializeField]
    private AudioClip castThrowClip;
    [SerializeField]
    private AudioClip throwBouldersClip;
    [SerializeField]
    private AudioClip throwBreakableWallsClip;
    [SerializeField]
    private AudioClip throwBubblesClip;

    [SerializeField]
    private AudioClip dieClip;


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bodySR = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        limbsSR = GetComponentsInChildren<SpriteRenderer>();

        animator.SetFloat("Speed", 1);

        animator.enabled = false;
        behindAvalanche.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        animator.enabled = true;
        behindAvalanche.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        animator.enabled = false;
        behindAvalanche.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (onUpdatePhaseAttacks[currentPhase][currentAttackID] != null)
            onUpdatePhaseAttacks[currentPhase][currentAttackID]();

        CheckHitColor();
    }

    protected override void SetupPhaseAttacks()
    {
        List<Action> startActions = new List<Action>();
        List<Action> updateActions = new List<Action>();


        behaviourActionStart += StartBehaviour;
        startActions.Add(behaviourActionStart);
        behaviourActionUpdate += Behaviour;
        updateActions.Add(behaviourActionUpdate);

        onStartPhaseAttacks.Add(Phase.PHASE_1, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.PHASE_1, updateActions.ToArray());


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

    private void StartBehaviour()
    {
        //Inicializar las variables para el Environment
        normalEnvironmentPool.lastEnvironmentId = new int[2];
        dangerEnvironmentPool.lastEnvironmentId = new int[2];

        normalEnvironmentPool.lastEnvironmentId[0] = -1;
        normalEnvironmentPool.lastEnvironmentId[1] = -1;

        dangerEnvironmentPool.lastEnvironmentId[0] = -1;
        dangerEnvironmentPool.lastEnvironmentId[1] = -1;


        nextEnvironmentPosition = enviromentStarterPosition.position;

        RandomizeNextDangerZone();

        PlaceNewEnvironment();

        //Inicializar variables lanzamientos
        lastThrowType = new ThrowTypes[2];

        lastThrowType[0] = ThrowTypes.NONE;
        lastThrowType[1] = ThrowTypes.NONE;
        timeToThrowWaited = 0;

        RandomizeNextThrow();



        BossManager.Instance.onBossEnter();

    }
    private void Behaviour()
    {
        MoveBehaviour();
        CheckIfEnvironmentFar();
        WaitToThrow();
        CheckPoolsDistance();
        CheckIfDead();
    }

    #region Move Behaviour
    private void MoveBehaviour()
    {
        rb2d.position = Vector2.Lerp(rb2d.position, rb2d.position + Vector2.right, movementSpeed * Time.deltaTime * TimeManager.Instance.timeParameter);
    }
    private void PlayCrawlClip()
    {
        AudioManager.instance.PlayOneShotRandomSound2d(crawlClips, "Boss3", 0.2f, 0.7f, 1.3f);
    }
    #endregion


    #region Throw Behaviour
    private void WaitToThrow()
    {
        timeToThrowWaited += Time.deltaTime * TimeManager.Instance.timeParameter;

        if (timeToThrowWaited >= timeToThrow)
        {
            animator.SetTrigger("HandThrow");
            timeToThrowWaited = 0;
            AudioManager.instance.Play2dOneShotSound(castThrowClip, "Boss3");
        }
    }
    private void Throw()
    {
        RandomizeNextThrow();

        switch (lastThrowType[0])
        {
            case ThrowTypes.BOULDER:
                boulders.ThrowCurrentItem(handPosition.position);
                AudioManager.instance.Play2dOneShotSound(throwBouldersClip, "Boss3");
                break;
            case ThrowTypes.BREAKABLE_ROCKS:
                List<GameObject> breakableRocksList = breakableRocks.ThrowCurrentItem(handPosition.position);
                foreach (GameObject breakableRock in breakableRocksList)
                    breakableRock.GetComponent<Boss2BreakableRockController>().ResetRockSize();
                
                AudioManager.instance.Play2dOneShotSound(throwBreakableWallsClip, "Boss3");
                break;
            case ThrowTypes.BUBBLES:
                bubbles.ThrowCurrentItem(handPosition.position);
                AudioManager.instance.Play2dOneShotSound(throwBubblesClip, "Boss3");
                break;
            default:
                break;
        }

    }
    private void RandomizeNextThrow()
    {
        ThrowTypes randomThrow = (ThrowTypes)UnityEngine.Random.Range(1, (int)ThrowTypes.BUBBLES + 1);
        //Si el numero esta repetido con los dos anteriores llamaremos a la misma funcion haciendola recursiva
        if (randomThrow != ThrowTypes.NONE && (randomThrow == lastThrowType[0] || randomThrow == lastThrowType[1]))
        {
            RandomizeNextThrow();
            return;
        }

        //Guardaremos el id de la 
        lastThrowType[1] = lastThrowType[0];
        lastThrowType[0] = randomThrow;

    }
    private void CheckPoolsDistance()
    {
        boulders.pool.CheckDisableDistance(transform.position);
        breakableRocks.pool.CheckDisableDistance(transform.position);
    }

    #endregion


    #region Environment Behaviour

    private void CheckIfEnvironmentFar()
    {

        //Comprobar si estan muy lejos 
        foreach (Pool pool in normalEnvironmentPool.environmentPool)
            pool.CheckDisableDistance(transform.position);

        foreach (Pool pool in dangerEnvironmentPool.environmentPool)
            pool.CheckDisableDistance(transform.position);
    }

    private void RandomizeNextDangerZone()
    {
        dangerEnvironmentToSkip = UnityEngine.Random.Range(minMaxDangerEnvironmentSpawn.x, minMaxDangerEnvironmentSpawn.y);
        dangerEnvironmentSkiped = 0;
    }

    public void PlaceNewEnvironment()
    {
        //Revisar que tipo de Zona he de colocar
        if (dangerEnvironmentSkiped >= dangerEnvironmentToSkip)
        {
            //Peligroso
            PlaceEnvironment(dangerEnvironmentPool);
            RandomizeNextDangerZone();
        }
        else
        {
            //Normal
            PlaceEnvironment(normalEnvironmentPool);
            dangerEnvironmentSkiped++;
        }
    }
    private void PlaceEnvironment(EnvironmentPool _environmentPool)
    {
        Pool environmentPool = RecursiveSelectRandomEnvironmentPool(_environmentPool);
        GameObject environmentToPlace = environmentPool.GetDisabledItem();
        if (environmentToPlace == null)
            return;

        //Activamos el escenario
        environmentToPlace.SetActive(true);

        //Lo colocamos
        environmentToPlace.transform.position = nextEnvironmentPosition;

        nextEnvironmentPosition += Vector2.right * environmentOffset;

        BreakableWallController breakableWall = environmentToPlace.GetComponentInChildren<BreakableWallController>();
        if (breakableWall)
            StartCoroutine(breakableWall.LoadStarterTilemapState());


    }
    private Pool RecursiveSelectRandomEnvironmentPool(EnvironmentPool _environmentPool)
    {
        
        int randomValue = UnityEngine.Random.Range(0, _environmentPool.environmentPool.Length);
        //Si el numero esta repetido con los dos anteriores llamaremos a la misma funcion haciendola recursiva
        if (randomValue == _environmentPool.lastEnvironmentId[0] || randomValue == _environmentPool.lastEnvironmentId[1])
        {
            return RecursiveSelectRandomEnvironmentPool(_environmentPool); ;
        }

        //Guardaremos el id de la 
        _environmentPool.lastEnvironmentId[1] = _environmentPool.lastEnvironmentId[0];
        _environmentPool.lastEnvironmentId[0] = randomValue;

        return _environmentPool.environmentPool[randomValue];
    }

    #endregion

    #region Get Damage
    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;
        hitColorLerpProcess = 0;
        UpdateHealthBar();
        AudioManager.instance.PlayOneShotRandomSound2d(damageClips, "Boss3");

    }
    private void CheckHitColor()
    {
        if (hitColorLerpProcess >= 1)
            return;

        hitColorLerpProcess += Time.deltaTime * hitColorLerpSpeed * TimeManager.Instance.timeParameter;

        Color lerpColor = Color.Lerp(hitColor, Color.white, hitColorLerpProcess);
        bodySR.color = lerpColor;

        foreach (SpriteRenderer item in limbsSR)
            item.color = lerpColor;

        hitColorLerpProcess = Mathf.Clamp01(hitColorLerpProcess);

    }
    #endregion

    #region Die
    protected override void StartDie()
    {
        AudioManager.instance.Play2dOneShotSound(victoryThemeClip, "VictoryTheme", 1, 1, 1);

        limitZone.SetActive(true);
        dropItem.SetActive(true);

        cameraLookObj.SetParent(null);
        behindAvalanche.SetParent(null);
        limitZone.transform.SetParent(null);
        dropItem.transform.SetParent(null);
        PickableItemController pickableItem = dropItem.GetComponent<PickableItemController>();
        pickableItem.InitializeItem(pickableItem.currentItem);
        
        dieExitDirection = Vector2.right;

        animator.SetFloat("Speed", 3);

        AudioManager.instance.Play2dOneShotSound(dieClip, "Boss3", 1, 1, 1);
    }
    protected override void UpdateDie()
    {
        if (Vector2.Distance(transform.position, PlayerManager.Instance.player.transform.position) > 60)
            return;
        dieExitDirection = Vector2.Lerp(dieExitDirection, Vector2.up, diedRotationSpeed * Time.deltaTime * TimeManager.Instance.timeParameter).normalized;
        transform.up = dieExitDirection;
        rb2d.position = Vector2.Lerp(rb2d.position, rb2d.position + dieExitDirection, diedExitSpeed * Time.deltaTime * TimeManager.Instance.timeParameter);
    }
    #endregion

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
            GetDamage(collision.gameObject.GetComponent<Laser>().GetBulletDamage());
    }

}
