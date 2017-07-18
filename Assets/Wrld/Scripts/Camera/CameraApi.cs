using AOT;
using Wrld.Space;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Wrld.Helpers;

namespace Wrld.MapCamera
{
    public class CameraApi
    {
        private const string m_nonNullCameraMessage = "A non-null camera must be supplied, or have been set via SetControlledCamera.";

        public delegate void TransitionStartHandler(CameraApi cameraApi, UnityEngine.Camera camera);
        public delegate void TransitionEndHandler(CameraApi cameraApi, UnityEngine.Camera camera);

        public event TransitionStartHandler OnTransitionStart;
        public event TransitionEndHandler OnTransitionEnd;
        public bool IsTransitioning { get; private set; }

        public CameraApi(ApiImplementation apiImplementation)
        {
            m_apiImplementation = apiImplementation;
            OnTransitionStart += TransitionStarted;
            OnTransitionEnd += TransitionEnded;
            m_inputHandler = new CameraInputHandler();
        }

        public void SetControlledCamera(UnityEngine.Camera camera)
        {
            m_controlledCamera = camera;
        }

        public void ClearControlledCamera()
        {
            m_controlledCamera = null;
        }

        public Vector3 GeographicToViewportPoint(LatLongAltitude position, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return m_apiImplementation.GeographicToViewportPoint(position, camera);
        }

        public LatLongAltitude ViewportToGeographicPoint(Vector3 viewportSpacePosition, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return m_apiImplementation.ViewportToGeographicPoint(viewportSpacePosition, camera);
        }

        public LatLongAltitude ScreenToGeographicPoint(Vector3 screenSpacePosition, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return ViewportToGeographicPoint(camera.ScreenToViewportPoint(screenSpacePosition), camera);
        }

        public Vector3 GeographicToScreenPoint(LatLongAltitude position, Camera camera = null)
        {
            camera = camera ?? m_controlledCamera;

            if (camera == null)
            {
                throw new ArgumentNullException("camera", m_nonNullCameraMessage);
            }

            return camera.ViewportToScreenPoint(GeographicToViewportPoint(position, camera));
        }

        public void Update()
        {
            if (m_controlledCamera != null)
            {
                m_inputHandler.Update();
                var cameraState = new NativeCameraState();
                GetCurrentCameraState(NativePluginRunner.API, ref cameraState);
                m_apiImplementation.ApplyNativeCameraState(cameraState, m_controlledCamera);
            }
        }

        public bool MoveTo(
            LatLong interestPoint,
            double? distanceFromInterest = null,
            double? headingDegrees = null,
            double? pitchDegrees = null)
        {
            if (m_controlledCamera == null)
            {
                throw new ArgumentNullException("Camera", m_nonNullCameraMessage);
            }

            const bool animated = false;
            const bool modifyPosition = true;
            const double transitionDuration = 0.0;
            const bool hasTransitionDuration = false;
            const bool jumpIfFarAway = true;
            const bool allowInterruption = true;
            const double interestAltitude = 0.0;

            return SetView(
                NativePluginRunner.API,
                animated,
                interestPoint.GetLatitude(), interestPoint.GetLongitude(), interestAltitude, modifyPosition,
                distanceFromInterest.HasValue ? distanceFromInterest.Value : 0.0, distanceFromInterest.HasValue,
                headingDegrees.HasValue ? headingDegrees.Value : 0.0, headingDegrees.HasValue,
                pitchDegrees.HasValue ? pitchDegrees.Value : 0.0, pitchDegrees.HasValue,
                transitionDuration, hasTransitionDuration,
                jumpIfFarAway, allowInterruption
                );
        }

        public bool MoveTo(
            LatLong interestPoint,
            LatLongAltitude cameraPosition)
        {
            if (m_controlledCamera == null)
            {
                throw new ArgumentNullException("Camera", m_nonNullCameraMessage);
            }

            double distance;
            double headingDegrees;
            double pitchDegrees;
            GetPitchHeadingAndDistanceFromCameraAndTargetPosition(interestPoint, cameraPosition, out pitchDegrees, out headingDegrees, out distance);

            return MoveTo(interestPoint, distance, headingDegrees, pitchDegrees);
        }

