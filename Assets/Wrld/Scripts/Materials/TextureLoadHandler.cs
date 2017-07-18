using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Wrld.Concurrency;

namespace Wrld.Materials
{
    public class TextureLoadHandler
    {
        const string DLL = NativePluginRunner.DLL;
        class TextureBuffer
        {
            public byte[] allocatedBuffer;
            public int width;
            public int height;
            public TextureFormat format;
            public bool mipMaps;
            public uint id;
            private List<GCHandle> gcHandles;

            public TextureBuffer(int _sizeInBytes, int _width, int _height, int _format, bool _mipmaps, uint _id)
            {
                allocatedBuffer = new byte[_sizeInBytes];
                gcHandles = new List<GCHandle>();
                width = _width;
                height = _height;
                format = (TextureFormat)_format;
                mipMaps = _mipmaps;
                id = _id;
            }
            public IntPtr PinAndTrackHandle(object member)
            {
                var handle = GCHandle.Alloc(member, GCHandleType.Pinned);
                gcHandles.Add(handle);
                return handle.AddrOfPinnedObject();
            }
            public void FreeTrackedHandles()
            {
                foreach (var handle in gcHandles)
                {
                    handle.Free();
                }

                gcHandles.Clear();
            }

            // Create a MarshalledMesh whose IntPtrs point at the (pinned) data arrays in this mesh
            public IntPtr CreatePointerToMarshalledTextureBuffer()
            {
                var marshalled = new MarshalledTextureBuffer();

                marshalled.format = (int)format;
                marshalled.height = height;
                marshalled.width = width;
                marshalled.mipMaps = mipMaps;
                marshalled.sizeInBytes = allocatedBuffer.Length;
                marshalled.data = PinAndTrackHandle(allocatedBuffer);

                GCHandle handle = GCHandle.Alloc(this);
                marshalled.textureBufferHandle = GCHandle.ToIntPtr(handle);
                gcHandles.Add(handle);

                return PinAndTrackHandle(marshalled);
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        struct MarshalledTextureBuffer
        {
            public IntPtr data;
            public int sizeInBytes;
            public int width;
            public int height;
            public int format;
            private byte _mipMaps;
            public bool mipMaps
            {
                get { return _mipMaps != 0; }
                set { _mipMaps = (byte)(value ? 1 : 0); }
            }
            public IntPtr textureBufferHandle;
        };

        internal delegate IntPtr AllocateTextureBufferCallback(int size, int width, int height, int format, int hasMipMaps);
        internal delegate uint BeginUploadTextureBufferCallback(IntPtr buffer);
        internal delegate void ReleaseTextureCallback(uint texture);

        Dictionary<uint, Texture> m_builtTextures = new Dictionary<uint, Texture>();
        ConcurrentQueue<TextureBuffer> m_processQueue = new ConcurrentQueue<TextureBuffer>();
        IdGenerator m_idGenerator = new IdGenerator();
        static TextureLoadHandler ms_instance;

        public TextureLoadHandler()
        {
            ms_instance = this;
        }

        // Update is called once per frame
        public void Update()
        {
            while (CreateTexture());
        }

        [MonoPInvokeCallback(typeof(AllocateTextureBufferCallback))]
        internal static IntPtr AllocateTextureBuffer(int size, int width, int height, int format, int hasMipMaps)
        {
            return ms_instance.AllocateTextureBufferInternal(size, width, height, format, hasMipMaps != 0);
        }

        [MonoPInvokeCallback(typeof(BeginUploadTextureBufferCallback))]
        internal static uint BeginUploadTextureBuffer(IntPtr bufferPtr)
        {
            return ms_instance.BeginUploadTextureBufferInternal(bufferPtr);
        }

        [MonoPInvokeCallback(typeof(ReleaseTextureCallback))]
        internal static void ReleaseTexture(uint id)
        {
            ms_instance.ReleaseTextureInternal(id);
        }

        private bool CreateTexture()
        {
            TextureBuffer buffer;

            if (!m_processQueue.TryDequeue(out buffer))
            {
                return false;
            }

            Debug.Assert(!m_builtTextures.ContainsKey(buffer.id));

            var texture = new Texture2D(buffer.width, buffer.height, buffer.format, buffer.mipMaps);
            texture.wrapMode = buffer.format == TextureFormat.RGB565 ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;
            texture.LoadRawTextureData(buffer.allocatedBuffer);
            texture.Apply(updateMipmaps: true, makeNoLongerReadable: false);

            m_builtTextures.Add(buffer.id, texture);

            return true;
        }

        public Texture GetTexture(uint id)
        {
            return m_builtTextures.ContainsKey(id) ? m_builtTextures[id] : null;
        }
        private IntPtr AllocateTextureBufferInternal(int size, int width, int height, int format, bool hasMipMaps)
        {
            // :TODO: do we need to know if linear?
            TextureBuffer textureBuffer = new TextureBuffer(size, width, height, format, hasMipMaps, m_idGenerator.GetNextID());
            return textureBuffer.CreatePointerToMarshalledTextureBuffer();
        }

        private uint BeginUploadTextureBufferInternal(IntPtr bufferPtr)
        {
            MarshalledTextureBuffer marshalled = (MarshalledTextureBuffer)Marshal.PtrToStructure(bufferPtr, typeof(MarshalledTextureBuffer));
            GCHandle textureBufferHandle = GCHandle.FromIntPtr(marshalled.textureBufferHandle);
            var result = textureBufferHandle.Target as TextureBuffer;
            result.FreeTrackedHandles();

            // this is one of the points where we could create an identifier that we could use to delete (other than the string name)
            m_processQueue.Enqueue(result);

            return result.id;
        }

        private void ReleaseTextureInternal(uint id)
        {
            Texture value;

            if (m_builtTextures.TryGetValue(id, out value))
            {
                m_idGenerator.FreeID(id);
                m_builtTextures.Remove(id);
                GameObject.DestroyImmediate(value);
            }
        }
    }
}