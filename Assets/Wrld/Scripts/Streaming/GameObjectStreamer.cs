using Wrld.Common.Maths;
using Wrld.Materials;
using Wrld.Space;
using UnityEngine;

namespace Wrld.Streaming
{
    class GameObjectStreamer
    {
        GameObjectRepository m_gameObjectRepository;
        MaterialRepository m_materialRepository;
        GameObjectFactory m_gameObjectCreator;

        private bool m_collisions;

        public GameObjectStreamer(string rootObjectName, MaterialRepository materialRepository, Transform parentForStreamedObjects, bool setUpCollisions)
        {
            m_materialRepository = materialRepository;
            m_gameObjectRepository = new GameObjectRepository(rootObjectName, parentForStreamedObjects, materialRepository);
            m_gameObjectCreator = new GameObjectFactory(m_gameObjectRepository.Root.transform);
            m_collisions = setUpCollisions;
        }

        public void AddObjectsForMeshes(Mesh[] meshes, DoubleVector3 originECEF, string materialName)
        {
            var id = meshes[0].name;
            var material = m_materialRepository.LoadOrCreateMaterial(id, materialName);
            var gameObjects = m_gameObjectCreator.CreateGameObjects(meshes, material, m_collisions);            
            m_gameObjectRepository.Add(id, originECEF, gameObjects);
        }

        public bool RemoveObjects(string id)
        {
            return m_gameObjectRepository.Remove(id);
        }

        public void UpdateTransforms(ITransformUpdateStrategy transformUpdateStrategy, float heightOffset = 0.0f)
        {
            m_gameObjectRepository.UpdateTransforms(transformUpdateStrategy, heightOffset);
        }

        public void SetVisible(string id, bool visible)
        {
            GameObject[] gameObjects;

            if (m_gameObjectRepository.TryGetGameObjects(id, out gameObjects))
            {
                foreach (var gameObject in gameObjects)
                {
                    gameObject.SetActive(visible);
                }
            }
        }
    }
}