using AOT;
using Wrld.Common.Maths;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld.Meshes
{
    public class MeshUploader
    {
        class UnpackedMesh
        {
            public Vector3[] vertices;

            public Vector2[] uvs;

            public Vector2[] uv2s;

            public int[] indices;

            public DoubleVector3[] originECEF;

            public string materialName;

            public string name;

            private List<GCHandle> gcHandles;
            public UnpackedMesh(int vertexCount, int uvCount, int uv2Count, int indexCount, IntPtr _name, IntPtr material)
            {
                gcHandles = new List<GCHandle>();
                vertices = new Vector3[vertexCount];
                uvs = new Vector2[uvCount];

                if (uv2Count > 0)
                {
                    uv2s = new Vector2[uv2Count];
                }

                indices = new int[indexCount];
                originECEF = new DoubleVector3[1];
                name = Marshal.PtrToStringAnsi(_name);
                materialName = Marshal.PtrToStringAnsi(material);
            }
            private IntPtr PinAndTrackHandle(object member)
            {
                var handle = GCHandle.Alloc(member, GCHandleType.Pinned);
                gcHandles.Add(handle);
                return handle.AddrOfPinnedObject();
            }

            // Create a MarshalledMesh whose IntPtrs point at the (pinned) data arrays in this mesh
            public IntPtr CreatePointerToMarshalledMesh()
            {
                var marshalledMesh = new MarshalledMesh();

                marshalledMesh.vertices = PinAndTrackHandle(vertices);
                marshalledMesh.uvs = PinAndTrackHandle(uvs);

                if (uv2s != null)
                {
                    marshalledMesh.uv2s = PinAndTrackHandle(uv2s);
                }

                marshalledMesh.indices = PinAndTrackHandle(indices);
                marshalledMesh.originEcef = PinAndTrackHandle(originECEF);

                GCHandle handle = GCHandle.Alloc(this);
                marshalledMesh.unpackedMeshHandle = GCHandle.ToIntPtr(handle);
                gcHandles.Add(handle);

                return PinAndTrackHandle(marshalledMesh);
            }

            public void FreeTrackedHandles()
            {
                foreach (var handle in gcHandles)
                {
                    handle.Free();
                }

                gcHandles.Clear();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MarshalledMesh
        {
            public IntPtr vertices;
            public int vertexCount;

            public IntPtr uvs;
            public int uvCount;

            public IntPtr uv2s;
            public int uv2Count;

            public IntPtr indices;
            public int indexCount;

            public IntPtr originEcef;

            public IntPtr name;

            public IntPtr unpackedMeshHandle;
        }

        public delegate IntPtr AllocateUnpackedMeshCallback(int vertNum, int uvsNum, int uv2sNum, int indicesNum, IntPtr meshName, IntPtr materialName);
        public delegate void UploadUnpackedMeshCallback(IntPtr meshBuffer);

        static PreparedMeshRepository m_preparedMeshes = new PreparedMeshRepository();

        public MeshUploader()
        {
        }

        [MonoPInvokeCallback(typeof(AllocateUnpackedMeshCallback))]
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public static IntPtr AllocateUnpackedMesh(int vertexCount, int uvCount, int uv2Count, int indexCount, IntPtr name, IntPtr material)
        {
            var unpackedMesh = new UnpackedMesh(vertexCount, uvCount, uv2Count, indexCount, name, material);

            return unpackedMesh.CreatePointerToMarshalledMesh();
        }

        [MonoPInvokeCallback(typeof(UploadUnpackedMeshCallback))]
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public static void UploadUnpackedMesh(IntPtr meshPtr)
        {
            MarshalledMesh marshalled = (MarshalledMesh)Marshal.PtrToStructure(meshPtr, typeof(MarshalledMesh));
            GCHandle unpackedMeshHandle = GCHandle.FromIntPtr(marshalled.unpackedMeshHandle);
            var result = unpackedMeshHandle.Target as UnpackedMesh;
            result.FreeTrackedHandles();

            // this is one of the points where we could create an identifier that we could use to delete the mesh (other than the string name)
            m_preparedMeshes.AddPreparedMeshRecord(CreatePreparedMeshRecordFromUnpackedMesh(result));
        }

        private static PreparedMeshRecord CreatePreparedMeshRecordFromUnpackedMesh(UnpackedMesh meshData)
        {
            var meshes = MeshBuilder.CreatePreparedMeshes(meshData.vertices, meshData.uvs, meshData.uv2s, meshData.indices, meshData.name, meshData.materialName, meshData.originECEF[0], 65535);

            return new PreparedMeshRecord { MaterialName = meshData.materialName, Meshes = meshes, OriginECEF = meshData.originECEF[0] };
        }

        public bool TryGetUnityMeshesForID(string id, out Mesh[] meshes, out DoubleVector3 originECEF, out string materialName)
        {
            return m_preparedMeshes.TryGetUnityMeshesForID(id, out meshes, out originECEF, out materialName);
        }
    }
}