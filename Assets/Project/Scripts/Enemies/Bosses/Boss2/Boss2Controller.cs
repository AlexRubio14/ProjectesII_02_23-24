using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss2Controller : BossController
{


    [Space, Header("Rock Drop"), SerializeField]
    protected Transform rockStarterPos;
    [SerializeField]
    protected float rockStarterPosXOffset;
    [SerializeField]
    protected int rockSize;
    [SerializeField]
    protected int rockBorderValue;
    [SerializeField]
    protected Tile defaultTile;
    [SerializeField]
    protected Boss2BreakableRockController[] breakableRocks;
    [SerializeField]
    protected float rockDropDuration;
    protected float rockDropTimeWaited;
    [SerializeField]
    protected float rockCD;
    protected float rockCDTimeWaited;
    [SerializeField]
    private ParticleSystem rockSpawnParticles;

    private Action startRockDropAction;
    private Action updateRockDropAction;


    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();

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

    protected void StartRockDrop()
    {
        for (int i = 0; i < breakableRocks.Length; i++)
            breakableRocks[i].gameObject.SetActive(false);

        //Activar las particulas del spawn
        rockSpawnParticles.Play();


        rockDropTimeWaited = 0;
        rockCDTimeWaited = 0;
    }

    protected void UpdateRockDrop()
    {
        CameraController.Instance.AddHighTrauma();
        RockCD();
        CheckIfEndRockDrop();
    }
    protected void RockCD()
    {
        rockCDTimeWaited += Time.deltaTime;

        if (rockCDTimeWaited >= rockCD)
        {
            rockCDTimeWaited = 0;
            DropRock(GetUnusedRock());
        }
    }

    protected void CheckIfEndRockDrop()
    {
        rockDropTimeWaited += Time.deltaTime;
        
        if (rockDropTimeWaited >= rockDropDuration)
        {
            //Acabar de tirar rocas
            //GenerateRandomAttack();
        }
    }
    protected int GetUnusedRock()
    {
        for (int i = 0; i < breakableRocks.Length; i++)
        {
            if (!breakableRocks[i].gameObject.activeInHierarchy)
                return i;
        }
        return 0;
    }
    protected void DropRock(int _rockId)
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
    protected void ResetRockTiles(Tilemap _tilemap)
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


    #endregion

    #region Dashes

    #endregion

    #region 

    #endregion



    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;

        UpdateHealthBar();
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            GetDamage(collision.gameObject.GetComponent<Laser>().GetBulletDamage());
        }
    }

    #region Die
    protected override void StartDie()
    {
        
    }

    protected override void UpdateDie()
    {
        
    }
    #endregion


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
