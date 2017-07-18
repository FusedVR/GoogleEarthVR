using Wrld;
using Wrld.Space;
using UnityEngine;

namespace Wrld.MapCamera
{
    public class CameraJumpBehaviour : MonoBehaviour
    {
        public double InterestPointLatitudeDegrees;
        public double InterestPointLongitudeDegrees;
        public double DistanceFromInterestPoint;

        public void SetCameraPosition()
        {
            var api = Api.Instance;
            
            api.CameraApi.MoveTo(LatLong.FromDegrees(InterestPointLatitudeDegrees, InterestPointLongitudeDegrees), DistanceFromInterestPoint);
        }
    }
}
