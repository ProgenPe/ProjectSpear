using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class WoodTileCracker : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private TileBase intactTile;
    [SerializeField] private TileBase crack1Tile;
    [SerializeField] private TileBase crack2Tile;
    [SerializeField] private TileBase crack3Tile;

    [SerializeField] private float crackDuration = 3f;

    private Tilemap tilemap;

    private Dictionary<Vector3Int, Coroutine> activeCracks = new();

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }


    public void StartCrackingCell(Vector3Int cell, Spear spear)
    {
        if (activeCracks.ContainsKey(cell))
            return;

        if (!tilemap.HasTile(cell))
            return;

        Coroutine c = StartCoroutine(CrackRoutine(cell, spear));
        activeCracks.Add(cell, c);
    }


    private IEnumerator CrackRoutine(Vector3Int cell, Spear spear)
    {
        tilemap.SetTile(cell, crack1Tile);
        yield return new WaitForSeconds(crackDuration / 3f);

        tilemap.SetTile(cell, crack2Tile);
        yield return new WaitForSeconds(crackDuration / 3f);

        tilemap.SetTile(cell, crack3Tile);
        yield return new WaitForSeconds(crackDuration / 3f);

        // Возвращаем тайл в целое состояние
        tilemap.SetTile(cell, intactTile);
        activeCracks.Remove(cell);

        // Скидываем копьё
        if (spear != null)
            spear.ForceDrop();
    }


}
