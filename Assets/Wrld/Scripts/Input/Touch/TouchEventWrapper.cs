// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using System.Collections.Generic;

namespace Wrld.MapInput.Touch
{
    public struct TouchInputPointerEvent
    {
        public float x, y;
        public int pointerIdentity;
        public int pointerIndex;
    };

    public struct TouchInputEvent
    {
        public bool isPointerUpEvent;
        public bool isPointerDownEvent;
        public int primaryActionIndex;
        public int primaryActionIdentifier;
        public List<TouchInputPointerEvent> pointerEvents;

        public TouchInputEvent(
                bool _isPointerUpEvent,
                bool _isPointerDownEvent,
                int _primaryActionIndex,
                int _primaryActionIdentifier)
        {
            isPointerUpEvent = _isPointerUpEvent;
            isPointerDownEvent = _isPointerDownEvent;
            primaryActionIndex = _primaryActionIndex;
            primaryActionIdentifier = _primaryActionIdentifier;
            pointerEvents = new List<TouchInputPointerEvent>();
        }
    };
}
