using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.Space
{
    public class ECEFTransformUpdateStrategy : ITransformUpdateStrategy
    {
        private DoubleVector3 m_cameraPositionECEF;
        private Vector3 m_up;

        public ECEFTransformUpdateStrategy(DoubleVector3 cameraPositionECEF, Vector3 up)
        {
            m_cameraPositionECEF = cameraPositionECEF;
            m_up = up;
        }

        public void UpdateTransform(Transform objectTransform, DoubleVector3 objectOriginECEF, float heightOffset)
        {
            var cameraRelativePosition = (objectOriginECEF - m_cameraPositionECEF).ToSingleVector();
            objectTransform.localPosition = cameraRelativePosition + m_up * heightOffset;
        }
    }
}

