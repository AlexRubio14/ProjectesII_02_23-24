using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableWallController : InteractableObject
{
    private Grid breakableWallGrid;
    private Tilemap breakableWallTilemap;

    private Dictionary<Vector3Int, TileBase> starterTiles;
    private void Awake()
    {
        breakableWallGrid = GetComponentInParent<Grid>();
        breakableWallTilemap = GetComponent<Tilemap>();
    }
    private void Start()
    {
        if (isHide)
            SetupParticles(vfxHideColor);
        else if (interactableParticles)
            interactableParticles.Stop();

        SaveStarterTilemapState();
    }

    public override void Interact()
    {
        Debug.LogWarning("No hay ninguna interaccion");
    }

    public override void UnHide()
    {
        base.UnHide();

        interactableParticles.Stop();

        breakableWallTilemap.CompressBounds();

        for (int x = breakableWallTilemap.cellBounds.xMin; x < breakableWallTilemap.cellBounds.xMax; x++)
        {
            for (int y = breakableWallTilemap.cellBounds.yMin; y < breakableWallTilemap.cellBounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y);
                if (breakableWallTilemap.GetTile(tilePos) != null)
                {
                    Vector3 worldPos = breakableWallGrid.CellToWorld(tilePos);

                    Vector3Int cellPos = grid.WorldToCell(worldPos);
                    if (tilemap.GetTile(cellPos) != null)
                    {
                        tilemap.SetTile(cellPos, null);
                    }
                }
                
            }
        }

        interactableParticles.Stop();

        isHide = false;
    }

    private void SaveStarterTilemapState()
    {
        starterTiles = new Dictionary<Vector3Int, TileBase>();
        BoundsInt bounds = breakableWallTilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                starterTiles.Add(tilePos, breakableWallTilemap.GetTile(tilePos));
            }
        }

    }
    public IEnumerator LoadStarterTilemapState()
    {
        yield return new WaitForEndOfFrame();
        foreach (KeyValuePair<Vector3Int, TileBase> item in starterTiles)
        {
            breakableWallTilemap.SetTile(item.Key, item.Value);
        }
    }


    public void ChangeTileContent(Vector2 _worldPos, TileBase _tile)
    {
        Vector3Int cellPos = breakableWallGrid.WorldToCell(_worldPos);
        breakableWallTilemap.SetTile(cellPos, _tile);
    }

    public TileBase GetTileContent(Vector2 _worldPos)
    {
        Vector3Int cellPos = breakableWallGrid.LocalToCell(_worldPos);

        return breakableWallTilemap.GetTile(cellPos);
    }

    public Vector2 GetGridOffset()
    {
        Vector2 gridOffset = new Vector2(breakableWallGrid.cellSize.x, breakableWallGrid.cellSize.y);
        return gridOffset;
    }
}
