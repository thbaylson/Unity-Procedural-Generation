using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMerger : MonoBehaviour
{
    // Merge level data from A onto B.
    public static void MergeLevelsByTextures(Texture2D levelA, Texture2D levelB, Color wallColor, Color newPixelColor)
    {
        if(levelA.width != levelB.width || levelA.height != levelB.height)
        {
            Debug.LogError("Levels must be the same size to merge.");
            return;
        }

        for(int x = 0; x < levelA.width; x++)
        {
            for(int y = 0; y < levelA.height; y++)
            {
                if(levelA.GetPixel(x, y) == wallColor && levelB.GetPixel(x, y) != wallColor)
                {
                    levelA.SetPixel(x, y, newPixelColor);
                }
            }
        }

        levelA.SaveAsset();
    }
}
