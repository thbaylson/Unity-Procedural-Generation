using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CellularAutomataCaves : MonoBehaviour
{
    //[SerializeField] RoomLevelLayoutConfig levelConfig;
    [SerializeField] private int levelWidth = 64;
    [SerializeField] private int levelLength = 64;
    [SerializeField] private int upperBoundNeighbors = 4;
    [SerializeField] private int lowerBoundNeighbors = 3;
    [SerializeField] private int iterations = 5;
    [SerializeField] private int borderSize = 3;

    [Header("Level Layout Display")]
    [SerializeField] GameObject levelLayoutDisplay;

    private Random random;

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

        int borderlessWidth = levelWidth - borderSize * 2;
        int borderlessLength = levelLength - borderSize * 2;

        int[,] borderlessLevel = new int[borderlessWidth, borderlessLength];
        GetNoiseyArray(borderlessLevel);

        for (int i = 0; i < iterations; i++)
        {
            borderlessLevel = Iterate(borderlessLevel);
        }

        // Add border to the level.
        int[,] borderedLevel = new int[levelWidth, levelLength];
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelLength; y++)
            {
                if (x < borderSize || x >= levelWidth - borderSize || y < borderSize || y >= levelLength - borderSize)
                {
                    borderedLevel[x, y] = 0;
                }
                else
                {
                    borderedLevel[x, y] = borderlessLevel[x - borderSize, y - borderSize];
                }
            }
        }

        DrawLayout(borderedLevel);
    }

    public void AppendCavesToLevel(Texture2D texture, Color floorColor)
    {
        levelWidth = texture.width;
        levelLength = texture.height;

        SharedLevelData.Instance.ResetRandom();
        random = SharedLevelData.Instance.Rand;

        int borderlessWidth = levelWidth - borderSize * 2;
        int borderlessLength = levelLength - borderSize * 2;

        int[,] borderlessLevel = new int[borderlessWidth, borderlessLength];
        GetNoiseyArray(borderlessLevel);

        for (int i = 0; i < iterations; i++)
        {
            borderlessLevel = Iterate(borderlessLevel);
        }

        // Copy walkable areas from the texture into the level array.
        for (int x = 0; x < borderlessWidth; x++)
        {
            for (int y = 0; y < borderlessLength; y++)
            {
                Color color = texture.GetPixel(x + borderSize, y + borderSize);
                if(color == floorColor)
                {
                    borderlessLevel[x, y] = 1;
                }
            }
        }

        // Add border to the level.
        int[,] borderedLevel = new int[levelWidth, levelLength];
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelLength; y++)
            {
                if (x < borderSize || x >= levelWidth - borderSize || y < borderSize || y >= levelLength - borderSize)
                {
                    borderedLevel[x, y] = 0;
                }
                else
                {
                    borderedLevel[x, y] = borderlessLevel[x - borderSize, y - borderSize];
                }
            }
        }

        DrawLayout(borderedLevel);
    }

    private int[,] Iterate(int[,] levelArray)
    {
        int width = levelArray.GetLength(0);
        int length = levelArray.GetLength(1);
        int[,] newLevel = new int[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                int neighbors = CountNeighbors(levelArray, x, y);
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
                    newLevel[x, y] = levelArray[x, y];
                }
            }
        }

        return newLevel;
    }

    private void GetNoiseyArray(int[,] levelArray)
    {
        int width = levelArray.GetLength(0);
        int length = levelArray.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                levelArray[x, y] = random.Next(0, 2);
            }
        }
    }

    private int CountNeighbors(int[,] levelArray, int x, int y)
    {
        int width = levelArray.GetLength(0);
        int length = levelArray.GetLength(1);

        int count = 0;
        for(int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0) continue;

                int newX = x + xOffset;
                int newY = y + yOffset;

                if (newX < 0 || newX >= width || newY < 0 || newY >= length) continue;

                if (levelArray[newX, newY] == 1)
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

    private void DrawLayout(int[,] levelArray)
    {
        int width = levelArray.GetLength(0);
        int length = levelArray.GetLength(1);

        Texture2D layoutTexture = GetLevelTexture();
        layoutTexture.Reinitialize(width, length);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                Color color = levelArray[x, y] == 0 ? Color.black : Color.white;
                layoutTexture.SetPixel(x, y, color);
            }
        }

        layoutTexture.SaveAsset();
    }
}
