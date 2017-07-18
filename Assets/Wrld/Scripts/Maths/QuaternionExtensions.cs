using UnityEngine;

namespace Wrld.Common.Maths
{
    public static class QuaternionExtensions
    {
        public static Vector3 RotateVector(this Quaternion _quaternion, Vector3 _vector)
        {
            return _quaternion * _vector;
        }

        public static Vector3 RotatePoint(this Quaternion _quaternion, Vector3 _point)
        {
            return _quaternion * _point;
        }
    }
}
