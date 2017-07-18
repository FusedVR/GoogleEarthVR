// Copyright eeGeo Ltd (2012-2014), All Rights Reserved

using Wrld.MapInput.Mouse;

namespace Wrld.MapInput.Touch
{
    public class UnityTouchInputProcessor : IUnityInputProcessor
    {
        private PanGesture m_pan;
        private PinchGesture m_pinch;
        private RotateGesture m_rotate;
        private TouchGesture m_touch;
        private TapGesture m_tap;

        public UnityTouchInputProcessor(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_pan = new PanGesture(handler, screenWidth, screenHeight);
            m_pinch = new PinchGesture(handler, screenWidth, screenHeight);
            m_rotate = new RotateGesture(handler, screenWidth, screenHeight);
            m_touch = new TouchGesture(handler);
            m_tap = new TapGesture(handler);
        }

        public void HandleInput(MouseInputEvent inputEvent)
        {

        }

        public void HandleInput(TouchInputEvent inputEvent)
        {
            if (inputEvent.isPointerDownEvent)
            {
                m_pan.PointerDown(inputEvent);
                m_pinch.PointerDown(inputEvent);
                m_rotate.PointerDown(inputEvent);
                m_touch.PointerDown(inputEvent);
                m_tap.PointerDown(inputEvent);
            }
            else if (inputEvent.isPointerUpEvent)
            {
                m_pan.PointerUp(inputEvent);
                m_pinch.PointerUp(inputEvent);
                m_rotate.PointerUp(inputEvent);
                m_touch.PointerUp(inputEvent);
                m_tap.PointerUp(inputEvent);
            }
            else
            {
                m_pan.PointerMove(inputEvent);
                m_pinch.PointerMove(inputEvent);
                m_rotate.PointerMove(inputEvent);
                m_touch.PointerMove(inputEvent);
                m_tap.PointerMove(inputEvent);
            }
        }

        public void Update(float deltaSeconds)
        {
            m_tap.Update(deltaSeconds);
        }
    };
}
