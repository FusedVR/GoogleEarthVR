using Wrld.Common.Camera;
using Wrld.Common.Maths;
using Wrld.MapCamera;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld
{
    public class StreamingUpdater
    {
        [DllImport(NativePluginRunner.DLL)]
        private static extern IntPtr SetUpdateParams(IntPtr ptr, CameraState state, UnityEngine.Plane[] planes, float screenWidth, float screenHeight, float fovRadians);

        public void Update(UnityEngine.Camera zeroBasedCameraECEF, DoubleVector3 cameraOriginECEF, DoubleVector3 interestPointECEF)
        {
            UnityEngine.Plane[] planes;
            CameraState state = CalculateCameraStateAndPlanes(zeroBasedCameraECEF, cameraOriginECEF, interestPointECEF, out planes);

            if (planes != null)
            {
                SetUpdateParams(NativePluginRunner.API, state, planes, Screen.width, Screen.height, Mathf.Deg2Rad * zeroBasedCameraECEF.fieldOfView);
            }
        }

        private CameraState CalculateCameraStateAndPlanes(UnityEngine.Camera zeroBasedCameraECEF, DoubleVector3 cameraOriginECEF, DoubleVector3 interestPointECEF, out UnityEngine.Plane[] planes)
        {
            var state = new CameraState();
            state.InterestPointEcef = interestPointECEF;
            state.LocationEcef = cameraOriginECEF;
            state.ProjectMatrix = Matrix4x4.identity;
            state.ViewMatrix = Matrix4x4.identity;

            var isZeroBased = zeroBasedCameraECEF.transform.position.sqrMagnitude < 0.001f;
            var hasNonzeroSize = zeroBasedCameraECEF.orthographicSize > 0;

            if (!isZeroBased)
            {
                Debug.LogError("Expected a camera with zero translation (position should be represented in cameraOriginECEF).");
            }
            if (!hasNonzeroSize)
            {
                Debug.LogError("Camera Orthographic Size must be greater than 0 for correct frustum calculation");
            }

            if (isZeroBased && hasNonzeroSize)
            {
                planes = GeometryUtility.CalculateFrustumPlanes(zeroBasedCameraECEF);
            }
            else
            {
                planes = null;
            }

            return state;
        }
    }
}

