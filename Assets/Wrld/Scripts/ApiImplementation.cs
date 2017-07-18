using UnityEngine;
using Wrld.Common.Maths;
using Wrld.Space;
using Wrld.Common.Camera;
using Wrld.MapCamera;

namespace Wrld
{
    // :TODO: Feels like it might be more natural/usual to split this into ECEF & UnityWorld classes & have both implement same interface.
    public class ApiImplementation
    {
        private NativePluginRunner m_nativePluginRunner;
        private CoordinateSystem m_coordinateSystem;
        private CameraApi m_cameraController;
        private UnityWorldSpaceCoordinateFrame m_frame;
        private DoubleVector3 m_originECEF;
        private InterestPointProvider m_interestPointProvider = new InterestPointProvider();

        public ApiImplementation(string apiKey, CoordinateSystem coordinateSystem, Transform parentTransformForStreamedObjects, ConfigParams configParams)
        {
            m_nativePluginRunner = new NativePluginRunner(apiKey, parentTransformForStreamedObjects, configParams);
            m_coordinateSystem = coordinateSystem;
            var defaultStartingLocation = LatLongAltitude.FromDegrees(
                configParams.LatitudeDegrees, 
                configParams.LongitudeDegrees, 
                coordinateSystem == CoordinateSystem.ECEF ? configParams.DistanceToInterest : 0.0);

            if (coordinateSystem == CoordinateSystem.ECEF)
            {
                m_originECEF = defaultStartingLocation.ToECEF();
            }
            else
            {
                m_frame = new UnityWorldSpaceCoordinateFrame(defaultStartingLocation);
            }

            m_cameraController = new CameraApi(this);
        }

        public void SetOriginPoint(LatLongAltitude lla)
        {
            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                m_cameraController.MoveTo(lla.GetLatLong());
            }
            else
            {
                m_frame.SetCentralPoint(lla);
            }
        }

        internal void ApplyNativeCameraState(NativeCameraState nativeCameraState, UnityEngine.Camera controlledCamera)
        {
            controlledCamera.fieldOfView = nativeCameraState.fieldOfViewDegrees;
            controlledCamera.nearClipPlane = nativeCameraState.nearClipPlaneDistance;
            controlledCamera.farClipPlane = nativeCameraState.farClipPlaneDistance;

            var interestBasis = new EcefTangentBasis(
                    nativeCameraState.interestPointECEF,
                    nativeCameraState.interestBasisRightECEF,
                    nativeCameraState.interestBasisUpECEF,
                    nativeCameraState.interestBasisForwardECEF);

            Vector3 forward, up;
            DoubleVector3 positionECEF;
            CameraHelpers.CalculateLookAt(
                interestBasis.PointEcef,
                interestBasis.Forward,
                nativeCameraState.tiltDegrees * Mathf.Deg2Rad,
                nativeCameraState.distanceToInterestPoint,
                out positionECEF, out forward, out up);

            m_interestPointProvider.UpdateFromNative(nativeCameraState.interestPointECEF);

            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                var position = (positionECEF - m_originECEF).ToSingleVector();
                controlledCamera.transform.position = position;
                controlledCamera.transform.LookAt((interestBasis.PointEcef - m_originECEF).ToSingleVector(), up);
            }
            else // if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                controlledCamera.transform.position = m_frame.ECEFToLocalSpace(positionECEF);
                controlledCamera.transform.LookAt(m_frame.ECEFToLocalSpace(interestBasis.PointEcef), m_frame.ECEFToLocalRotation * up);
            }
        }

        public void StreamResourcesForCamera(UnityEngine.Camera streamingCamera)
        {
            // This code would be a lot cleaner if there was a decent way of copying camera properties around.
            // Maybe we should just make StreamResourcesForCamera ingest something more abstract
            //UnityEngine.Camera zeroBasedCameraECEF = new UnityEngine.Camera();
            //zeroBasedCameraECEF.CopyFrom(streamingCamera);
            var savedPosition = streamingCamera.transform.position;

            if (m_coordinateSystem == CoordinateSystem.ECEF)
            {
                DoubleVector3 finalOriginECEF = m_originECEF + savedPosition;
                streamingCamera.transform.position = Vector3.zero;
                DoubleVector3 interestPointECEF = m_interestPointProvider.CalculateInterestPoint(streamingCamera, finalOriginECEF);
                m_nativePluginRunner.StreamResourcesForCamera(streamingCamera, finalOriginECEF, interestPointECEF);
                m_originECEF = finalOriginECEF; // :TODO: somehow update any other scene-relative cameras - OnRecentreScene event?
                UpdateTransforms();
            }
            else // if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                var savedRotation = streamingCamera.transform.rotation;
                DoubleVector3 finalOriginECEF = m_frame.LocalSpaceToECEF(savedPosition);
                streamingCamera.transform.rotation = m_frame.LocalToECEFRotation * savedRotation;
                streamingCamera.transform.position = Vector3.zero;
                DoubleVector3 interestPointECEF = m_interestPointProvider.CalculateInterestPoint(streamingCamera, finalOriginECEF);
                m_nativePluginRunner.StreamResourcesForCamera(streamingCamera, finalOriginECEF, interestPointECEF);
                streamingCamera.transform.position = savedPosition;
                streamingCamera.transform.rotation = savedRotation;
            }
        }

        private void UpdateTransforms()
        {
            ITransformUpdateStrategy transformUpdateStrategy;

            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                transformUpdateStrategy = new UnityWorldSpaceTransformUpdateStrategy(m_frame);
            }
            else
            {
                var cameraPosition = m_originECEF;// + cam.transform.localPosition;
                transformUpdateStrategy = new ECEFTransformUpdateStrategy(
                    cameraPosition,
                    cameraPosition.normalized.ToSingleVector());
            }

            m_nativePluginRunner.UpdateTransforms(transformUpdateStrategy);
        }

        public CameraApi CameraApi
        {
            get
            {
                return m_cameraController;
            }
        }

        public void Update()
        {
            m_nativePluginRunner.Update();
            m_cameraController.Update();

            UpdateTransforms();
        }

        public void Destroy()
        {
            m_nativePluginRunner.OnDestroy();
        }

        internal Vector3 GeographicToViewportPoint(LatLongAltitude position, Camera camera)
        {
            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                var point = m_frame.ECEFToLocalSpace(position.ToECEF());
                return camera.WorldToViewportPoint(point);
            }
            else
            {
                var point = (position.ToECEF() - m_originECEF).ToSingleVector();
                return camera.WorldToViewportPoint(point);
            }
        }

        internal LatLongAltitude ViewportToGeographicPoint(Vector3 viewportSpacePosition, UnityEngine.Camera camera)
        {
            var unityWorldSpacePosition = camera.ViewportToWorldPoint(viewportSpacePosition);

            if (m_coordinateSystem == CoordinateSystem.UnityWorld)
            {
                return m_frame.LocalSpaceToLatLongAltitude(unityWorldSpacePosition);
            }
            else
            {
                var finalPositionECEF = m_originECEF + unityWorldSpacePosition;

                return LatLongAltitude.FromECEF(finalPositionECEF);
            }
        }

        public void OnApplicationPaused()
        {
            m_nativePluginRunner.OnApplicationPaused();
        }

        public void OnApplicationResumed()
        {
            m_nativePluginRunner.OnApplicationResumed();
        }
    }
}