using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration
{
[CreateAssetMenu(fileName = "Tileset", menuName = "ScriptableObjects/Procedural Generation/Tileset", order = 1)]
public class Tileset : ScriptableObject
{
    [SerializeField] Color wallColor;
    [SerializeField] TileVariant[] tiles = new TileVariant[16];

    public Color WallColor => wallColor;

    public GameObject GetTile(int tileIndex)
    {
        if (tileIndex >= tiles.Length) return null;

        return tiles[tileIndex].GetRandomTile();
    }
}
}
