using UnityEngine;

[ExecuteInEditMode]
public class ViewNormals : MonoBehaviour
{
    // Use this for initialization
    private MeshFilter objectMesh;

    void OnDrawGizmos()
    {
        if (objectMesh != null)
        {
            for (int i = 0; i < objectMesh.mesh.vertexCount; ++i)
            {
                var scale = objectMesh.gameObject.transform.localScale.x;
                var pos = objectMesh.gameObject.transform.position;
                var vertPos = objectMesh.sharedMesh.vertices[i] * scale;
                const float SCALE = 100.0f;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(vertPos + pos, (vertPos + (SCALE * objectMesh.sharedMesh.normals[i]) + pos));
            }
        }
        else
        {
            objectMesh = GetComponent<MeshFilter>();
        }
    }
}