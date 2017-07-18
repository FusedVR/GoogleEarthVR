using Wrld.Common.Maths;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wrld.Meshes
{
    public static class MeshBuilder
    {
        public static PreparedMesh[] CreatePreparedMeshes(Vector3[] verts, Vector2[] uvs, Vector2[] uv2s, int[] indices, string name, string materialName, DoubleVector3 originECEF, int maxVerticesPerMesh)
        {
            var meshes = new List<PreparedMesh>();

            if (verts.Length <= maxVerticesPerMesh)
            {
                meshes.Add(PreparedMesh.CreateFromArrays(verts, uvs, uv2s, indices, name, materialName, originECEF));
            }
            else
            {
                // there's probably a lot to be done to optimize this, but we're still to meet about re-chunking etc.
                int triangleCount = indices.Length / 3;

                for (int startingTriangleIndex = 0; startingTriangleIndex < triangleCount;)
                {
                    var indexRemapper = new Dictionary<int, int>();
                    int triangleIndex = startingTriangleIndex;

                    for (; triangleIndex < triangleCount; ++triangleIndex)
                    {
                        if ((indexRemapper.Count + 3) > maxVerticesPerMesh)
                        {
                            break;
                        }

                        for (int vertexIndex = 0; vertexIndex < 3; ++vertexIndex)
                        {
                            int originalIndex = indices[triangleIndex * 3 + vertexIndex];

                            if (!indexRemapper.ContainsKey(originalIndex))
                            {
                                indexRemapper.Add(originalIndex, indexRemapper.Count);
                            }
                        }
                    }

                    var reversedRemapping = indexRemapper.ToDictionary(_x => _x.Value, _x => _x.Key);

                    var remappedVerts = Enumerable.Range(0, reversedRemapping.Count).Select(_i => verts[reversedRemapping[_i]]).ToArray();
                    var remappedUVs = Enumerable.Range(0, reversedRemapping.Count).Select(_i => uvs[reversedRemapping[_i]]).ToArray();
                    var remappedUV2s = Enumerable.Range(0, reversedRemapping.Count).Select(_i => uv2s[reversedRemapping[_i]]).ToArray();
                    var remappedIndices = indices.Skip(startingTriangleIndex * 3).Take((triangleIndex - startingTriangleIndex) * 3).Select(_i => indexRemapper[_i]).ToArray();

                    // use the remapped indices
                    meshes.Add(PreparedMesh.CreateFromArrays(remappedVerts, remappedUVs, remappedUV2s, remappedIndices, name, materialName, originECEF));
                    startingTriangleIndex = triangleIndex;
                }
            }

            return meshes.ToArray();
        }
    }
}