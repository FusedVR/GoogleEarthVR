using Wrld;
using UnityEngine;

public class SeparateStreamingAndRenderingExample : MonoBehaviour
{
    // public only so they can be easily modified in the Inspector
    public string apiKey = "";
    public Camera streamingCamera = null;

    void Start ()
    {
        // start up in the default position
        var config = ConfigParams.MakeDefaultConfig();
        Api.Create(apiKey, CoordinateSystem.UnityWorld, transform, config);
        
        // stream the resources attached to the specified camera
        if (!streamingCamera)
        {
            streamingCamera = GameObject.Find("FixedStreamingCamera").GetComponent<Camera>();
        }

        // Stream in the resources for the area viewed by FixedStreamingCamera.  In this example we're calling
        // it only once, at startup time because the streaming camera doesn't move.
        Api.Instance.StreamResourcesForCamera(streamingCamera);
    }

    void Update ()
    {
        Api.Instance.Update();
    }
    void OnApplicationQuit()
    {
        Api.Instance.Destroy();
    }
}
