using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld
{
    /// <summary>
    /// These are parameters required during initialization and construction of the map.
    /// They are only used for startup. Please use the [API endpoints]({{ site.baseurl }}/docs/api) to interact with the map.
    /// </summary>
    public struct ConfigParams
    {
        //only used for types that are sent across to the native side
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeConfig
        {
            public double m_latitudeDegrees;
            public double m_longitudeDegrees;
            public double m_distanceToInterest;
            public double m_headingDegrees;
        }

        NativeConfig m_nativeConfig;

        /// <summary>
        /// Get the native config which can be used across language boundaries.
        /// For internal use only.
        /// </summary>
        public NativeConfig GetNativeConfig()
        {
            return m_nativeConfig;
        }

        /// <summary>
        /// Starting Latitude in degrees
        /// </summary>
        public double LatitudeDegrees
        {
            get { return m_nativeConfig.m_latitudeDegrees; }
            set { m_nativeConfig.m_latitudeDegrees = value; }
        }

        /// <summary>
        /// Starting Longitude in degrees
        /// </summary>
        public double LongitudeDegrees
        {
            get { return m_nativeConfig.m_longitudeDegrees; }
            set { m_nativeConfig.m_longitudeDegrees = value; }
        }

        /// <summary>
        /// Starting distance of the camera from the interest point, in meters.
        /// </summary>
        public double DistanceToInterest
        {
            get { return m_nativeConfig.m_distanceToInterest; }
            set { m_nativeConfig.m_distanceToInterest = value; }
        }

        /// <summary>
        /// Starting heading / direction in degrees
        /// </summary>
        public double HeadingDegrees
        {
            get { return m_nativeConfig.m_headingDegrees; }
            set { m_nativeConfig.m_headingDegrees = value; }
        }

        /// <summary>
        /// Default materials lookup directory with existing semantically named materials required for runtime assignment.
        /// </summary>
        public string MaterialsDirectory;

        /// <summary>
        /// Default material to be assigned to Wrld Landmarks.
        /// Landmarks are special buildings which are dotted around the globe, they have a custom texture
        /// which will be automatically assigned to this materials diffuse value. Setting this value to null uses
        /// a standard diffuse material.
        /// </summary>
        public Material OverrideLandmarkMaterial;

        /// <summary>
        /// The override URL pointing to a valid coverage tree binary file
        /// </summary>
        public string CoverageTreeManifestUrl;

        /// <summary>
        /// The override URL pointing to a valid manifest with theme information, also a binary file
        /// </summary>
        public string ThemeManifestUrl;

        /// <summary>
        /// The Collision structure that holds whether to create collision meshes for different types of map geometry.
        /// </summary>
        public struct CollisionConfig
        {
            /// <summary>
            /// Will cause collision geometry to be generated for terrain meshes if true.
            /// </summary>
            public bool TerrainCollision { get; set; }
            
            /// <summary>
            /// Will cause collision geometry to be generated for road meshes if true.
            /// </summary>
            public bool RoadCollision { get; set; }

            /// <summary>
            /// Will cause collision geometry to be generated for building meshes if true.
            /// </summary>
            public bool BuildingCollision { get; set; }
        }

        /// <summary>
        /// Contains information on whether to enable or disable generation of collision meshes for different types of map geometry.
        /// </summary>
        public CollisionConfig Collisions;

        /// <summary>
        /// Create and return a default configuration which sets up all parameters to default values.
        /// The map starts up at San Francisco (37.771092, -122.468385) at an altitude above sea level of
        /// 1781 meters while facing North (0 degrees heading). The default directory is set to WrldMaterials, which
        /// points to Assets/Wrld/Resources/WrldMaterials. And default landmark material is set to null
        /// </summary>
        public static ConfigParams MakeDefaultConfig()
        {
            var config = new ConfigParams();

            config.m_nativeConfig = new NativeConfig();

            config.LatitudeDegrees = 37.771092;
            config.LongitudeDegrees = -122.468385;
            config.DistanceToInterest = 1781.0;
            config.HeadingDegrees = 0.0;

            config.MaterialsDirectory = "WrldMaterials/";
            config.OverrideLandmarkMaterial = null;

            config.CoverageTreeManifestUrl = "";
            config.ThemeManifestUrl = "";

            config.Collisions = new CollisionConfig { TerrainCollision = false, RoadCollision = false, BuildingCollision = false };

            return config;
        }
    }
}