using UnityEngine;
using Unity.AI.Navigation;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] RoomLayoutGenerator roomLayoutGenerator;
    [SerializeField] MarchingSquares levelGeometryGenerator;
    [SerializeField] CellularAutomataCaves levelCaves;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] RoomDecorator roomDecorator;

    [ContextMenu("Gen Level And Geometry")]
    public void GenerateLayoutAndGeometry()
    {
        // Add base room layout.
        roomLayoutGenerator.GenerateNewSeed();
        Level level = roomLayoutGenerator.GenerateLevel();

        // Add caves.
        levelCaves.GenerateLevel(roomLayoutGenerator.GetLevelTexture());
        LevelMerger.MergeLevelsByTextures(roomLayoutGenerator.GetLevelTexture(), levelCaves.GetLevelTexture());

        // Add decorations.
        levelGeometryGenerator.CreateLevelGeometry();
        roomDecorator.PlaceItems(level);
        
        // Add nav mesh.
        navMeshSurface.BuildNavMesh();

        Room startRoom = level.PlayerStartRoom;
        Vector3 startPosition = LevelPositionToWorldPosition(startRoom.Area.center);

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
