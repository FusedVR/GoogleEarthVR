// Copyright eeGeo Ltd (2012-2014), All Rights Reserved

namespace Wrld.MapInput
{
    public interface IUnityInputHandler
    {
        void Event_TouchRotate(AppInterface.RotateData data);
        void Event_TouchRotate_Start(AppInterface.RotateData data);
        void Event_TouchRotate_End(AppInterface.RotateData data);

        void Event_TouchPinch(AppInterface.PinchData data);
        void Event_TouchPinch_Start(AppInterface.PinchData data);
        void Event_TouchPinch_End(AppInterface.PinchData data);

        void Event_TouchPan(AppInterface.PanData data);
        void Event_TouchPan_Start(AppInterface.PanData data);
        void Event_TouchPan_End(AppInterface.PanData data);

        void Event_TouchTap(AppInterface.TapData data);
        void Event_TouchDoubleTap(AppInterface.TapData data);

        void Event_TouchDown(AppInterface.TouchData data);
        void Event_TouchMove(AppInterface.TouchData data);
        void Event_TouchUp(AppInterface.TouchData data);

        void Event_Zoom(AppInterface.ZoomData data);

        void Event_TiltStart(AppInterface.TiltData data);
        void Event_Tilt(AppInterface.TiltData data);
        void Event_TiltEnd(AppInterface.TiltData data);

        float TranslateGlobalXToLocalX(float x);
        float TranslateGlobalYToLocalY(float y);
    };
}
