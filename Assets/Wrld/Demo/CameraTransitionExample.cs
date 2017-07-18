using Wrld;
using Wrld.Space;
using UnityEngine;

public class CameraTransitionExample : MonoBehaviour
{
    int m_currentTransitionNum = 0;

    void PlayTransitionNum(int num)
    {
        var api = Api.Instance;

        switch (num)
        {
            case 0:
                var transamericaPyramidSanFrancisco = LatLong.FromDegrees(37.7951572, -122.4028915);
                api.CameraApi.AnimateTo(transamericaPyramidSanFrancisco, distanceFromInterest: 1500, headingDegrees: 90, pitchDegrees:45);
                break;

            case 1:
                var ferryBuildingSanfrancisco = LatLong.FromDegrees(37.7955683, -122.3935391);
                api.CameraApi.AnimateTo(ferryBuildingSanfrancisco, headingDegrees: 270, pitchDegrees:0);
                break;

            case 2:
                var rinconParkSanFrancisco = LatLong.FromDegrees(37.791542, -122.3902725);
                var cameraPosition = LatLongAltitude.FromDegrees(37.791542 + 0.005, -122.3902725, 500.0);
                api.CameraApi.AnimateTo(rinconParkSanFrancisco, cameraPosition);
                break;
        }
    }

    void Update ()
    {
        var api = Api.Instance;

        if (!api.CameraApi.IsTransitioning)
        {
            ++m_currentTransitionNum;

            m_currentTransitionNum %= 3;

            PlayTransitionNum(m_currentTransitionNum);
        }
    }
}
