// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Wrld.MapInput.Touch
{
    public class PanGesture
    {
        private IUnityInputHandler m_handler;
        private bool panning;
        private List<Vector2> inputs;
        private Vector2 panCenter;
        private Vector2 panAnchor;
        private float majorScreenDimension;

        private Vector2 GetTouchExtents(TouchInputEvent touchEvent)
        {
            Vector2 touchExtents = Vector2.zero;

            int numTouches = touchEvent.pointerEvents.Count;
            int inputsToConsider = Math.Min(MaxPanInputs, numTouches);

            if (inputsToConsider > 1)
            {
                Vector2 boundsMin = inputs[0];
                Vector2 boundsMax = boundsMin;

                for (int i = 1; i < inputsToConsider; ++i)
                {
                    boundsMin = Vector2.Min(boundsMin, inputs[i]);
                    boundsMax = Vector2.Max(boundsMax, inputs[i]);
                }

                touchExtents = boundsMax - boundsMin;
            }

            return touchExtents;
        }

        private AppInterface.PanData PopulatePanData(int numTouches, Vector2 touchExtents)
        {
            var result = new AppInterface.PanData();
            result.pointAbsolute.Set((float)Math.Round(panCenter.x), (float)Math.Round(panCenter.y));
            result.pointRelative.Set((float)Math.Round(result.pointAbsolute.x - panAnchor.x), (float)Math.Round(result.pointAbsolute.y - panAnchor.y));
            result.pointRelativeNormalized = result.pointRelative / majorScreenDimension;
            result.numTouches = numTouches;
            result.touchExtents = touchExtents;
            result.majorScreenDimension = majorScreenDimension;
            result.velocity = Vector2.zero; // not implemented

            return result;
        }

        public PanGesture(IUnityInputHandler handler, float screenWidth, float screenHeight)
        {
            m_handler = handler;
            panning = false;
            majorScreenDimension = Math.Max(screenWidth, screenHeight);

            inputs = new List<Vector2>();

            for (int i = 0; i < MaxPanInputs; ++i)
            {
                inputs.Add(Vector2.zero);
            }

            panCenter = Vector2.zero;
            panAnchor = Vector2.zero;
        }

        private const int MaxPanInputs = 10;
        private const float InputActivityTolerance = 4.0f;
        private const float MinPanMagDeltaSquaredTolerance = 12.0f;

        public void PointerDown(TouchInputEvent touchEvent)
        {
            int numTouches = touchEvent.pointerEvents.Count;
            float x = 0;
            float y = 0;
            int inputsToConsider = Math.Min(MaxPanInputs, numTouches);

            for (int i = 0; i < inputsToConsider; ++i)
            {
                int pid = touchEvent.pointerEvents[i].pointerIdentity;
                float newx = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[i].x));
                float newy = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[i].y));
                x += newx;
                y += newy;
                inputs[pid] = new Vector2(newx, newy);
            }

            if (!panning)
            {
                panAnchor.Set(x / inputsToConsider, y / inputsToConsider);
            }
        }

        public void PointerUp(TouchInputEvent touchEvent)
        {
            int numTouches = touchEvent.pointerEvents.Count;

            if (panning && (numTouches == 1))
            {
                AppInterface.PanData pan;
                pan = PopulatePanData(0, Vector2.zero);
                m_handler.Event_TouchPan_End(pan);

                panning = false;
                panCenter = Vector2.zero;
                panAnchor = Vector2.zero;
            }

            int pointerId = touchEvent.primaryActionIdentifier;

            if (pointerId < MaxPanInputs)
            {
                inputs[pointerId] = Vector2.zero;
            }
        }

        public void PointerMove(TouchInputEvent touchEvent)
        {
            int numTouches = touchEvent.pointerEvents.Count;
            float dx = 0;
            float dy = 0;
            float totalX = 0;
            float totalY = 0;
            int inputsToConsider = Math.Min(MaxPanInputs, numTouches);
            bool somethingPanned = false;

            for (int i = 0; i < inputsToConsider; ++i)
            {
                int pid = touchEvent.pointerEvents[i].pointerIdentity;

                //Wrld_TTY("PanGesture::PointerMove %d -- %f\n", pid, inputs[pid].LengthSq());
                if (inputs[pid].sqrMagnitude > InputActivityTolerance)
                {
                    //Wrld_TTY("POINTER %d CONSIDERED\n", pid);

                    float x = (m_handler.TranslateGlobalXToLocalX(touchEvent.pointerEvents[i].x));
                    float y = (m_handler.TranslateGlobalYToLocalY(touchEvent.pointerEvents[i].y));
                    totalX += x;
                    totalY += y;
                    dx += x - inputs[pid].x;
                    dy += y - inputs[pid].y;
                    inputs[pid] = new Vector2(x, y);

                    //var delta = new Vector2(dx, dy);
                    //if(delta.LengthSq() > MIN_PAN_MAG_DELTA_SQUARED_TOLERANCE) {
                    somethingPanned = true;
                    //}
                }
            }

            if (inputsToConsider > 0)
            {
                if (!panning && somethingPanned)
                {
                    totalX /= inputsToConsider;
                    totalY /= inputsToConsider;
                    panning = true;
                    panCenter.Set(totalX, totalY);

                    AppInterface.PanData pan;
                    Vector2 touchExtents = GetTouchExtents(touchEvent);
                    pan = PopulatePanData(inputsToConsider, touchExtents);

                    //Wrld_TTY("PAN START\n");
                    m_handler.Event_TouchPan_Start(pan);
                }

                if (panning)
                {
                    dx /= inputsToConsider;
                    dy /= inputsToConsider;
                    float newX = panCenter.x + dx;
                    float newY = panCenter.y + dy;

                    panCenter.Set(newX, newY);

                    AppInterface.PanData pan;
                    Vector2 touchExtents = GetTouchExtents(touchEvent);
                    pan = PopulatePanData(inputsToConsider, touchExtents);

                    m_handler.Event_TouchPan(pan);
                }
            }
        }
    };
}
