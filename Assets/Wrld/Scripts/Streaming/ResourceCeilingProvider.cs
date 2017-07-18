using Wrld.Resources.Terrain.Heights;

namespace Wrld.Streaming
{
    public class ResourceCeilingProvider
    {
        public ResourceCeilingProvider(TerrainHeightProvider terrainHeightProvider)
        {
            m_terrainHeightProvider = terrainHeightProvider;
        }

        public float GetApproximateResourceCeilingAltitude()
        {
            return m_terrainHeightProvider.GetMaxHeight();
        }

        public float GetApproximateResourceFloorAltitude()
        {
            return m_terrainHeightProvider.GetMinHeight();
        }

        TerrainHeightProvider m_terrainHeightProvider;
    };
}