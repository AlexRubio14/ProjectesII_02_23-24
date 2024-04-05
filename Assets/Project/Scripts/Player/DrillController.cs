using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class DrillController : MonoBehaviour
{
    [SerializeField]
    private GameObject laserDrill;

    [Space, Header("Drilling"), SerializeField]
    private float raysDelimiter;

    [Space, SerializeField]
    private float raysDistance;
    [SerializeField]
    private int totalRays;

    [SerializeField]
    private LayerMask breakableWallLayer;

    [Space, Header("Laser Line Renderers"), SerializeField]
    private Transform[] laserCannons; 
    [SerializeField]
    private Material laserMaterial;
    [SerializeField]
    private float laserWidth;
    [SerializeField]
    private Gradient laserColor;

    [Space, Header("Particles"), SerializeField]
    private ParticleSystem[] laserParticles;

    #region Tiles

    [Space, Header("Tiles"), SerializeField]
    private TileBase baseTile;
    [SerializeField]
    private TileBase aloneTile;
    [SerializeField]
    private TileBase oneSideTileUp;
    [SerializeField]
    private TileBase oneSideTileDown;
    [SerializeField]
    private TileBase oneSideTileRight;
    [SerializeField]
    private TileBase oneSideTileLeft;
    [SerializeField]
    private TileBase twoSideTileDownLeft;
    [SerializeField]
    private TileBase twoSideTileDownRight;
    [SerializeField]
    private TileBase twoSideTileUpLeft;
    [SerializeField]
    private TileBase twoSideTileUpRight;
    [SerializeField]
    private TileBase twoSideTileUpDown;
    [SerializeField]
    private TileBase twoSideTileRightLeft;
    [SerializeField]
    private TileBase threeSideTileUp;
    [SerializeField]
    private TileBase threeSideTileDown;
    [SerializeField]
    private TileBase threeSideTileRight;
    [SerializeField]
    private TileBase threeSideTileLeft;
    #endregion

    private LineRenderer[] lasers;


    [Space, SerializeField]
    private bool showGizmos = false;

    [Space, SerializeField]
    private AudioClip drillClip;

    private SizeUpgradeController sizeUpgrade;

    private void Awake()
    {
        lasers = new LineRenderer[totalRays];
        for (int i = 0; i < totalRays; i++)
        {
            GameObject laserObj = new GameObject("Laser" + i);
            laserObj.transform.parent = laserDrill.transform;
            LineRenderer currLaser = laserObj.AddComponent<LineRenderer>();
            currLaser.enabled = false;
            currLaser.numCapVertices = 90;
            currLaser.material = laserMaterial;
            currLaser.startWidth = laserWidth;
            currLaser.SetPositions(new Vector3[2]);
            currLaser.colorGradient = laserColor;
            currLaser.sortingLayerName = "Player";
            lasers[i] = currLaser;
        }


        sizeUpgrade = GetComponent<SizeUpgradeController>();    
    }

    private void Update()
    {
        Drill();        
    }

    private void Drill()
    {

        List<RaycastHit2D> hits = CheckSomethingToDrill();

        DrillWalls(hits);

        ThrowLasers(hits);

        hits.Clear();
    }

    private List<RaycastHit2D> CheckSomethingToDrill()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        float raysOffset = (raysDelimiter * 2 * sizeUpgrade.sizeMultiplyer) / totalRays;
        Vector2 rayPos = transform.localPosition - (transform.up * (raysOffset * Mathf.Floor(totalRays / 2)));

        for (int i = 1; i < totalRays + 1; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rayPos, transform.right, raysDistance * sizeUpgrade.sizeMultiplyer, breakableWallLayer);
            if (hit && hit.collider.CompareTag("BreakableWall"))
                hits.Add(hit);

            rayPos += (Vector2)(transform.up * raysOffset);
        }

        return hits;
    }
    private void DrillWalls(List<RaycastHit2D> _hits)
    {
        int totalHits = 0;
        foreach (RaycastHit2D hit in _hits)
        {
            BreakableWallController breakableWall = hit.collider.GetComponent<BreakableWallController>();
            if (breakableWall.isHide)
            {
                breakableWall = null;
                return;
            }

            breakableWall.ChangeTileContent(hit.centroid, null);

            Vector2 gridOffset = breakableWall.GetGridOffset();
            Vector2 tilePos = hit.centroid;
            UpdateSideTile(new Vector2(tilePos.x + gridOffset.x, tilePos.y), breakableWall);
            UpdateSideTile(new Vector2(tilePos.x - gridOffset.x, tilePos.y), breakableWall);
            UpdateSideTile(new Vector2(tilePos.x, tilePos.y + gridOffset.y), breakableWall);
            UpdateSideTile(new Vector2(tilePos.x, tilePos.y - gridOffset.y), breakableWall);

            totalHits++;
        }
        

        if (totalHits != 0)
        {
            CameraController.Instance.SetTrauma(0.5f);
        }
    }

    private void UpdateSideTile(Vector2 _tilePos, BreakableWallController _breakableWall)
    {
        Vector2 gridOffset = _breakableWall.GetGridOffset();

        if (_breakableWall.GetTileContent(_tilePos) == null)
            return;

        bool haveTileUp;
        if (_breakableWall.GetTileContent(new Vector3(_tilePos.x, _tilePos.y - gridOffset.y)) != null)
        {
            haveTileUp = true;
        }
        else
        {
            haveTileUp = false;
        }

        bool haveTileDown;
        if (_breakableWall.GetTileContent(new Vector3(_tilePos.x, _tilePos.y + gridOffset.y)) != null)
        {
            haveTileDown = true;
        }
        else
        {
            haveTileDown = false;
        }
        bool haveTileRight;
        if (_breakableWall.GetTileContent(new Vector3(_tilePos.x - gridOffset.x, _tilePos.y )) != null)
        {
            haveTileRight = true;
        }
        else
        {
            haveTileRight = false;
        }
        bool haveTileLeft;
        if (_breakableWall.GetTileContent(new Vector3(_tilePos.x + gridOffset.x, _tilePos.y)) != null)
        {
            haveTileLeft = true;
        }
        else
        {
            haveTileLeft = false;
        }

        TileBase currentTile = null;

        if (haveTileUp && haveTileDown && haveTileRight && haveTileLeft)
        {
            //LOS TIENE TODOS
            currentTile = baseTile;
        }
        else if (!haveTileUp && haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE ARRIBA
            currentTile = threeSideTileDown;
        }
        else if (haveTileUp && !haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE ABAJO
            currentTile = threeSideTileUp;
        }
        else if (haveTileUp && haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE LA DERECHA
            currentTile = threeSideTileLeft;
        }
        else if (haveTileUp && haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTA EL DE LA IZQUIERDA
            currentTile = threeSideTileRight;
        }
        else if (!haveTileUp && !haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y ABAJO
            currentTile = twoSideTileRightLeft;
        }
        else if (haveTileUp && haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE IZQUIERDA Y DERECHA
            currentTile = twoSideTileUpDown;
        }
        else if (!haveTileUp && haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y DERECHA
            currentTile = twoSideTileUpRight;

        }
        else if (!haveTileUp && haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y IZQUIERDA
            currentTile = twoSideTileUpLeft;

        }
        else if (haveTileUp && !haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ABAJO Y DERECHA
            currentTile = twoSideTileDownRight;

        }
        else if (haveTileUp && !haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ABAJO Y IZQUIERDA
            currentTile = twoSideTileDownLeft;
        }
        else if (haveTileUp && !haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ABAJO, DERECHA Y IZQUIERDA
            currentTile = oneSideTileDown;
        }
        else if (!haveTileUp && haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA, DERECHA Y IZQUIERDA
            currentTile = oneSideTileUp;
        }
        else if (!haveTileUp && !haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA, ABAJO Y IZQUIERDA
            currentTile = oneSideTileLeft;
        }
        else if (!haveTileUp && !haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA, ABAJO Y DERECHA
            currentTile = oneSideTileRight;
        }
        else if (!haveTileUp && !haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN TODOS
            currentTile = aloneTile;
        }

        _breakableWall.ChangeTileContent(_tilePos, currentTile);

    }

    private void ThrowLasers(List<RaycastHit2D> _hits)
    {
        for (int i = 0; i < totalRays; i++)
        {
            if (i < _hits.Count)
            {
                lasers[i].enabled = true;
                lasers[i].SetPosition(0, GetNearestCannonPos(_hits[i].point));
                lasers[i].SetPosition(1, _hits[i].point);

                ParticleSystem currentParticle = GetUnusedParticleSystem();
                if (currentParticle != null)
                {
                    currentParticle.gameObject.transform.position = _hits[i].point;
                    currentParticle.Play(true);
                }
                AudioManager.instance.Play2dOneShotSound(drillClip, "Drill");
            }
            else
            {
                lasers[i].enabled = false;
            }
        }
    }

    private Vector3 GetNearestCannonPos(Vector2 _destinyPos)
    {
        float distanceBetweenFirstPos = Vector2.Distance(laserCannons[0].position, _destinyPos);
        float distanceBetweenSecondPos = Vector2.Distance(laserCannons[1].position, _destinyPos); ;
        if (distanceBetweenFirstPos <= distanceBetweenSecondPos)
        {
            return laserCannons[0].position;
        }
        else
        {
            return laserCannons[1].position;
        }
    }

    private ParticleSystem GetUnusedParticleSystem()
    {
        foreach (ParticleSystem item in laserParticles)
        {
            if (!item.isPlaying)
            {
                return item;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.magenta;
        float sizeMultiplier = sizeUpgrade ? sizeUpgrade.sizeMultiplyer : 1f;
        float raysOffset = (raysDelimiter * 2 * sizeMultiplier) / totalRays;
        Vector2 rayPos = transform.localPosition - (transform.up * (raysOffset  * Mathf.Floor(totalRays/2)));

        for (int i = 1; i < totalRays + 1; i++)
        {
            Gizmos.DrawLine(rayPos, rayPos + ((Vector2)transform.right * raysDistance));

            rayPos += (Vector2)(transform.up * raysOffset);
        }

    }

    
}

