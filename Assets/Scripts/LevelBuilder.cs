using UnityEngine;
using Unity.AI.Navigation;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] RoomLayoutGenerator roomLayoutGenerator;
    [SerializeField] MarchingSquares levelGeometryGenerator;
    [SerializeField] NavMeshSurface navMeshSurface;

    //[ContextMenu("Regen Level Layout")]
    //public void RegenerateLevelLayout()
    //{
    //    roomLayoutGenerator.GenerateLevel();
    //}

    //[ContextMenu("Gen Level Layout")]
    //public void GenerateRandomLayout()
    //{
    //    roomLayoutGenerator.GenerateNewSeed();
    //    RegenerateLevelLayout();
    //}

    //[ContextMenu("Regen Geometry")]
    //public void RegenerateGeometry()
    //{
    //    levelGeometryGenerator.CreateLevelGeometry();
    //    navMeshSurface.BuildNavMesh();
    //}

    [ContextMenu("Gen Level And Geometry")]
    public void GenerateLayoutAndGeometry()
    {
        //GenerateRandomLayout();
        //RegenerateGeometry();

        roomLayoutGenerator.GenerateNewSeed();
        Level level = roomLayoutGenerator.GenerateLevel();
        levelGeometryGenerator.CreateLevelGeometry();
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
