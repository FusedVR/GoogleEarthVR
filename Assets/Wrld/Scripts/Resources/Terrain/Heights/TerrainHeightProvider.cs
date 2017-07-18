using System;
using System.Runtime.InteropServices;
using Wrld.Common.Maths;

namespace Wrld.Resources.Terrain.Heights
{

public class TerrainHeightProvider
{
    const string DLL = NativePluginRunner.DLL;

    [DllImport(DLL)]
    private static extern bool TryGetTerrainHeight(IntPtr ptr, double ecefX, double ecefY, double ecefZ, ref float out_terrainHeight);

    [DllImport(DLL)]
    private static extern float GetMinTerrainHeight(IntPtr ptr);

    [DllImport(DLL)]
    private static extern float GetMaxTerrainHeight(IntPtr ptr);

    public bool TryGetHeight(DoubleVector3 ecefPosition, out float out_terrainHeight)
    {
        out_terrainHeight = 0.0f;

        return TryGetTerrainHeight(NativePluginRunner.API, 
            ecefPosition.x, ecefPosition.y, ecefPosition.z, 
            ref out_terrainHeight);
    }

    public float GetMinHeight()
    {
        return GetMinTerrainHeight(NativePluginRunner.API);
    }

    public float GetMaxHeight()
    {
        return GetMaxTerrainHeight(NativePluginRunner.API);
    }
};

};