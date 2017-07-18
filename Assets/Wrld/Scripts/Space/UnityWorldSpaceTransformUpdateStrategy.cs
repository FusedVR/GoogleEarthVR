using Wrld.Common.Maths;
using UnityEngine;
using Wrld.Space;

namespace Wrld.Space
{
    public class UnityWorldSpaceTransformUpdateStrategy : ITransformUpdateStrategy
    {
        private UnityWorldSpaceCoordinateFrame m_frame;

        public UnityWorldSpaceTransformUpdateStrategy(UnityWorldSpaceCoordinateFrame frame)
        {
            m_frame = frame;
        }

        public void UpdateTransform(Transform objectTransform, DoubleVector3 objectOriginECEF, float heightOffset)
        {
            objectTransform.localPosition = m_frame.ECEFToLocalSpace(objectOriginECEF) + Vector3.up * heightOffset;
            objectTransform.localRotation = m_frame.ECEFToLocalRotation;
        }
    }
}

