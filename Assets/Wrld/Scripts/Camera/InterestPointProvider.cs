using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.MapCamera
{
    class InterestPointProvider
    {
        private DoubleVector3 m_interestPointECEF;
        private bool m_hasInterestPointFromNativeController;

        public void UpdateFromNative(DoubleVector3 interestPointECEF)
        {
            m_interestPointECEF = interestPointECEF;
            m_hasInterestPointFromNativeController = true;
        }

        public DoubleVector3 CalculateInterestPoint(Camera cameraECEF, DoubleVector3 cameraOriginECEF)
        {
            if (m_hasInterestPointFromNativeController)
            {
                m_hasInterestPointFromNativeController = false;
                return m_interestPointECEF;
            }

            return CalculateEstimatedInterestPoint(cameraECEF, cameraOriginECEF);
        }

        private DoubleVector3 CalculateEstimatedInterestPoint(Camera cameraECEF, DoubleVector3 cameraOriginECEF)
        {
            DoubleVector3 finalCameraPositionECEF = cameraOriginECEF;// + cameraECEF.transform.position;
            DoubleVector3 estimatedInterestPointECEF = finalCameraPositionECEF + cameraECEF.transform.forward * (cameraECEF.nearClipPlane + cameraECEF.farClipPlane) * 0.5f;

            return estimatedInterestPointECEF;
        }
    }
}