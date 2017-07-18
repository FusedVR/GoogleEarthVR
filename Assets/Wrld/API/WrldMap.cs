using UnityEngine;
using Wrld;
using Wrld.Scripts.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WrldMap : MonoBehaviour
{
    [Header("Camera/View Settings")]
    [Tooltip("Camera used to request streamed resources")]
    [SerializeField]
    private Camera m_streamingCamera;

    [Header("Setup")]
    [SerializeField]
    private string m_apiKey;

    [Tooltip("In degrees")]
    [Range(-90.0f, 90.0f)]
    [SerializeField]
    private double m_latitudeDegrees = 37.771092;

    [Tooltip("In degrees")]
    [Range(-180.0f, 180.0f)]
    [SerializeField]
    private double m_longitudeDegrees = -122.468385;

    [Tooltip("The distance of the camera from the interest point (meters)")]
    [SerializeField]
    [Range(300.0f, 7000000.0f)]
    private double m_distanceToInterest = 1781.0;

    [Tooltip("Direction you are facing")]
    [SerializeField]
    [Range(0, 360.0f)]
    private double m_headingDegrees = 0.0;

    [Header("Map Behaviour Settings")]
    [Tooltip("Coordinate System to be used. ECEF requires additional calculation and setup")]
    [SerializeField]
    private CoordinateSystem m_coordSystem = CoordinateSystem.UnityWorld;

    [Header("Theme Settings")]
    [Tooltip("Directory within the Resources folder to look for materials during runtime. Default is provided with the package")]
    [SerializeField]
    private string m_materialDirectory = "WrldMaterials/";

    [Tooltip("The material to override all landmarks with. Uses standard diffuse if null.")]
    [SerializeField]
    private Material m_overrideLandmarkMaterial = null;

    [Tooltip("Set to true to use the default mouse & keyboard/touch controls, false if controlling the camera by some other means.")]
    [SerializeField]
    private bool m_useBuiltInCameraControls;

    [Header("Collision Settings")]
    [Tooltip("Set to true for Terrain collisions")]
    [SerializeField]
    private bool m_terrainCollisions;

    [Tooltip("Set to true for Road collision")]
    [SerializeField]
    private bool m_roadCollisions;

    [Tooltip("Set to true for Building collision")]
    [SerializeField]
    private bool m_buildingCollisions;

    private Api m_api;

    void Awake()
    {
        var defaultConfig = ConfigParams.MakeDefaultConfig();
        defaultConfig.DistanceToInterest = m_distanceToInterest;
        defaultConfig.LatitudeDegrees = m_latitudeDegrees;
        defaultConfig.LongitudeDegrees = m_longitudeDegrees;
        defaultConfig.HeadingDegrees = m_headingDegrees;
        defaultConfig.MaterialsDirectory = m_materialDirectory;
        defaultConfig.OverrideLandmarkMaterial = m_overrideLandmarkMaterial;
        defaultConfig.Collisions.TerrainCollision = m_terrainCollisions;
        defaultConfig.Collisions.RoadCollision = m_roadCollisions;
        defaultConfig.Collisions.BuildingCollision = m_buildingCollisions;

        Transform rootTransform = null;

        if (m_coordSystem == CoordinateSystem.UnityWorld)
        {
            rootTransform = transform;
        }

        try
        {
            Api.Create(m_apiKey, m_coordSystem, rootTransform, defaultConfig);
        }
        catch (InvalidApiKeyException)
        {
            #if UNITY_EDITOR
            EditorUtility.DisplayDialog(
                "Error: Invalid WRLD API Key",
                string.Format("Please enter a valid WRLD API Key (see the WrldMap script on GameObject \"{0}\" in the Inspector).",
                    gameObject.name),
                "OK");
            #endif
            throw;
        }

        m_api = Api.Instance;

        if (m_useBuiltInCameraControls)
        {
            m_api.CameraApi.SetControlledCamera(m_streamingCamera);
        }
    }

    double prevLat = 0f;
    double prevLong = 0f;

    void Update ()
    {
        m_api.Update();
        m_api.StreamResourcesForCamera(m_streamingCamera);
    }

#if UNITY_EDITOR
    void OnDestroy()
    {
        OnApplicationQuit();
    }
#endif

    void OnApplicationQuit()
    {
        m_api.Destroy();
    }

    void OnApplicationPause(bool isPaused)
    {
        SetApplicationPaused(isPaused);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        SetApplicationPaused(!hasFocus);
    }

    void SetApplicationPaused(bool isPaused)
    {
        if (isPaused)
        {
            m_api.OnApplicationPaused();
        }
        else
        {
            m_api.OnApplicationResumed();
        }
    }
}
