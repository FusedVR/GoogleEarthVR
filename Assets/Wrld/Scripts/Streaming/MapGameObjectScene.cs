using AOT;
using Wrld.Common.Maths;
using Wrld.Materials;
using Wrld.Meshes;
using Wrld.Space;
using Wrld.Streaming;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld
{ 
    public class MapGameObjectScene
    {
        public delegate void AddMeshCallback(IntPtr data);
        public delegate void DeleteMeshCallback(IntPtr data);
        public delegate void VisibilityCallback(IntPtr id, bool visible);

        GameObjectStreamer m_terrainStreamer;
        GameObjectStreamer m_roadStreamer;
        GameObjectStreamer m_buildingStreamer;
        MeshUploader m_meshUploader;
        private static MapGameObjectScene ms_instance;

        public MapGameObjectScene(MaterialRepository materialRepository, Transform parentForStreamedObjects, ConfigParams.CollisionConfig collisions)
        {
            m_terrainStreamer = new GameObjectStreamer("Terrain", materialRepository, parentForStreamedObjects, collisions.TerrainCollision);
            m_roadStreamer = new GameObjectStreamer("Roads", materialRepository, parentForStreamedObjects, collisions.RoadCollision);
            m_buildingStreamer = new GameObjectStreamer("Buildings", materialRepository, parentForStreamedObjects, collisions.BuildingCollision);
            m_meshUploader = new MeshUploader();
            ms_instance = this;
        }

        public void UpdateTransforms(ITransformUpdateStrategy transformUpdateStrategy)
        {
            const float roadHeightOffset = 0.1f;
            m_terrainStreamer.UpdateTransforms(transformUpdateStrategy);
            m_roadStreamer.UpdateTransforms(transformUpdateStrategy, roadHeightOffset);
            m_buildingStreamer.UpdateTransforms(transformUpdateStrategy);
        }

        [MonoPInvokeCallback(typeof(DeleteMeshCallback))]
        public static void DeleteMesh(IntPtr meshID)
        {
            string id = Marshal.PtrToStringAnsi(meshID);
            ms_instance.DeleteMesh(id);
        }

        void DeleteMesh(string id)
        {
            var streamer = GetStreamerFromName(id);
            streamer.RemoveObjects(id);
        }

        [MonoPInvokeCallback(typeof(AddMeshCallback))]
        public static void AddMesh(IntPtr meshID)
        {
            string id = Marshal.PtrToStringAnsi(meshID);
            ms_instance.AddMesh(id);
        }

        private void AddMesh(string id)
        {
            Mesh[] meshes;
            DoubleVector3 originECEF;
            string materialName;

            if (m_meshUploader.TryGetUnityMeshesForID(id, out meshes, out originECEF, out materialName))
            {
                var streamer = GetStreamerFromName(id);
                streamer.AddObjectsForMeshes(meshes, originECEF, materialName);
            }
        }

        [MonoPInvokeCallback(typeof(VisibilityCallback))]
        public static void SetVisible(IntPtr meshID, bool visible)
        {
            string id = Marshal.PtrToStringAnsi(meshID);
            ms_instance.SetVisible(id, visible);
        }

        private void SetVisible(string id, bool visible)
        {
            var streamer = GetStreamerFromName(id);
            streamer.SetVisible(id, visible);
        }

        private GameObjectStreamer GetStreamerFromName(string name)
        {
            // :TODO: replace this string lookup with a shared type enum or similar
            if (name.StartsWith("Raster") || name.StartsWith("Terrain"))
            {
                return m_terrainStreamer;
            }

            switch (name[0])
            {
                case 'M':
                case 'L':
                    return m_terrainStreamer;
                case 'R':
                    return m_roadStreamer;
                case 'B':
                    return m_buildingStreamer;
                default:
                    throw new ArgumentException(string.Format("Unknown streamer for name: {0}", name), "name");
            }
        }
    }
}

