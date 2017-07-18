using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.Common.Camera
{
    public class CameraHelpers
    {
        public static void CalculateLookAt(
            DoubleVector3 interestPointEcef,
            Vector3 interestBasisFoward,
            float pitchRadians,
            float distanceCameraToInterest,
            out DoubleVector3 cameraLocation,
            out Vector3 cameraDirection,
            out Vector3 cameraUp)
        {
            var interestBasis = new EcefTangentBasis(interestPointEcef, interestBasisFoward);
            
            var q = Quaternion.AngleAxis(pitchRadians * Mathf.Rad2Deg, interestBasis.Right);
            cameraDirection = q.RotatePoint(interestBasis.Forward);

            var toCamera = -cameraDirection * distanceCameraToInterest;

            cameraLocation = interestBasis.PointEcef + new DoubleVector3(toCamera.x, toCamera.y, toCamera.z);
            cameraUp = Vector3.Cross(cameraDirection, interestBasis.Right);
        }
    }
}