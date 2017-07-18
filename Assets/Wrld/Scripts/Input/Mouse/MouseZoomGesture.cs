// Copyright Wrld Ltd (2012-2014), All Rights Reserved
using UnityEngine;

namespace Wrld.MapInput.Mouse
{
    public class MouseZoomGesture
    {
        private IUnityInputHandler m_handler;
        float m_sensitivity;
        float m_maxZoomDelta;
        float m_zoomAccumulator;

        private bool UpdatePinching(bool pinching, MouseInputEvent touchEvent, out float pinchScale, int numTouches, bool pointerUp)
        {
            pinchScale = 0.0f;
            return false;
        }


        public MouseZoomGesture(IUnityInputHandler handler)
        {
            m_handler = handler;
            m_sensitivity = 1.0f;
            m_maxZoomDelta = 50.0f;
        }

        public void PointerMove(MouseInputEvent mouseEvent)
        {
            float zoomDelta = -mouseEvent.z * m_sensitivity;
            m_zoomAccumulator += zoomDelta;
        }

        public void Update(float dt)
        {
            if (m_zoomAccumulator == 0.0f)
                return;

            float maxZoomDelta = dt * m_maxZoomDelta;
            float clampedZoomDelta = Mathf.Clamp(m_zoomAccumulator, -maxZoomDelta, maxZoomDelta);

            m_zoomAccumulator -= clampedZoomDelta;

            AppInterface.ZoomData zoomData;
            zoomData.distance = clampedZoomDelta;

            m_handler.Event_Zoom(zoomData);

            float maxBufferTime = 0.5f;
            float maxMagnitude = m_maxZoomDelta * maxBufferTime;
            m_zoomAccumulator = Mathf.Clamp(m_zoomAccumulator, -maxMagnitude, maxMagnitude);
        }
    };
}
