using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    public void BakeNavMesh()
    {
        surface.BuildNavMesh();
        Debug.Log("NavMesh baked successfully!");
    }
}
