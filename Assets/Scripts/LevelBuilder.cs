using UnityEngine;
using Unity.AI.Navigation;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] RoomLayoutGenerator roomLayoutGenerator;
    [SerializeField] MarchingSquares levelGeometryGenerator;
    [SerializeField] CellularAutomataCaves levelCaves;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] RoomDecorator roomDecorator;

    [SerializeField] bool debugRooms = false;

    [SerializeField] bool buildCaves = false;
    [SerializeField] bool newSeed = true;

    [ContextMenu("Gen Level And Geometry")]
    public void GenerateLayoutAndGeometry()
    {
        if (newSeed) { roomLayoutGenerator.GenerateNewSeed(); }

        // Add base room layout.
        Level level = roomLayoutGenerator.GenerateLevel();
        Texture2D levelMapTexture = roomLayoutGenerator.GetLevelTexture();

        // Find the player's start room.
        Room startRoom = level.PlayerStartRoom;
        Vector3 startPosition = LevelPositionToWorldPosition(startRoom.Area.center);

        // Add walls and floors.
        levelGeometryGenerator.CreateLevelGeometry();

        // Add caves?
        if (buildCaves)
        {
            // Add caves.
            levelCaves.AppendCavesToLevel(levelMapTexture, Color.white);
            Texture2D caveMapTexture = levelCaves.GetLevelTexture();

            LevelMerger.MergeLevelsByTextures(caveMapTexture, levelMapTexture, Color.black, TextureBasedLevel.brown);

            // Flood fill the cave map starting from the player start room.
            caveMapTexture.FloodFill(startRoom.Area.center, Color.cyan);

            // Remove white sections from cave map.
            caveMapTexture.ReplaceColor(Color.white, Color.black);
            caveMapTexture.ConvertToBlackAndWhite();

            // Subtract the layout map from the cave map.
            caveMapTexture.SubtractPixels(levelMapTexture, Color.white, Color.black);

            caveMapTexture.RemoveSmallRegions(Color.white, levelCaves.MinRegionSize);

            // Add cave walls.
            levelGeometryGenerator.AppendLevelGeometry(caveMapTexture);

            // Add cave data to level map.
            LevelMerger.MergeLevelsByTextures(levelMapTexture, caveMapTexture, Color.black, TextureBasedLevel.brown);
        }

        RoomGraph graph = RoomGraphUtility.BuildRoomGraph(levelMapTexture);
        Debug.Log($"Detected {graph.Rooms.Count} rooms");

        if (debugRooms)
        {
            Color[] palette = new Color[graph.Rooms.Count];
            for (int i = 0; i < palette.Length; i++)
            {
                // Generate bright colors with random hues.
                palette[i] = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            }
            foreach (var pair in graph.Rooms)
            {
                Color c = palette[pair.Key % palette.Length];
                foreach (var pos in pair.Value)
                {
                    levelMapTexture.SetPixel(pos.x, pos.y, c);
                }
            }
            foreach (var door in graph.Doors)
            {
                levelMapTexture.SetPixel(door.Position.x, door.Position.y, Color.white);
            }
            levelMapTexture.SaveAsset();
        }

        // Add decorations.
        roomDecorator.PlaceItems(level);
        
        // Add nav mesh.
        navMeshSurface.BuildNavMesh();

        // Why not make the player a property of LevelBuilder and instantiate it?
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = startPosition;
    }

    private Vector3 LevelPositionToWorldPosition(Vector2 levelPosition)
    {
        int scale = SharedLevelData.Instance.Scale;
        return new Vector3(
            (levelPosition.x - 1) * scale,
            0f,
            (levelPosition.y - 1) * scale
        );
    }

    private void Start()
    {
        //GenerateLayoutAndGeometry();
    }
}
