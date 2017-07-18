// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using UnityEngine;

namespace Wrld.MapInput.Mouse
{
    public class MouseTiltGesture
    {
        private IUnityInputHandler m_handler;
        bool m_tilting;
        float m_previousMousePositionY;

        public MouseTiltGesture(IUnityInputHandler handler)
        {
            m_handler = handler;
            m_tilting = false;
            m_previousMousePositionY = 0.0f;
        }

        AppInterface.TiltData MakeTiltData(float distance)
        {
            AppInterface.TiltData tiltData;
            tiltData.distance = distance;
            tiltData.screenHeight = 1.0f;
            tiltData.screenPercentageNormalized = 1.0f;
            return tiltData;
        }

        AppInterface.TiltData MakeTiltData(float distance, float screenHeight)
        {
            const float TILT_SCALE = 0.4f;
            AppInterface.TiltData tiltData;
            tiltData.distance = distance;
            tiltData.screenHeight = screenHeight;
            tiltData.screenPercentageNormalized = TILT_SCALE;
            return tiltData;
        }

        public void PointerDown(MouseInputEvent mouseEvent)
        {
            m_tilting = true;

            m_previousMousePositionY = mouseEvent.y;

            var data = MakeTiltData(0.0f);

            m_handler.Event_TiltStart(data);
        }

        public void PointerUp(MouseInputEvent mouseEvent)
        {
            if (!m_tilting)
                return;

            EndTiltGesture();
        }

        public void PointerMove(MouseInputEvent mouseEvent)
        {
            if (!m_tilting)
                return;

            var data = MakeTiltData(m_previousMousePositionY - mouseEvent.y, Screen.height);
            m_previousMousePositionY = mouseEvent.y;

            m_handler.Event_Tilt(data);
        }

        void EndTiltGesture()
        {
            m_tilting = false;
            m_previousMousePositionY = 0.0f;

            var data = MakeTiltData(0.0f);

            m_handler.Event_TiltEnd(data);
        }
    };
}
