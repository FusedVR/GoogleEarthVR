// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using UnityEngine;
using System;

namespace Wrld.MapInput.Touch
{
    public class RotateGesture
    {
        private IUnityInputHandler m_handler;
        private bool rotating;
        private Vector2[] lastPointer;
        private float lastRotationRadians;
        private float m_totalRotation;
        private float m_previousRotationDelta;

        private bool needNewBaseline;

        private Vector2 m_baselineDirection;

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

        private bool UpdateRotation(TouchInputEvent touchEvent, int numTouches, bool pointerUp)
        {
            if (!rotating)
            {
                m_totalRotation = 0.0f;
            }

            if (numTouches == 2 && pointerUp)
            {
                //Eegeo_TTY("UpdateRotation - rotation ended as numTouches %d\n", numTouches);
                return false;
            }

            if (numTouches > 1)
            {
                float x1 = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[0].x));
                float y1 = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[0].y));
                float x2 = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[1].x));
                float y2 = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[1].y));

                Vector2 p1 = new Vector2(Mathf.Floor(x1), Mathf.Floor(y1));
                Vector2 p2 = new Vector2(Mathf.Floor(x2), Mathf.Floor(y2));

                if (!rotating || needNewBaseline)
                {
                    m_baselineDirection = (p2 - p1).normalized;
                    lastRotationRadians = 0.0f;
                    m_previousRotationDelta = 0.0f;
                    needNewBaseline = false;
                }

                float currentRotation = AngleBetween(m_baselineDirection, (p2 - p1));
                float rotationDelta = currentRotation - lastRotationRadians;

                float deltaThresholdDifferentDirection = Mathf.Deg2Rad * 10.0f;

                if (m_previousRotationDelta * rotationDelta < 0.0f)
                {
                    // if rotation changed direction, throw away rotation deltas until we have surpassed a threshold
                    if (Math.Abs(rotationDelta) > deltaThresholdDifferentDirection)
                    {
                        m_previousRotationDelta = rotationDelta;
                        lastRotationRadians = currentRotation;
                    }
                    rotationDelta = 0.0f;
                }
                else
                {
                    m_previousRotationDelta = rotationDelta;
                    lastRotationRadians = currentRotation;
                }

                m_totalRotation += rotationDelta;

                lastPointer[0] = p1;
                lastPointer[1] = p2;
                return true;
            }

            return false;
        }


        public RotateGesture(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_handler = handler;
            rotating = false;
            lastRotationRadians = 0.0f;
            m_totalRotation = 0.0f;
            m_previousRotationDelta = 0.0f;
            needNewBaseline = true;
            m_baselineDirection = Vector2.zero;

            lastPointer = new Vector2[2];
        }

        public void PointerDown(TouchInputEvent touchEvent)
        {
            //Wrld_TTY("Rotate - down - primary %d\n", event.primaryActionIndex);
            if (touchEvent.primaryActionIndex > 1)
            {
                return;
            }

            float x = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[touchEvent.primaryActionIndex].x));
            float y = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[touchEvent.primaryActionIndex].y));
            lastPointer[touchEvent.primaryActionIndex] = new Vector2(x, y);

            if (!rotating)
            {
                int numTouches = touchEvent.pointerEvents.Count;
                bool isRotating = UpdateRotation(touchEvent, numTouches, false);

                if (isRotating)
                {
                    //Wrld_TTY("ROTATE START\n");
                    AppInterface.RotateData rot;
                    rot = PopulateRotateData(numTouches, m_totalRotation);
                    rotating = true;

                    m_handler.Event_TouchRotate_Start(rot);
                }
            }

        }

        public void PointerUp(TouchInputEvent touchEvent)
        {
            if (touchEvent.primaryActionIndex > 1)
            {
                return;
            }

            lastPointer[touchEvent.primaryActionIndex] = Vector2.zero;

            if (rotating)
            {
                int numTouches = touchEvent.pointerEvents.Count;
                bool isRotating = UpdateRotation(touchEvent, numTouches, true);

                if (!isRotating)
                {
                    //Wrld_TTY("ROTATE STOP\n");
                    needNewBaseline = true;
                    AppInterface.RotateData rot = PopulateRotateData(numTouches, 0.0f);
                    rotating = false;
                    m_previousRotationDelta = 0.0f;
                    m_totalRotation = 0.0f;
                    m_handler.Event_TouchRotate_End(rot);
                }
            }
        }

        public void PointerMove(TouchInputEvent touchEvent)
        {
            if (touchEvent.primaryActionIndex > 1)
            {
                return;
            }

            if (rotating)
            {
                int numTouches = touchEvent.pointerEvents.Count;
                bool isRotating = UpdateRotation(touchEvent, numTouches, false);

                if (isRotating)
                {
                    AppInterface.RotateData rot = PopulateRotateData(numTouches, m_totalRotation);
                    m_handler.Event_TouchRotate(rot);
                }
            }
        }
    };
}
