using UnityEngine;

namespace Wrld.Common.Maths
{
    public static class DoubleVector3Extensions
    {
        public static Vector3 ToSingleVector(this DoubleVector3 _vector)
        {
            return new Vector3((float)_vector.x, (float)_vector.y, (float)_vector.z);
        }

        public static bool AreEqualToTolerance(DoubleVector3 _vectorA, DoubleVector3 _vectorB, double _tolerance)
        {
            if (
                EqualToTolerance(_vectorA.x, _vectorB.x, _tolerance) &&
                EqualToTolerance(_vectorA.y, _vectorB.y, _tolerance) &&
                EqualToTolerance(_vectorA.z, _vectorB.z, _tolerance)
                )
            {
                return true;
            }
            return false;
        }

        private static bool EqualToTolerance(double _a, double _b, double _tolerance)
        {
            if (_a >= (_b - _tolerance) && _a < (_b + _tolerance))
            {
                return true;
            }
            return false;
        }
    }
}