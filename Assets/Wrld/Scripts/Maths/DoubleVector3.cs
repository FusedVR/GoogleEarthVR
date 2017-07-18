using System;
using System.Diagnostics;
using UnityEngine;

namespace Wrld.Common.Maths
{
    /// <summary>
    /// Representation of 3 component (3D) vector and points in double precision required for dealing with very large scale precision.
    /// </summary>
    [DebuggerDisplay("{x}, {y}, {z}")]
    public struct DoubleVector3 : IEquatable<DoubleVector3>
    {
        private const string StringFormat = "{0},{1},{2}";

        /// <summary>
        /// Represents a zero vector with all 3 axes set to 0.0
        /// </summary>
        public static readonly DoubleVector3 zero = new DoubleVector3(0.0, 0.0, 0.0);

        /// <summary>
        /// Represents a vector with all axes set to 1.0
        /// </summary>
        public static readonly DoubleVector3 one = new DoubleVector3(1.0, 1.0, 1.0);

        /// <summary>
        /// x component of the vector, points right
        /// </summary>
        public double x;

        /// <summary>
        /// y component of the vector, points up 
        /// </summary>
        public double y;

        /// <summary>
        /// z component of the vector, points forward
        /// </summary>
        public double z;

        /// <summary>
        /// Calculates and returns the vector magnitude
        /// </summary>
        public double magnitude { get { return Math.Sqrt(sqrMagnitude); } }

        /// <summary>
        /// Calculates and returns the square of the magnitude.
        /// Does not require calculating the square root.
        /// </summary>
        public double sqrMagnitude { get { return Dot(this, this); } }

        /// <summary>
        /// Returns the normalized version of the vector. Does not store this value.
        /// </summary>
        public DoubleVector3 normalized { get { return this / magnitude; } }

        /// <summary>
        /// Initializes the vector with its 3 main component values
        /// </summary>
        /// <param name="X">x axis</param>
        /// <param name="Y">y axis</param>
        /// <param name="Z">z axis</param>
        public DoubleVector3(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        /// Conjunction comparison with each respective element
        /// This comparison does not use epsilon approximation
        /// </summary>
        /// <param name="other">Vector to compare against</param>
        public bool Equals(DoubleVector3 other)
        {
            return (x == other.x) && (y == other.y) && (z == other.z);
        }

        /// <summary>
        /// Conjunction comparison with each respective element.
        /// Only succeeds if object is of the same type.
        /// This comparison does not use epsilon approximation
        /// </summary>
        /// <param name="other">Vector to compare against</param>
        public override bool Equals(object other)
        {
            bool flag = false;

            if (other is DoubleVector3)
            {
                flag = Equals((DoubleVector3)other);
            }
            return flag;
        }

        /// <summary>
        /// Returns the combined hash of all three internal x-y-z axes
        /// </summary>
        public override int GetHashCode()
        {
            return (x.GetHashCode() << 1) ^ y.GetHashCode() ^ (z.GetHashCode() >> 1);
        }

        /// <summary>
        /// Returns a string representatin of the vector of the form x, y, z
        /// </summary>
        public override string ToString()
        {
            return string.Format(StringFormat, x, y, z);
        }

        public static implicit operator DoubleVector3(Vector2 _rhs)
        {
            return new DoubleVector3(_rhs.x, _rhs.y, 0.0f);
        }

        public static implicit operator DoubleVector3(Vector3 _rhs)
        {
            return new DoubleVector3(_rhs.x, _rhs.y, _rhs.z);
        }

        public static DoubleVector3 operator +(DoubleVector3 _lhs, DoubleVector3 _rhs)
        {
            return new DoubleVector3 { x = _lhs.x + _rhs.x, y = _lhs.y + _rhs.y, z = _lhs.z + _rhs.z };
        }

        public static DoubleVector3 operator -(DoubleVector3 _lhs, DoubleVector3 _rhs)
        {
            return new DoubleVector3 { x = _lhs.x - _rhs.x, y = _lhs.y - _rhs.y, z = _lhs.z - _rhs.z };
        }

        public static DoubleVector3 operator *(DoubleVector3 _lhs, double _rhs)
        {
            return new DoubleVector3 { x = _lhs.x * _rhs, y = _lhs.y * _rhs, z = _lhs.z * _rhs };
        }

        public static DoubleVector3 operator *(double _lhs, DoubleVector3 _rhs)
        {
            return _rhs * _lhs;
        }

        public static DoubleVector3 operator /(DoubleVector3 _lhs, double _rhs)
        {
            return new DoubleVector3 { x = _lhs.x / _rhs, y = _lhs.y / _rhs, z = _lhs.z / _rhs };
        }

        public static bool operator ==(DoubleVector3 _lhs, DoubleVector3 _rhs)
        {
            return (_lhs.x == _rhs.x) && (_lhs.y == _rhs.y) && (_lhs.z == _rhs.z);
        }

        public static bool operator !=(DoubleVector3 _lhs, DoubleVector3 _rhs)
        {
            return (_lhs.x != _rhs.x) || (_lhs.y != _rhs.y) || (_lhs.z != _rhs.z);
        }

        /// <summary>
        /// Constructs and returns a new vector with the minimum of each component
        /// Eg: newX = Min(a.x, b.x)
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Secpmd vectpr</param>
        public static DoubleVector3 Min(DoubleVector3 a, DoubleVector3 b)
        {
            return new DoubleVector3 { x = Math.Min(a.x, b.x), y = Math.Min(a.y, b.y), z = Math.Min(a.z, b.z) };
        }

        /// <summary>
        /// Constructs and returns a new vector with the maximum of each component
        /// Eg: newX = Max(a.x, b.x)
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        public static DoubleVector3 Max(DoubleVector3 a, DoubleVector3 b)
        {
            return new DoubleVector3 { x = Math.Max(a.x, b.x), y = Math.Max(a.y, b.y), z = Math.Max(a.z, b.z) };
        }

        /// <summary>
        /// Calculates and returns the dot product of the two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        public static double Dot(DoubleVector3 a, DoubleVector3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        /// <summary>
        /// Calculates and returns the cross product of the two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        public static DoubleVector3 Cross(DoubleVector3 a, DoubleVector3 b)
        {
            return new DoubleVector3(
                (a.y * b.z) - (a.z * b.y),
                (a.z * b.x) - (a.x * b.z),
                (a.x * b.y) - (a.y * b.x));
        }

        /// <summary>
        /// Calcualates and returns a new interpolated vector based on the time parameter
        /// </summary>
        /// <param name="from">Starting vector position</param>
        /// <param name="to">Ending vector position</param>
        /// <param name="time">Current time to interpolate from</param>
        /// <returns></returns>
        public static DoubleVector3 Lerp(DoubleVector3 from, DoubleVector3 to, float time)
        {
            return new DoubleVector3
                (
                from.x + (time * (to.x - from.x)),
                from.y + (time * (to.y - from.y)),
                from.z + (time * (to.z - from.z))
                );
        }
    }
}
