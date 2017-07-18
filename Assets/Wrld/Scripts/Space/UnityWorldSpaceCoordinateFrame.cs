using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.Space
{
    public class UnityWorldSpaceCoordinateFrame
    {
        public DoubleVector3 OriginECEF { get { return m_originECEF; } }
        public Quaternion ECEFToLocalRotation { get; private set; }
        public Quaternion LocalToECEFRotation { get; private set; }

        public UnityWorldSpaceCoordinateFrame(LatLongAltitude centralPoint)
        {
            SetCentralPoint(centralPoint);
        }

        public void SetCentralPoint(LatLongAltitude latLongAlt)
        {
            SetCentralPoint(latLongAlt.ToECEF(), latLongAlt);
        }

        public void SetCentralPoint(DoubleVector3 ecefOrigin)
        {
            SetCentralPoint(ecefOrigin, LatLongAltitude.FromECEF(ecefOrigin));
        }

        // :TODO: put this somewhere sensible
        private static Quaternion CreateQuaternionFromBasisVectors(Vector3 right, Vector3 up, Vector3 forward)
        {
            var basis = Matrix4x4.identity;
            basis.SetRow(0, new Vector4(right.x, right.y, right.z, 0.0f));
            basis.SetRow(1, new Vector4(up.x, up.y, up.z, 0.0f));
            basis.SetRow(2, new Vector4(forward.x, forward.y, forward.z, 0.0f));

            return basis.ToQuaternion();
        }

        private void SetCentralPoint(DoubleVector3 ecefOrigin, LatLongAltitude latLongAltOrigin)
        {
            m_originLatLongAlt = latLongAltOrigin;
            m_originECEF = ecefOrigin;

            var upEcef = m_originECEF.normalized;
            var northPole = new DoubleVector3(0.0, 1.0, 0.0);
            var toNorthPole = (northPole - upEcef).normalized;

            m_upECEF = upEcef.ToSingleVector();
            m_rightECEF = -DoubleVector3.Cross(toNorthPole, upEcef).normalized.ToSingleVector();
            m_forwardECEF = -Vector3.Cross(m_upECEF, m_rightECEF);

            ECEFToLocalRotation = CreateQuaternionFromBasisVectors(m_rightECEF, m_upECEF, m_forwardECEF);
            LocalToECEFRotation = Quaternion.Inverse(ECEFToLocalRotation);
        }

        public Vector3 ECEFToLocalSpace(DoubleVector3 ecef)
        {
            Vector3 offsetFromOrigin = (ecef - m_originECEF).ToSingleVector();

            return ECEFToLocalRotation * offsetFromOrigin;
        }
        public DoubleVector3 LocalSpaceToECEF(Vector3 localSpace)
        {
            return m_originECEF + LocalToECEFRotation * localSpace;
        }

        public LatLongAltitude LocalSpaceToLatLongAltitude(Vector3 localSpace)
        {
            return LatLongAltitude.FromECEF(LocalSpaceToECEF(localSpace));
        }
        public Vector3 LatLongAltitudeToLocalSpace(LatLongAltitude latLongAlt)
        {
            return ECEFToLocalSpace(latLongAlt.ToECEF());
        }

        private LatLongAltitude m_originLatLongAlt;
        private DoubleVector3 m_originECEF;
        private Vector3 m_upECEF;
        private Vector3 m_rightECEF;
        private Vector3 m_forwardECEF;
    }

}