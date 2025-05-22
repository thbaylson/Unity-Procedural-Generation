using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMerger : MonoBehaviour
{
    // Merge level data from A onto B.
    public static void MergeLevelsByTextures(Texture2D levelA, Texture2D levelB)
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
                if(levelA.GetPixel(x, y) == Color.black && levelB.GetPixel(x, y) == Color.white)
                {
                    levelA.SetPixel(x, y, new Color(0.36f, 0.23f, 0.08f));//Brown - 5C3B14
                }
            }
        }

        levelA.SaveAsset();
    }
}
