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
        roomLayoutGenerator.GenerateLevel();
        levelGeometryGenerator.CreateLevelGeometry();
        navMeshSurface.BuildNavMesh();
    }

    private void Start()
    {
        GenerateLayoutAndGeometry();
    }
}
