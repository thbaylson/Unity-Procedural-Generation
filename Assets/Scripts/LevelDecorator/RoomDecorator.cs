using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RoomDecorator : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] RoomLayoutGenerator roomLayoutGenerator;
    [SerializeField] BaseDecoratorRule[] availableRules;
    [SerializeField] Texture2D levelTexture;
    [SerializeField] Texture2D decoratedTexture;

    private Random random;

    [ContextMenu("Place Items")]
    public void PlaceItemsFromMenu()
    {
        SharedLevelData.Instance.ResetRandom();
        Level level = roomLayoutGenerator.GenerateLevel();
        PlaceItems(level);
    }

    public void PlaceItems(Level level)
    {
        random = SharedLevelData.Instance.Rand;
        Transform decorationsTransform = parent.transform.Find("Decorations");
        if (decorationsTransform == null)
        {
            decorationsTransform = new GameObject("Decorations").transform;
            decorationsTransform.SetParent(parent.transform);
        }
        else
        {
            decorationsTransform.DestroyAllChildren();
        }

        TileType[,] levelDecorated = InitializeDecoratorArray();
        DecorateRoom(levelDecorated, level.Rooms[0], decorationsTransform);
        GenerateTextureFromTileType(levelDecorated);
    }

    private TileType[,] InitializeDecoratorArray()
    {
        TileType[,] levelDecorated = new TileType[levelTexture.width, levelTexture.height];
        for (int y = 0; y < levelTexture.height; y++)
        {
            for (int x = 0; x < levelTexture.width; x++)
            {
                Color pixelColor = levelTexture.GetPixel(x, y);
                if (pixelColor == Color.black)
                {
                    levelDecorated[x, y] = TileType.Wall;
                }
                else
                {
                    levelDecorated[x, y] = TileType.Floor;
                }
            }
        }

        return levelDecorated;
    }

    private void DecorateRoom(TileType[,] levelDecorated, Room room, Transform decorationsTransform)
    {
        BaseDecoratorRule selectedRule = availableRules[0];
        if (selectedRule.CanBeApplied(levelDecorated, room))
        {
            selectedRule.Apply(levelDecorated, room, decorationsTransform);
        }
    }

    private void GenerateTextureFromTileType(TileType[,] tileTypes)
    {
        int width = tileTypes.GetLength(0);
        int height = tileTypes.GetLength(1);

        Color32[] pixels = new Color32[width * height];
        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[x + y * width] = tileTypes[x, y].GetColor();
            }
        }

        decoratedTexture.Reinitialize(width, height);
        decoratedTexture.SetPixels32(pixels);
        decoratedTexture.Apply();
        decoratedTexture.SaveAsset();
    }
}
