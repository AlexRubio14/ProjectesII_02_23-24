using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableWallController : InteractableObject
{
    private Grid breakableWallGrid;
    private Tilemap breakableWallTilemap;


    private void Awake()
    {
        breakableWallGrid = GetComponentInParent<Grid>();
        breakableWallTilemap = GetComponent<Tilemap>();
    }
    private void Start()
    {
        if (isHide)
        {
            SetupParticles(vfxHideColor);
        }
        else
        {
            SetupParticles(vfxUnhideColor);
        }
    }

    public override void Interact()
    {
        Debug.LogWarning("No hay ninguna interaccion");
    }

    public override void UnHide()
    {
        SetupParticles(vfxUnhideColor);

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


    public void ChangeTileContent(Vector2 _worldPos, TileBase _tile)
    {
        Vector3Int cellPos = breakableWallGrid.LocalToCell(_worldPos);
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
