// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using UnityEngine;

namespace Wrld.MapInput.Mouse
{
    class MouseRotateGesture
    {
        private IUnityInputHandler m_handler;
        bool m_rotating;

        Vector2 m_anchor;
        float m_totalRotation;

        private static AppInterface.RotateData PopulateRotateData(int numTouches, float rotationRadians)
        {
            AppInterface.RotateData result = new AppInterface.RotateData();
            result.numTouches = numTouches;
            result.rotation = rotationRadians;
            result.velocity = 0.0f; // not implemented

            return result;
        }

        private static float AngleBetween(Vector2 a, Vector2 b)
        {
            Vector2 dirA = a.normalized;
            Vector2 dirB = b.normalized;

            float dot = Vector2.Dot(dirA, dirB);
            float angle = Mathf.Acos(Mathf.Clamp(dot, -1.0f, 1.0f));
            float sign = (dirA.x * dirB.y) - (dirA.y * dirB.x);
            angle *= (sign < 0.0f) ? -1.0f : 1.0f;

            return angle;
        }

        private bool UpdateRotation(MouseInputEvent touchEvent, int numTouches, bool pointerUp)
        {
            return false;
        }


        public MouseRotateGesture(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_handler = handler;
            m_totalRotation = 0.0f;
            m_rotating = false;
            m_anchor = Vector2.zero;

        }

        AppInterface.RotateData MakeRotateData(float rotation, float velocity, int numTouches)
        {
            AppInterface.RotateData rotateData;
            rotateData.rotation = rotation;
            rotateData.velocity = velocity;
            rotateData.numTouches = numTouches;
            return rotateData;
        }

        public void PointerDown(MouseInputEvent mouseEvent)
        {
            m_rotating = true;
            m_anchor.x = mouseEvent.x;
            m_anchor.y = mouseEvent.y;

            m_totalRotation = 0.0f;


            var data = MakeRotateData(0.0f, 0.0f, 2);
            m_handler.Event_TouchRotate_Start(data);
        }

        public void PointerUp(MouseInputEvent mouseEvent)
        {
            m_rotating = false;

            var data = MakeRotateData(0.0f, 0.0f, 0);

            m_handler.Event_TouchRotate_End(data);
        }

        public void PointerMove(MouseInputEvent mouseEvent)
        {
            if (!m_rotating)
            {
                return;
            }

            const float INPUT_SCALE = 1 / 500.0f;
            var data = MakeRotateData(INPUT_SCALE * (m_anchor.x - mouseEvent.x), 0.0f, 2);

            m_totalRotation += data.rotation;

            m_handler.Event_TouchRotate(data);
        }
    };
}
