using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquares : MonoBehaviour
{
    [SerializeField] Texture2D levelTexture;
    
    [ContextMenu("Create Level Geometry")]
    public void CreateLevelGeometry()
    {
        TextureBasedLevel level = new TextureBasedLevel(levelTexture);
        for (int y = 0; y < level.Length - 1; y++)
        {
            string line = "";
            for (int x = 0; x < level.Length - 1; x++)
            {
                int tileIndex = CalculateTileIndex(level, x, y);
                line += tileIndex.ToString("00") + " ";
            }
            Debug.Log(line);
        }
    }

    private int CalculateTileIndex(ILevel level, int x, int y)
    {
        // Calculate the tile index based on the state of the four corners
        // TODO: Consider a less hardcoded solution.
        int topLeft = level.IsBlocked(x, y + 1) ? 1 : 0;
        int topRight = level.IsBlocked(x + 1, y + 1) ? 1 : 0;
        int bottomLeft = level.IsBlocked(x, y) ? 1 : 0;
        int bottomRight = level.IsBlocked(x + 1, y) ? 1 : 0;
        int tileIndex = topLeft + 2*topRight + 4*bottomRight + 8*bottomLeft;

        return tileIndex;
    }
}