        public bool AnimateTo(
            LatLong interestPoint,
            double? distanceFromInterest = null,
            double? headingDegrees = null,
            double? pitchDegrees = null,
            double? transitionDuration = null,
            bool jumpIfFarAway = true)
        {
            if (m_controlledCamera == null)
            {
                throw new ArgumentNullException("Camera", m_nonNullCameraMessage);
            }

            const bool animated = true;
            const bool modifyPosition = true;
            const bool allowInterruption = true;
            const double interestAltitude = 0.0;

            return SetView(
                NativePluginRunner.API, 
                animated,
                interestPoint.GetLatitude(), interestPoint.GetLongitude(), interestAltitude, modifyPosition,
                distanceFromInterest.HasValue ? distanceFromInterest.Value : 0.0, distanceFromInterest.HasValue,
                headingDegrees.HasValue ? headingDegrees.Value : 0.0, headingDegrees.HasValue,
                pitchDegrees.HasValue ? pitchDegrees.Value : 0.0, pitchDegrees.HasValue,
                transitionDuration.HasValue ? transitionDuration.Value : 0.0, transitionDuration.HasValue,
                jumpIfFarAway,
                allowInterruption
                );
        }

        public bool AnimateTo(
            LatLong interestPoint,
            LatLongAltitude cameraPosition,
            double? transitionDuration = null,
            bool jumpIfFarAway = true)
        {
            double distance;
            double headingDegrees;
            double pitchDegrees;
            GetPitchHeadingAndDistanceFromCameraAndTargetPosition(interestPoint, cameraPosition, out pitchDegrees, out headingDegrees, out distance);

            return AnimateTo(interestPoint, distance, headingDegrees, pitchDegrees, transitionDuration, jumpIfFarAway);
        }

        public bool HasControlledCamera {  get { return m_controlledCamera != null; } }

        [DllImport(NativePluginRunner.DLL, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SetView(
            IntPtr ptr,
            [MarshalAs(UnmanagedType.I1)] bool animated,
            double latDegrees, double longDegrees, double altitude, [MarshalAs(UnmanagedType.I1)] bool modifyPosition,
            double distance, [MarshalAs(UnmanagedType.I1)] bool modifyDistance,
            double headingDegrees, [MarshalAs(UnmanagedType.I1)] bool modifyHeading,
            double tiltDegrees, [MarshalAs(UnmanagedType.I1)] bool modifyTilt,
            double transitionDurationSeconds, [MarshalAs(UnmanagedType.I1)] bool hasTransitionDuration,
            [MarshalAs(UnmanagedType.I1)] bool jumpIfFarAway,
            [MarshalAs(UnmanagedType.I1)] bool allowInterruption
            );

        [DllImport(NativePluginRunner.DLL)]
        private static extern void GetCurrentCameraState(IntPtr ptr, ref NativeCameraState cameraState);

        private void TransitionStarted(CameraApi controller, UnityEngine.Camera camera)
        {
            IsTransitioning = true;
        }

        private void TransitionEnded(CameraApi controller, UnityEngine.Camera camera)
        {
            IsTransitioning = false;
        }
        private static void GetPitchHeadingAndDistanceFromCameraAndTargetPosition(LatLong interestPoint, LatLongAltitude cameraPosition, out double pitchDegrees, out double headingDegrees, out double distance)
        {
            double distanceAlongGround = LatLong.EstimateGreatCircleDistance(interestPoint, cameraPosition.GetLatLong());
            double cameraAltitude = cameraPosition.GetAltitude();
            distance = Math.Sqrt(distanceAlongGround * distanceAlongGround + cameraAltitude * cameraAltitude);
            headingDegrees = cameraPosition.BearingTo(interestPoint);
            pitchDegrees = MathsHelpers.Rad2Deg(Math.PI * 0.5 - Math.Atan2(cameraAltitude, distanceAlongGround));
        }

        internal enum CameraEventType
        {
            Move,
            MoveStart,
            MoveEnd,
            Drag,
            DragStart,
            DragEnd,
            Pan,
            PanStart,
            PanEnd,
            Rotate,
            RotateStart,
            RotateEnd,
            Tilt,
            TiltStart,
            TiltEnd,
            Zoom,
            ZoomStart,
            ZoomEnd,
            TransitionStart,
            TransitionEnd
        };

        internal delegate void CameraEventCallback(CameraEventType eventId);

        [MonoPInvokeCallback(typeof(CameraEventCallback))]
        internal static void OnCameraEvent(CameraEventType eventID)
        {
            var controller = Api.Instance.CameraApi;

            if (eventID == CameraEventType.TransitionStart)
            {
                controller.OnTransitionStart.Invoke(controller, controller.m_controlledCamera);
            }
            else if (eventID == CameraEventType.TransitionEnd)
            {
                controller.OnTransitionEnd.Invoke(controller, controller.m_controlledCamera);
            }
            // :TODO: handle other events
        }

        private UnityEngine.Camera m_controlledCamera;
        private ApiImplementation m_apiImplementation;
        private CameraInputHandler m_inputHandler;
    }
}