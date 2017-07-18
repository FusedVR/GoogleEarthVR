using UnityEngine;
using UnityEditor;
using Wrld.MapCamera;
using Wrld;

[CustomEditor(typeof(CameraJumpBehaviour))]
public class CameraJumpEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraJumpBehaviour myScript = (CameraJumpBehaviour)target;

        var api = EditorApplication.isPlaying ? Api.Instance : null;
        bool usingBuiltInCameraControls = api != null && api.CameraApi.HasControlledCamera;

        if (usingBuiltInCameraControls)
        {
            if (GUILayout.Button("Set Camera Position"))
            {
                myScript.SetCameraPosition();
            }
        }
        else
        {
            GUILayout.Label("Camera must use built-in controls to enable jump");
        }
    }
}
