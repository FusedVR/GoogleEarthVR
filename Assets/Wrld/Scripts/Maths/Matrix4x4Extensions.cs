using UnityEngine;

namespace Wrld.Common.Maths
{
    public static class Matrix4x4Extensions
    {
        public static Quaternion ToQuaternion(this Matrix4x4 _matrix)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm

            var q = new Quaternion
                        {
                            w = Mathf.Sqrt(Mathf.Max(0, 1 + _matrix[0, 0] + _matrix[1, 1] + _matrix[2, 2]))/2,
                            x = Mathf.Sqrt(Mathf.Max(0, 1 + _matrix[0, 0] - _matrix[1, 1] - _matrix[2, 2]))/2,
                            y = Mathf.Sqrt(Mathf.Max(0, 1 - _matrix[0, 0] + _matrix[1, 1] - _matrix[2, 2]))/2,
                            z = Mathf.Sqrt(Mathf.Max(0, 1 - _matrix[0, 0] - _matrix[1, 1] + _matrix[2, 2]))/2
                        };

            q.x *= Mathf.Sign(q.x * (_matrix[2, 1] - _matrix[1, 2]));
            q.y *= Mathf.Sign(q.y * (_matrix[0, 2] - _matrix[2, 0]));
            q.z *= Mathf.Sign(q.z * (_matrix[1, 0] - _matrix[0, 1]));
            
            return q;
        }
    }
}
