using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.Space
{
    public interface ITransformUpdateStrategy
    {
        void UpdateTransform(Transform objectTransform, DoubleVector3 objectOriginECEF, float heightOffset);
    }
}
