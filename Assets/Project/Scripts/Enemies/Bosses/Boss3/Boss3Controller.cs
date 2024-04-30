using System;
using System.Collections.Generic;
using UnityEngine;


public class Boss3Controller : BossController
{
    [Serializable]
    struct EnvironmentPool
    {
        public Pool[] environmentPool;
        public int[] lastEnvironmentId;
    }

    [Serializable]
    struct Pool
    {
        public GameObject[] objectsPool;
    }


    [Header("Movement"), SerializeField]
    private float movementSpeed;

    [Header("Environment"), SerializeField]
    private EnvironmentPool normalEnvironmentPool;
    [SerializeField]
    private EnvironmentPool dangerEnvironmentPool;
    [SerializeField]
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
    

    [Header("Throw"), SerializeField]
    private float temp;

    private Action behaviourActionStart;
    private Action behaviourActionUpdate;


    [Space, SerializeField]
    private Color hitColor;
    [SerializeField]
    private float hitColorLerpSpeed;
    private float hitColorLerpProcess = 1;

    private SpriteRenderer bodySR;
    private SpriteRenderer[] limbsSR;


    private Animator animator;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bodySR = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        limbsSR = GetComponentsInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        if (onUpdatePhaseAttacks[currentPhase][currentAttackID] != null)
            onUpdatePhaseAttacks[currentPhase][currentAttackID]();

        CheckHitColor();


        if (Input.GetKeyDown(KeyCode.U)) 
        {
            animator.SetTrigger("HandThrow");
        }

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

        BossManager.Instance.onBossEnter();

    }
    private void Behaviour()
    {
        MoveBehaviour();
        CheckIfEnvironmentFar();
    }

    #region Move Behaviour
    private void MoveBehaviour()
    {
        rb2d.position = Vector2.Lerp(rb2d.position, rb2d.position + Vector2.right, Time.deltaTime * movementSpeed);
    }

    #endregion


    #region Throw Behaviour


    #endregion


    #region Environment Behaviour

    private void CheckIfEnvironmentFar()
    {

        //Comprobar si estan muy lejos 
        foreach (Pool pool in normalEnvironmentPool.environmentPool)
        {
            foreach (GameObject item in pool.objectsPool)
            {
                if (item.activeInHierarchy && Vector2.Distance(transform.position, item.transform.position) > maxDistanceToDisableEnvironment)
                    item.SetActive(false);
            }
        }

        foreach (Pool pool in dangerEnvironmentPool.environmentPool)
        {
            foreach (GameObject item in pool.objectsPool)
            {
                if (item.activeInHierarchy && Vector2.Distance(transform.position, item.transform.position) > maxDistanceToDisableEnvironment)
                    item.SetActive(false);
            }
        }
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
        GameObject environmentToPlace = GetEnvironmentFromPool(environmentPool);
        if (environmentToPlace == null)
            return;

        //Activamos el escenario
        environmentToPlace.SetActive(true);

        //Lo colocamos
        environmentToPlace.transform.position = nextEnvironmentPosition;

        nextEnvironmentPosition += Vector2.right * environmentOffset;

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
    private GameObject GetEnvironmentFromPool(Pool _environmentPool)
    {
        foreach (GameObject item in _environmentPool.objectsPool)
        {
            if (!item.activeInHierarchy)
                return item;
        }

        return null;
    }

    #endregion

    #region Get Damage
    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;
        hitColorLerpProcess = 0;
        UpdateHealthBar();
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
        throw new System.NotImplementedException();
    }
    protected override void UpdateDie()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
            GetDamage(collision.gameObject.GetComponent<Laser>().GetBulletDamage());
    }
}
