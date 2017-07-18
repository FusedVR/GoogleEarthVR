// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using System;
using UnityEngine;

namespace Wrld.MapInput.Touch
{
    public class PinchGesture
    {
        private IUnityInputHandler m_handler;
        private bool pinching;
        private float previousDistance;
        private float majorScreenDimension;

        private bool UpdatePinching(bool pinching, TouchInputEvent touchEvent, out float pinchScale, int numTouches, bool pointerUp)
        {
            float distance = 0.0f;
            bool nowPinching = false;
            pinchScale = 0.0f;

            if (numTouches == 2 && pointerUp)
            {
                return false;
            }

            if (numTouches < 2)
            {
                distance = previousDistance;
            }
            else
            {
                float x1 = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[0].x));
                float y1 = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[0].y));
                float x2 = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[1].x));
                float y2 = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[1].y));

                Vector2 p1 = new Vector2(x1, y1);
                Vector2 p2 = new Vector2(x2, y2);
                Vector2 v2Dist = p1 - p2;
                distance = v2Dist.magnitude;
                nowPinching = true;
            }

            float delta = (previousDistance - distance);
            pinchScale = delta;
            previousDistance = distance;
            return pinching || nowPinching;
        }


        public PinchGesture(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_handler = handler;
            pinching = false;
            majorScreenDimension = Math.Max(screenWidth, screenHeight);
        }

        public void PointerDown(TouchInputEvent touchEvent)
        {
            if (!pinching)
            {
                float pinchScale;
                int numTouches = touchEvent.pointerEvents.Count;
                bool isPinching = UpdatePinching(pinching, touchEvent, out pinchScale, numTouches, false);

                if (isPinching)
                {
                    //Wrld_TTY("PINCH START\n");
                    AppInterface.PinchData pinch;
                    pinching = true;
                    pinch.scale = 0.0f;
                    m_handler.Event_TouchPinch_Start(pinch);
                }
            }
        }
        public void PointerUp(TouchInputEvent touchEvent)
        {
            if (pinching)
            {
                float pinchScale;
                int numTouches = touchEvent.pointerEvents.Count;
                bool isPinching = UpdatePinching(pinching, touchEvent, out pinchScale, numTouches, true);
                if (!isPinching)
                {
                    //Wrld_TTY("PINCH STOP\n");
                    AppInterface.PinchData pinch;
                    pinching = false;
                    previousDistance = 0.0f;
                    pinch.scale = pinchScale;
                    m_handler.Event_TouchPinch_End(pinch);
                }
            }
        }
        public void PointerMove(TouchInputEvent touchEvent)
        {
            if (pinching)
            {
                float pinchScale;
                int numTouches = touchEvent.pointerEvents.Count;
                bool isPinching = UpdatePinching(pinching, touchEvent, out pinchScale, numTouches, false);
                if (isPinching)
                {
                    AppInterface.PinchData pinch;
                    pinch.scale = pinchScale / majorScreenDimension;
                    m_handler.Event_TouchPinch(pinch);
                }
            }
        }
    };
}
