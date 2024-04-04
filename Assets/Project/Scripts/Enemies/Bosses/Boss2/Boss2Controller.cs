using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss2Controller : BossController
{
    [Space, Header("Boss 2"), SerializeField]
    private Transform arenaMiddlePos;
    
    [SerializeField]
    protected SpriteRenderer bossMainSR;
    [SerializeField]
    private Transform[] mapBottomCorners;
    [SerializeField]
    private Tile defaultTile;
    private Animator animator;
    [SerializeField]
    private GameObject bubblePrefab;

    [Space, Header("Rock Drop"), SerializeField]
    private Transform rockStarterPos;
    [SerializeField]
    private float rockStarterPosXOffset;
    [SerializeField]
    private int rockSize;
    [SerializeField]
    private int rockBorderValue;
    [SerializeField]
    private Boss2BreakableRockController[] breakableRocks;
    [SerializeField]
    private float rockDropDuration;
    private float rockDropTimeWaited;
    [SerializeField]
    private float rockCD;
    private float rockCDTimeWaited;
    [SerializeField]
    private ParticleSystem rockSpawnParticles;
    private Vector2 rockMovementPosition;
    [SerializeField]
    private float rockMovementSpeed;
    [SerializeField]
    protected float rockRotationSpeed;
    private Action startRockDropAction;
    private Action updateRockDropAction;


    [Space, Header("Dashes"), SerializeField]
    private int totalDashesAmount;
    private int dashesDone;
    [SerializeField]
    private float castDuration;
    [SerializeField]
    private float castTrackDuration;
    private float castTimeWaited;
    [SerializeField]
    private float dashForce;
    [SerializeField]
    private int bubblesPerDash;
    [SerializeField]
    private float minBubblesDashDot;
    [SerializeField]
    private float dashCollisionRotationAngle;

    private Action startDashesAction;
    private Action updateDashesAction;


    [Space, Header("Create Breakable Wall"), SerializeField]
    private BreakableWallController breakableWallCreate;
    [SerializeField]
    private Transform posToSpawnBreakableWall;
    [SerializeField]
    private float createBreakableWallDuration;
    private float createBreakableWallTimeWaited;
    [SerializeField]
    private float offsetBetweenCreateBW;
    [SerializeField]
    private int breakableWallWidth;
    private Vector2 createBreakableWallDir;
    [SerializeField]
    private float createBreakableWallSpeed;
    [SerializeField]
    private float createBreakableWallRotationSpeed;
    private Action startCreateBreakableWallAction;
    private Action updateCreateBreakableWallAction;

    [Space, Header("Die"), SerializeField]
    private SpriteRenderer finRenderer;
    [SerializeField]
    private SpriteRenderer tailRenderer;

    [SerializeField]
    private float timeToEnrock;
    private float timeToEnrockWaited;

    [SerializeField]
    private int enrockSize;
    [SerializeField]
    private int enrockValue;

    [SerializeField]
    private float timeToEnrockLayering;
    private float timeToEnrockLayeringWaited;

    [SerializeField]
    private GameObject pickeableItemPrefab;
    [SerializeField]
    private ItemObject rewardObject;

    [SerializeField]
    private Tilemap deadEnrockTilemap;
    private CircleCollider2D circleCollider;




    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        rockSpawnParticles.Stop();

    }

    void Update()
    {

        if (onUpdatePhaseAttacks[currentPhase][currentAttackID] != null)
            onUpdatePhaseAttacks[currentPhase][currentAttackID]();

        CheckIfDead();
    }

    protected override void SetupPhaseAttacks()
    {
        //Fase 1
        List<Action> startActions = new List<Action>();
        List<Action> updateActions = new List<Action>();

        startRockDropAction += StartRockDrop;
        startActions.Add(startRockDropAction);
        updateRockDropAction += UpdateRockDrop;
        updateActions.Add(updateRockDropAction);

        startDashesAction += StartDashes;
        startActions.Add(startDashesAction);
        updateDashesAction += UpdateDashes;
        updateActions.Add(updateDashesAction);

        startCreateBreakableWallAction += StartCreateBreakableWall;
        startActions.Add(startCreateBreakableWallAction);
        updateCreateBreakableWallAction += UpdateCreateBreakableWall;
        updateActions.Add(updateCreateBreakableWallAction);

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

    #region Rock Drop

    private void StartRockDrop()
    {
        for (int i = 0; i < breakableRocks.Length; i++)
            breakableRocks[i].gameObject.SetActive(false);

        //Activar las particulas del spawn
        rockSpawnParticles.Play();


        rockDropTimeWaited = 0;
        rockCDTimeWaited = 0;

        rockMovementPosition = mapBottomCorners[UnityEngine.Random.Range(0, mapBottomCorners.Length)].position;
        gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");

    }

    private void UpdateRockDrop()
    {
        CameraController.Instance.AddHighTrauma();
        RockMovementBehaviour();
        RockCD();
        CheckIfEndRockDrop();
    }

    //Movement Behaviour
    private void RockMovementBehaviour()
    {
        //Comprobar cuanto tiempo ha havido segun la caida de rocas
        if (rockDropTimeWaited >= rockDropDuration - rockDropDuration / 6) //Si es mayor a una  sexta parte del tiempo total
        {
            //Moverse hacia dentro de la zona
            MoveTo(arenaMiddlePos.position);
            return;
        }
        
        
        float distanceBetweenCorner = Vector2.Distance(transform.position, rockMovementPosition);
        if (distanceBetweenCorner < 1)
            LookMiddlePos();
        else
            MoveTo(rockMovementPosition);        

    }
    private void MoveTo(Vector2 _moveToPos)
    {
        //Mover a la esquina
        Vector2 dirToLook = (_moveToPos - (Vector2)transform.position).normalized;
        LookForwardDirection(dirToLook, rockRotationSpeed);
        rb2d.velocity = transform.right * rockMovementSpeed;
    }
    private void LookMiddlePos()
    {
        rb2d.velocity = Vector2.zero;

        Vector2 dirToLook = (arenaMiddlePos.position - transform.position).normalized;
        LookForwardDirection(dirToLook, rockRotationSpeed);
    }


    private void RockCD()
    {
        rockCDTimeWaited += Time.deltaTime;

        if (rockCDTimeWaited >= rockCD)
        {
            rockCDTimeWaited = 0;
            DropRock(GetUnusedRock());
        }
    }
    private int GetUnusedRock()
    {
        for (int i = 0; i < breakableRocks.Length; i++)
        {
            if (!breakableRocks[i].gameObject.activeInHierarchy)
                return i;
        }
        return 0;
    }
    private void DropRock(int _rockId)
    {
        //Cambiarle la posicion
        Vector2 randomPos = rockStarterPos.position + new Vector3(UnityEngine.Random.Range(-rockStarterPosXOffset, rockStarterPosXOffset), 0);
        breakableRocks[_rockId].transform.position = randomPos;

        //Reiniciar la roca
        ResetRockTiles(breakableRocks[_rockId].tilemap);

        //Activar el la roca
        breakableRocks[_rockId].gameObject.SetActive(true);
        breakableRocks[_rockId].rb2d.velocity = Vector2.zero;
        //Añadir una fuerza de rotacion random
        float maxRotationForce = 10;
        float randomRotationForce = UnityEngine.Random.Range(-maxRotationForce, maxRotationForce);

        breakableRocks[_rockId].rb2d.angularVelocity = randomRotationForce;

    }
    private void ResetRockTiles(Tilemap _tilemap)
    {
        for (int i = -rockSize; i < rockSize + 1; i++)
        {
            for (int j = -rockSize; j < rockSize + 1; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) < rockBorderValue)
                {
                    //Puedo poner trozo de piedra
                    Vector3Int tilePos = new Vector3Int(i, j, 0);
                    _tilemap.SetTile(tilePos, defaultTile);
                }
            }
        }
    }

    private void CheckIfEndRockDrop()
    {
        rockDropTimeWaited += Time.deltaTime;
        
        if (rockDropTimeWaited >= rockDropDuration)
        {
            //Acabar de tirar rocas
            rockSpawnParticles.Stop();
            GenerateRandomAttack();
        }
    }
    #endregion

    #region Dashes
    private void StartDashes()
    {
        dashesDone = 0;
        castTimeWaited = 0;
        rb2d.velocity = Vector2.zero;
        gameObject.layer = LayerMask.NameToLayer("Boss");

    }
    private void UpdateDashes()
    {
        if (rb2d.velocity.magnitude < 3f)
        {
            if (dashesDone >= totalDashesAmount)
            {
                GenerateRandomAttack();
                return;
            }

            rb2d.angularVelocity = 0;
            //Volver a hacer un dash
            CastDash();
        }
    }
    private void CastDash()
    {
        castTimeWaited += Time.deltaTime;

        if (castTimeWaited < castTrackDuration)
        {
            //Mirar hacia el player
            Vector2 targetDir = (PlayerManager.Instance.player.transform.position - transform.position).normalized;
            LookForwardDirection(targetDir, rockRotationSpeed);
            return;
        }

        if (castTimeWaited < castDuration)
            return;

        //EmpezarDash
        DoDash();
    }
    private void DoDash()
    {
        rb2d.AddForce(transform.right * dashForce, ForceMode2D.Impulse);
        dashesDone++;
        castTimeWaited = 0;

        for (int i = 0; i < bubblesPerDash; i++)
        {
            Rigidbody2D bubbleRb2d = Instantiate(bubblePrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
            Vector2 randomDirection = -transform.right;

            for (int j = 0; j < 10; j++)
            {
                float x = UnityEngine.Random.Range(-1f, 1f);
                float y = UnityEngine.Random.Range(-1f, 1f);
                Vector2 randomDirTemp = new Vector2(x, y).normalized;

                if (Vector2.Dot(randomDirTemp, -transform.right) > minBubblesDashDot)
                {
                    randomDirection = randomDirTemp;
                    break;
                }
            }


            float bubbleSpawnForce = 150f;

            bubbleRb2d.AddForce(randomDirection * bubbleSpawnForce, ForceMode2D.Impulse);
        }
        
    }

    #endregion

    #region CreateBreakableWall
    private void StartCreateBreakableWall()
    {
        gameObject.layer = LayerMask.NameToLayer("Boss");

        createBreakableWallTimeWaited = 0;
        createBreakableWallDir = transform.right;
    }
    private void UpdateCreateBreakableWall()
    {
        //Mover
        LookForwardDirection(createBreakableWallDir, createBreakableWallRotationSpeed);
        rb2d.velocity = createBreakableWallDir * createBreakableWallSpeed;
        //Crear el muro detras suyo
        CreateBreakableWall();
        //Comprobar si acaba el ataque
        CheckIfStopCreatingBreakableWalls();
    }

    private void CreateBreakableWall()
    {
        breakableWallCreate.ChangeTileContent(posToSpawnBreakableWall.position, defaultTile);

        Vector2 direction = transform.up;
        for (int i = 1; i < breakableWallWidth; i++)
        {
            int multiplier = (int)Mathf.Ceil(i / 2);
            int sign = i % 2 == 0 ? 1 : -1;

            breakableWallCreate.ChangeTileContent(
                (Vector2)transform.position + direction * offsetBetweenCreateBW * sign * multiplier,
                defaultTile
                );

            breakableWallCreate.ChangeTileContent(
                (Vector2)posToSpawnBreakableWall.position + direction * offsetBetweenCreateBW * sign * multiplier,
                defaultTile
                );
        }
    }

    private void CheckIfStopCreatingBreakableWalls()
    {
        createBreakableWallTimeWaited += Time.deltaTime;

        if (createBreakableWallTimeWaited >= createBreakableWallDuration)
        {
            GenerateRandomAttack();
        }
    }


    private void CollisionWithWall(Collision2D _collision)
    {
        switch (currentAttackID)
        {
            case 1:
                float randomAgle = UnityEngine.Random.Range(-dashCollisionRotationAngle, dashCollisionRotationAngle);
                rb2d.angularVelocity = randomAgle;
                break;
            case 2:
                createBreakableWallDir = (createBreakableWallDir + _collision.contacts[0].normal * 3).normalized;
                transform.right = createBreakableWallDir;
                break;
            default:
                break;
        }

        
    }

    #endregion


    private void LookForwardDirection(Vector2 _posToLook, float _rotationSpeed)
    {
        transform.right = Vector2.Lerp(transform.right, _posToLook.normalized, Time.deltaTime * _rotationSpeed);


        Quaternion newRotation = new Quaternion();
        newRotation.x = Vector2.Dot(transform.right, Vector2.right) < 0 ? 180 : 0;
        bossMainSR.transform.localRotation = newRotation;
    }

    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;

        UpdateHealthBar();
    }
    
    #region Die
    protected override void StartDie()
    {
        timeToEnrockWaited = 0;
        timeToEnrockLayeringWaited = 0;


        //Parar las particulas del spawn
        rockSpawnParticles.Stop();


        rockDropTimeWaited = 0;
        rockCDTimeWaited = 0;
        gameObject.layer = LayerMask.NameToLayer("BossNoHitWalls");

        circleCollider.enabled = false;

    }

    protected override void UpdateDie()
    {
        DieBehaviour();
    }

    private void DieBehaviour()
    {
        if (timeToEnrock > timeToEnrockWaited)
        {
            Enrock();
        }
        else
        {
            EnrockLayering();
        }
    }

    private void Enrock() 
    {
        timeToEnrockWaited += Time.deltaTime;

        Debug.Log("Enrocking");

        if (timeToEnrock <= timeToEnrockWaited)
        {
            //Ultimo frame moviendose

            rb2d.velocity = Vector2.zero;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;

            //Mirar Recto
            LookForwardDirection(Vector2.right, 1000000);

            //Limpiar las rocas a su alrededor del createBreakableWall
            Tilemap breakableWallCreateTilemap = breakableWallCreate.GetComponent<Tilemap>();
            for (int i = -enrockSize; i < enrockSize + 1; i++)
            {
                for (int j = -enrockSize; j < enrockSize + 1; j++)
                {
                    if (Mathf.Abs(i) + Mathf.Abs(j) < enrockValue)
                    {
                        //Puedo poner trozo de piedra
                        Vector3Int tilePos = new Vector3Int(i, j, 0);
                        breakableWallCreateTilemap.SetTile(tilePos, null);
                    }
                }
            }
            //Cambiar el sorting layer de todo
            bossMainSR.sortingLayerName = "Map";
            finRenderer.sortingLayerName = "Map";
            tailRenderer.sortingLayerName = "Map";
        }
    }

    private void EnrockLayering()
    {
        timeToEnrockLayeringWaited += Time.deltaTime;

        timeToEnrockLayeringWaited = Mathf.Clamp(timeToEnrockLayeringWaited, 0, enrockSize);
        int loopsToDraw = (int)(timeToEnrockLayeringWaited / timeToEnrockLayering);


        deadEnrockTilemap.SetTile(new Vector3Int(0, 0, 0), defaultTile);

        for (int i = -loopsToDraw; i < loopsToDraw; i++)
        {
            for (int j = -loopsToDraw; j < loopsToDraw; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) < enrockValue)
                {
                    Vector3Int tilePos = new Vector3Int(i, j, 0);
                    Debug.Log(tilePos);
                    deadEnrockTilemap.SetTile(tilePos, defaultTile);
                }

            }
            
        }


        if (loopsToDraw == enrockSize)
        {
            //Acaba la muerte
            Debug.Log("100% morido");
            enabled = false;   
        }

    }



    #endregion

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            GetDamage(collision.gameObject.GetComponent<Laser>().GetBulletDamage());
        }

        if (collision.collider.CompareTag("Map"))
        {
            CollisionWithWall(collision);
        }

    }


    private void OnDrawGizmosSelected()
    {
        if (rockStarterPos)
        {
            Vector3 startPos = rockStarterPos.position;
            startPos.x -= rockStarterPosXOffset;
            Vector3 endPos = rockStarterPos.position;
            endPos.x += rockStarterPosXOffset;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
