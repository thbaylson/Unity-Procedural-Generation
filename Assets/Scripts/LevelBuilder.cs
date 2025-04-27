using UnityEngine;
using Unity.AI.Navigation;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] RoomLayoutGenerator roomLayoutGenerator;
    [SerializeField] MarchingSquares levelGeometryGenerator;
    [SerializeField] NavMeshSurface navMeshSurface;

    [ContextMenu("Regenerate Level Layout")]
    public void RegenerateLevelLayout()
    {
        roomLayoutGenerator.GenerateLevel();
    }

    [ContextMenu("Generate Level Layout")]
    public void GenerateRandomLayout()
    {
        roomLayoutGenerator.GenerateNewSeed();
        RegenerateLevelLayout();
    }

    [ContextMenu("Regenerate Geometry")]
    public void RegenerateGeometry()
    {
        levelGeometryGenerator.CreateLevelGeometry();
        navMeshSurface.BuildNavMesh();
    }

    [ContextMenu("Generate Level Layout And Geometry")]
    public void GenerateLayoutAndGeometry()
    {
        GenerateRandomLayout();
        RegenerateGeometry();
    }

    private void Start()
    {
        GenerateLayoutAndGeometry();
    }
}
