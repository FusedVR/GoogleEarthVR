using UnityEngine;

namespace Wrld.Streaming
{
    class GameObjectFactory
    {
        private Transform m_parentTransform;

        public GameObjectFactory(Transform parentTransform)
        {
            m_parentTransform = parentTransform;
        }

        private static string CreateGameObjectName(string baseName, int meshIndex)
        {
            return string.Format("{0}_INDEX{1}", baseName, meshIndex);
        }

        private GameObject CreateGameObject(Mesh mesh, Material material, string objectName, bool createCollisionMesh)
        {
            var gameObject = new GameObject(objectName);
            gameObject.SetActive(false);
            gameObject.transform.parent = m_parentTransform;

            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
            if (createCollisionMesh)
            { 
                gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
            }
            return gameObject;
        }

        public GameObject[] CreateGameObjects(Mesh[] meshes, Material material, bool createCollisionMesh)
        {
            var gameObjects = new GameObject[meshes.Length];

            for (int meshIndex = 0; meshIndex < meshes.Length; ++meshIndex)
            {
                var name = CreateGameObjectName(meshes[meshIndex].name, meshIndex);
                gameObjects[meshIndex] = CreateGameObject(meshes[meshIndex], material, name, createCollisionMesh);
            }

            return gameObjects;
        }
    }
}