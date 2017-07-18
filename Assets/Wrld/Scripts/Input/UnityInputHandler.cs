// Copyright eeGeo Ltd (2012-2014), All Rights Reserved
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld.MapInput
{
    public class UnityInputHandler : IUnityInputHandler
    {
        const string DLL = NativePluginRunner.DLL;

        [DllImport(DLL)]
        private static extern void Event_TouchRotate(IntPtr API, ref AppInterface.RotateData data);

        [DllImport(DLL)]
        private static extern void Event_TouchRotate_Start(IntPtr API, ref AppInterface.RotateData data);

        [DllImport(DLL)]
        private static extern void Event_TouchRotate_End(IntPtr API, ref AppInterface.RotateData data);

        [DllImport(DLL)]
        private static extern void Event_TouchPinch(IntPtr API, ref AppInterface.PinchData data);

        [DllImport(DLL)]
        private static extern void Event_TouchPinch_Start(IntPtr API, ref AppInterface.PinchData data);

        [DllImport(DLL)]
        private static extern void Event_TouchPinch_End(IntPtr API, ref AppInterface.PinchData data);

        [DllImport(DLL)]
        private static extern void Event_TouchPan(IntPtr API, ref AppInterface.PanData data);

        [DllImport(DLL)]
        private static extern void Event_TouchPan_Start(IntPtr API, ref AppInterface.PanData data);

        [DllImport(DLL)]
        private static extern void Event_TouchPan_End(IntPtr API, ref AppInterface.PanData data);

        [DllImport(DLL)]
        private static extern void Event_TouchTap(IntPtr API, ref AppInterface.TapData data);

        [DllImport(DLL)]
        private static extern void Event_TouchDoubleTap(IntPtr API, ref AppInterface.TapData data);

        [DllImport(DLL)]
        private static extern void Event_TouchDown(IntPtr API, ref AppInterface.TouchData data);

        [DllImport(DLL)]
        private static extern void Event_TouchMove(IntPtr API, ref AppInterface.TouchData data);

        [DllImport(DLL)]
        private static extern void Event_TouchUp(IntPtr API, ref AppInterface.TouchData data);

        [DllImport(DLL)]
        private static extern void Event_Zoom(IntPtr API, ref AppInterface.ZoomData data);

        [DllImport(DLL)]
        private static extern void Event_TiltStart(IntPtr API, ref AppInterface.TiltData data);

        [DllImport(DLL)]
        private static extern void Event_Tilt(IntPtr API, ref AppInterface.TiltData data);

        [DllImport(DLL)]
        private static extern void Event_TiltEnd(IntPtr API, ref AppInterface.TiltData data);

        private IntPtr m_apiPtr;

        public UnityInputHandler(IntPtr Api)
        {
            m_apiPtr = Api;
        }

        public void Event_TouchRotate(AppInterface.RotateData data)
        {
            Event_TouchRotate(m_apiPtr, ref data);
        }

        public void Event_TouchRotate_Start(AppInterface.RotateData data)
        {
            Event_TouchRotate_Start(m_apiPtr, ref data);
        }

        public void Event_TouchRotate_End(AppInterface.RotateData data)
        {
            Event_TouchRotate_End(m_apiPtr, ref data);
        }

        public void Event_TouchPinch(AppInterface.PinchData data)
        {
            Event_TouchPinch(m_apiPtr, ref data);
        }

        public void Event_TouchPinch_Start(AppInterface.PinchData data)
        {
            Event_TouchPinch_Start(m_apiPtr, ref data);
        }

        public void Event_TouchPinch_End(AppInterface.PinchData data)
        {
            Event_TouchPinch_End(m_apiPtr, ref data);
        }

        public void Event_TouchPan(AppInterface.PanData data)
        {
            Event_TouchPan(m_apiPtr, ref data);
        }

        public void Event_TouchPan_Start(AppInterface.PanData data)
        {
            Event_TouchPan_Start(m_apiPtr, ref data);
        }

        public void Event_TouchPan_End(AppInterface.PanData data)
        {
            Event_TouchPan_End(m_apiPtr, ref data);
        }

        public void Event_TouchTap(AppInterface.TapData data)
        {
            Event_TouchTap(m_apiPtr, ref data);
        }

        public void Event_TouchDoubleTap(AppInterface.TapData data)
        {
            Event_TouchDoubleTap(m_apiPtr, ref data);
        }

        public void Event_TouchDown(AppInterface.TouchData data)
        {
            Event_TouchDown(m_apiPtr, ref data);
        }

        public void Event_TouchMove(AppInterface.TouchData data)
        {
            Event_TouchMove(m_apiPtr, ref data);
        }

        public void Event_TouchUp(AppInterface.TouchData data)
        {
            Event_TouchUp(m_apiPtr, ref data);
        }

        public void Event_Zoom(AppInterface.ZoomData data)
        {
            Event_Zoom(m_apiPtr, ref data);
        }

        public void Event_TiltStart(AppInterface.TiltData data)
        {
            Event_TiltStart(m_apiPtr, ref data);
        }


        public void Event_Tilt(AppInterface.TiltData data)
        {
            Event_Tilt(m_apiPtr, ref data);
        }

        public void Event_TiltEnd(AppInterface.TiltData data)
        {
            Event_TiltEnd(m_apiPtr, ref data);
        }

        public float TranslateGlobalXToLocalX(float x)
        {
            return x;
        }

        public float TranslateGlobalYToLocalY(float y)
        {
            float result = Screen.height - y;
            return result;
        }
    };
}
