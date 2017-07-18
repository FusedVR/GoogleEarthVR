// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using System;
using UnityEngine;

namespace Wrld.MapInput.Mouse
{
    public class MousePanGesture
    {
        private IUnityInputHandler m_handler;
        bool m_panButtonDown;
        bool m_panning;
        private Vector2 m_current;
        private Vector2 m_anchor;
        private float majorScreenDimension;

        private AppInterface.PanData PopulatePanData()
        {
            var result = new AppInterface.PanData();
            result.pointAbsolute.Set((float)Math.Round(m_current.x), (float)Math.Round(m_current.y));
            result.pointRelative.Set((float)Math.Round(result.pointAbsolute.x - m_anchor.x), (float)Math.Round(m_anchor.y - result.pointAbsolute.y)); //INVERT Y
            result.pointRelativeNormalized = result.pointRelative / majorScreenDimension;
            result.numTouches = m_panButtonDown ? 1 : 0;
            result.touchExtents = Vector2.zero;
            result.majorScreenDimension = majorScreenDimension;
            result.velocity = Vector2.zero; // not implemented

            return result;
        }

        public MousePanGesture(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_handler = handler;
            m_panning = false;
            majorScreenDimension = Math.Max(screenWidth, screenHeight);

            m_current = Vector2.zero;
            m_anchor = Vector2.zero;
        }

        private const int MaxPanInputs = 10;
        private const float InputActivityTolerance = 4.0f;
        private const float MinPanMagDeltaSquaredTolerance = 12.0f;

        public void PointerDown(MouseInputEvent mouseEvent)
        {
            if (mouseEvent.Action != MouseInputAction.MousePrimaryDown)
                return;

            m_panButtonDown = true;

            if (!m_panning)

            {
                m_current.Set(mouseEvent.x, mouseEvent.y);
                m_anchor = m_current;
            }
        }

        public void PointerUp(MouseInputEvent mouseEvent)
        {
            if (mouseEvent.Action != MouseInputAction.MousePrimaryUp)
                return;

            m_panButtonDown = false;

            if (m_panning)
            {
                m_current.x = mouseEvent.x;
                m_current.y = mouseEvent.y;

                m_panning = false;
                var pan = PopulatePanData();
                m_handler.Event_TouchPan_End(pan);
            }
        }

        public void PointerMove(MouseInputEvent mouseEvent)
        {
            if (!m_panButtonDown)
                return;

            Vector2 newPos = new Vector2(mouseEvent.x, mouseEvent.y);
            if (m_current == newPos)
            {
                return;
            }


            float anchorDeltaSqMag = (m_anchor - newPos).sqrMagnitude;
            bool exceededStartPanThreshold = (anchorDeltaSqMag > 4.0f); //m_panStartThresholdPixels);

            if (!m_panning && exceededStartPanThreshold)
            {
                m_panning = true;
                var pan = PopulatePanData();
                m_handler.Event_TouchPan_Start(pan);
            }

            if (m_panning)
            {
                m_current = newPos;
                var pan = PopulatePanData();

                m_handler.Event_TouchPan(pan);
            }
        }
    };
}
