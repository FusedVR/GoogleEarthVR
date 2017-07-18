// Copyright eeGeo Ltd (2012-2014), All Rights Reserved

using Wrld.MapInput.Touch;

namespace Wrld.MapInput.Mouse
{
    public class UnityMouseInputProcessor : IUnityInputProcessor
    {
        private MousePanGesture m_pan;
        private MouseZoomGesture m_zoom;
        private MouseRotateGesture m_rotate;
        private MouseTiltGesture m_tilt;
        private MouseTouchGesture m_touch;
        private MouseTapGesture m_tap;

        public UnityMouseInputProcessor(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_pan = new MousePanGesture(handler, screenWidth, screenHeight);
            m_zoom = new MouseZoomGesture(handler);
            m_rotate = new MouseRotateGesture(handler, screenWidth, screenHeight);
            m_tilt = new MouseTiltGesture(handler);
            m_touch = new MouseTouchGesture(handler);
            m_tap = new MouseTapGesture(handler);
        }

        public void HandleInput(TouchInputEvent inputEvent)
        { }

        public void HandleInput(MouseInputEvent inputEvent)
        {
            switch (inputEvent.Action)
            {
                case MouseInputAction.MousePrimaryDown:
                    m_pan.PointerDown(inputEvent);
                    m_touch.PointerDown(inputEvent);
                    m_tap.PointerDown(inputEvent);
                    break;

                case MouseInputAction.MousePrimaryUp:
                    m_pan.PointerUp(inputEvent);
                    m_touch.PointerUp(inputEvent);
                    m_tap.PointerUp(inputEvent);
                    break;

                case MouseInputAction.MouseMove:
                    m_pan.PointerMove(inputEvent);
                    m_rotate.PointerMove(inputEvent);
                    m_tilt.PointerMove(inputEvent);
                    m_touch.PointerMove(inputEvent);
                    break;

                case MouseInputAction.MouseSecondaryDown:
                    m_rotate.PointerDown(inputEvent);
                    break;

                case MouseInputAction.MouseSecondaryUp:
                    m_rotate.PointerUp(inputEvent);
                    break;

                case MouseInputAction.MouseMiddleDown:
                    m_tilt.PointerDown(inputEvent);
                    break;

                case MouseInputAction.MouseMiddleUp:
                    m_tilt.PointerUp(inputEvent);
                    break;

                case MouseInputAction.MouseWheel:
                    m_zoom.PointerMove(inputEvent);
                    break;
            }
        }

        public void Update(float deltaSeconds)
        {
            m_tap.Update(deltaSeconds);
            m_zoom.Update(deltaSeconds);
        }
    };
}
