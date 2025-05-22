using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CellularAutomataCaves : MonoBehaviour
{
    //[Header("Level Layout Settings")]
    //[SerializeField] RoomLevelLayoutConfig levelConfig;
    [SerializeField] private int levelWidth = 64;
    [SerializeField] private int levelLength = 64;
    [SerializeField] private int upperBoundNeighbors = 4;
    [SerializeField] private int lowerBoundNeighbors = 3;
    [SerializeField] private int iterations = 5;

    [Header("Level Layout Display")]
    [SerializeField] GameObject levelLayoutDisplay;

    private Random random;
    private int[,] level;

    [ContextMenu("Generate New Seed")]
    public void GenerateNewSeed()
    {
        SharedLevelData.Instance.GenerateSeed();
    }

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        SharedLevelData.Instance.ResetRandom();
        random = SharedLevelData.Instance.Rand;

        level = new int[levelWidth, levelLength];
        GetNoiseyArray();

        for (int i = 0; i < iterations; i++)
        {
            Iterate();
        }

        DrawLayout();
    }

    public void GenerateLevel(Texture2D texture)
    {
        SharedLevelData.Instance.ResetRandom();
        random = SharedLevelData.Instance.Rand;

        if(texture.width != levelWidth || texture.height != levelLength)
        {
            Debug.LogError("Texture must be the same size as the level.");
            return;
        }

        level = new int[levelWidth, levelLength];
        GetNoiseyArray();
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelLength; y++)
            {
                Color color = texture.GetPixel(x, y);
                if(color == Color.white)
                {
                    level[x, y] = 1;
                }
            }
        }

        for (int i = 0; i < iterations; i++)
        {
            Iterate();
        }

        DrawLayout();
    }

    private void Iterate()
    {
        int[,] newLevel = new int[levelWidth, levelLength];
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelLength; y++)
            {
                int neighbors = CountNeighbors(x, y);
                if (neighbors > upperBoundNeighbors)
                {
                    newLevel[x, y] = 1;
                }
                else if (neighbors < lowerBoundNeighbors)
                {
                    newLevel[x, y] = 0;
                }
                else
                {
                    newLevel[x, y] = level[x, y];
                }
            }
        }
        level = newLevel;
    }

    private void GetNoiseyArray()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelLength; y++)
            {
                level[x, y] = random.Next(0, 2);
            }
        }
    }

    private int CountNeighbors(int x, int y)
    {
        int count = 0;
        for(int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0) continue;

                int newX = x + xOffset;
                int newY = y + yOffset;

                if (newX < 0 || newX >= levelWidth || newY < 0 || newY >= levelLength) continue;

                if (level[newX, newY] == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [ContextMenu("Generate New Seed And New Level")]
    public void GenerateNewSeedAndNewLevel()
    {
        GenerateNewSeed();
        GenerateLevel();
    }

    public Texture2D GetLevelTexture()
    {
        var renderer = levelLayoutDisplay.GetComponent<Renderer>();

        // This will allow us to change the texture from the editor. But this is usually not recommended.
        var layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;
        return layoutTexture;
    }

    private void DrawLayout()
    {
        Texture2D layoutTexture = GetLevelTexture();

        layoutTexture.Reinitialize(levelWidth, levelLength);
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelLength; y++)
            {
                Color color = level[x, y] == 0 ? Color.black : Color.white;
                layoutTexture.SetPixel(x, y, color);
            }
        }

        layoutTexture.ConvertToBlackAndWhite();
        layoutTexture.SaveAsset();
    }
}
