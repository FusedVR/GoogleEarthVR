using Wrld.Common.Maths;
using Wrld.Materials;
using Wrld.Space;
using System.Collections.Generic;
using UnityEngine;

namespace Wrld.Streaming
{
    public class GameObjectRecord
    {
        public DoubleVector3 OriginECEF { get; set; }
        public GameObject[] GameObjects { get; set; }
    }

    public class GameObjectRepository
    {
        private Dictionary<string, GameObjectRecord> m_gameObjectsById = new Dictionary<string, GameObjectRecord>();
        private GameObject m_root;
        private MaterialRepository m_materialRepository;

        public GameObject Root { get { return m_root; } }
        

        public GameObjectRepository(string rootName, Transform parentForStreamedObjects, MaterialRepository materialRepository)
        {
            m_root = new GameObject(rootName);
            m_root.transform.parent = parentForStreamedObjects;
            m_materialRepository = materialRepository;
        }

        public void Add(string id, DoubleVector3 originEcef, GameObject[] gameObjects)
        {
            if(m_gameObjectsById.ContainsKey(id))
            {
                return; // :TODO: fix
            }

            var record = new GameObjectRecord { OriginECEF = originEcef, GameObjects = gameObjects };
            m_gameObjectsById.Add(id, record);
        }

        public bool Remove(string id)
        {
            GameObjectRecord value;

            if (m_gameObjectsById.TryGetValue(id, out value))
            {
                m_gameObjectsById.Remove(id);

                foreach (var gameObject in value.GameObjects)
                {
                    var mesh = gameObject.GetComponent<MeshFilter>();

                    if (mesh != null)
                    {
                        GameObject.DestroyImmediate(mesh.sharedMesh);
                    }

                    var meshRenderer = gameObject.GetComponent<MeshRenderer>();

                    if (meshRenderer != null)
                    {
                        var material = meshRenderer.sharedMaterial;
                        
                        if (material != null)
                        {
                            m_materialRepository.ReleaseMaterial(material.name);
                        }
                    }

                    GameObject.DestroyImmediate(gameObject);
                }

                return true;
            }

            return false;
        }

        public void UpdateTransforms(ITransformUpdateStrategy transformUpdateStrategy, float heightOffset)
        {
            foreach (var record in m_gameObjectsById.Values)
            {
                foreach (var gameObject in record.GameObjects)
                {
                    transformUpdateStrategy.UpdateTransform(gameObject.transform, record.OriginECEF, heightOffset);
                }
            }
        }

        public bool TryGetGameObjects(string id, out GameObject[] gameObjects)
        {
            GameObjectRecord record;

            if (m_gameObjectsById.TryGetValue(id, out record))
            {
                gameObjects = record.GameObjects;
                return true;
            }
            else
            {
                gameObjects = null;
            }

            return false;
        }
    }
}
