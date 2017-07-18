// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
namespace Wrld.MapInput.Touch
{
    public class TouchGesture
    {
        private IUnityInputHandler m_handler;

        private AppInterface.TouchData CreateTouchEventData(TouchInputEvent inputEvent)
        {
            var result = new AppInterface.TouchData();
            result.point.Set(inputEvent.pointerEvents[0].x, inputEvent.pointerEvents[0].y);
            return result;
        }

        public TouchGesture(IUnityInputHandler handler)
        {
            m_handler = handler;
        }

        public void PointerDown(TouchInputEvent inputEvent)
        {
            m_handler.Event_TouchDown(CreateTouchEventData(inputEvent));
        }

        public void PointerUp(TouchInputEvent inputEvent)
        {
            m_handler.Event_TouchUp(CreateTouchEventData(inputEvent));
        }

        public void PointerMove(TouchInputEvent inputEvent)
        {
            m_handler.Event_TouchMove(CreateTouchEventData(inputEvent));
        }
    };
}
