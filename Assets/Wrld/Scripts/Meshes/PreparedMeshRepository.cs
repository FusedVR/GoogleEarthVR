using Wrld.Common.Maths;
using System.Collections.Generic;
using UnityEngine;

namespace Wrld.Meshes
{
    class PreparedMeshRecord
    {
        public PreparedMesh[] Meshes { get; set; }
        public DoubleVector3 OriginECEF { get; set; }
        public string MaterialName { get; set; }
    }

    class PreparedMeshRepository
    {
        private Dictionary<string, PreparedMeshRecord> m_meshRecords = new Dictionary<string, PreparedMeshRecord>();

        public void AddPreparedMeshRecord(PreparedMeshRecord record)
        {
            lock (m_meshRecords)
            {
                string meshID = record.Meshes[0].Name;

                if (!m_meshRecords.ContainsKey(meshID))
                { 
                    m_meshRecords.Add(meshID, record);
                }
            }
        }

        public bool TryGetUnityMeshesForID(string id, out Mesh[] meshes, out DoubleVector3 originECEF, out string materialName)
        {
            meshes = null;
            originECEF = DoubleVector3.zero;
            materialName = null;

            PreparedMeshRecord record;
            bool foundRecord;

            lock (m_meshRecords)
            {
                foundRecord = m_meshRecords.TryGetValue(id, out record);

                if (foundRecord)
                {
                    m_meshRecords.Remove(id);
                }
            }

            if (foundRecord)
            {
                meshes = new Mesh[record.Meshes.Length];

                for (int meshIndex = 0; meshIndex < meshes.Length; ++meshIndex)
                {
                    meshes[meshIndex] = record.Meshes[meshIndex].ToUnityMesh();
                }

                originECEF = record.OriginECEF;
                materialName = record.MaterialName;
            }
            else
            {
                Debug.LogFormat("Error! Could not find mesh with id {0}", id);
            }

            return foundRecord;
        }
    }
}