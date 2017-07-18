using System;
using UnityEngine;

namespace Wrld.Common.Maths
{
    /// <summary>
    /// This tangent basis class defines a coordinate frame that is at a point on the surface of the earth.
    /// The tangent at that point on the surface is used to create this local coordinate system and is useful 
    /// for positioning objects relative to this reference frame.
    /// </summary>
    public class EcefTangentBasis
    {
        DoubleVector3 m_pointEcef;
        Vector3 m_basisRight;
        Vector3 m_basisUp;
        Vector3 m_basisForward;

        /// <summary>
        /// Construct the tangent basis from its constituent vectors
        /// </summary>
        /// <param name="pointEcef">Point on the surface of the earth in ECEF</param>
        /// <param name="tangentRight">Right unit vector at the ECEF point</param>
        /// <param name="tangentUp">Up unit vector at the ECEF point</param>
        /// <param name="tangentForward">Forward unit vector at the ECEF point</param>
        public EcefTangentBasis(DoubleVector3 pointEcef, Vector3 tangentRight, Vector3 tangentUp, Vector3 tangentForward)
        {
            m_pointEcef = pointEcef;
            m_basisRight = tangentRight.normalized;
            m_basisUp = tangentUp.normalized;
            m_basisForward = tangentForward.normalized;
        }

        /// <summary>
        /// Create basis using a zero vector for all its members (point, up, right, forward)
        /// </summary>
        public EcefTangentBasis()
        {
            m_pointEcef = DoubleVector3.zero;
            m_basisRight = Vector3.zero;
            m_basisUp = Vector3.zero;
            m_basisForward = Vector3.zero;
        }

        /// <summary>
        /// Create a basis using a surface ECEF point and a vector heading.
        /// </summary>
        /// <param name="pointEcef">Point on the surface of the earth in ECEF</param>
        /// <param name="heading">A vector pointing along the forward direction</param>
        public EcefTangentBasis(DoubleVector3 pointEcef, Vector3 heading)
        {
            Set(pointEcef, heading);
        }

        /// <summary>
        /// Get the surface point of the tangent basis in ECEF
        /// </summary>
        public DoubleVector3 PointEcef { get { return m_pointEcef; } }

        /// <summary>
        /// Get the right vector of the tangent plane
        /// </summary>
        public Vector3 Right { get { return m_basisRight; } }

        /// <summary>
        /// Get the up vector of the tangent plane
        /// </summary>
        public Vector3 Up { get { return m_basisUp; } }

        /// <summary>
        /// Get the forward vector of the tangent plane
        /// </summary>
        public Vector3 Forward { get { return m_basisForward; } }

        private void CalculateTangentBasisVectorsFromPointAndHeading( DoubleVector3 ecefPoint, Vector3 direction, out Vector3 basisRight, out Vector3 basisUp, out Vector3 basisForward)
        {
            const float epsilon = 0.0001f;
            basisUp = ecefPoint.ToSingleVector().normalized;
            Vector3 right = Vector3.Cross(basisUp, direction);

            if (right.magnitude < epsilon)
            {
                Vector3 yAxis = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 xAxis = new Vector3(1.0f, 0.0f, 0.0f);

                float dot = Vector3.Dot(basisUp, yAxis);

                if (Math.Abs(dot) < (1.0f - epsilon))
                {
                    right = Vector3.Cross(basisUp, yAxis);
                }
                else
                {
                    right = Vector3.Cross(basisUp, xAxis);
                }
            }

            basisRight = right.normalized;
            basisForward = Vector3.Cross(basisRight, basisUp).normalized;
        }

        /// <summary>
        /// Calculate a new tangent plane and set its parameters in the tangent basis
        /// </summary>
        /// <param name="pointEcef">The point on the surface of the earth in ECEF</param>
        /// <param name="heading">A vector pointing along the forward direction</param>
        public void Set(DoubleVector3 pointEcef, Vector3 heading)
        {
            CalculateTangentBasisVectorsFromPointAndHeading(pointEcef, heading, out m_basisRight, out m_basisUp, out m_basisForward);
            m_pointEcef = pointEcef;
        }

        /// <summary>
        /// Calculate a new tangent plane using the stored forward vector
        /// </summary>
        /// <param name="pointEcef">The point on the surface of the earth in ECEf</param>
        public void SetPoint(DoubleVector3 pointEcef)
        {
            Set(pointEcef, m_basisForward);
        }

        /// <summary>
        /// Calculate a new tangent plane using the stored surface ECEF point
        /// </summary>
        /// <param name="heading">A vector pointing along the forward direction</param>
        public void SetHeading(Vector3 heading)
        {
            Set(m_pointEcef, heading);
        }
    }
}
