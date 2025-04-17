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
                line += level.IsBlocked(x, y) ? "#" : " ";
            }
            Debug.Log(line);
        }
    }
}
