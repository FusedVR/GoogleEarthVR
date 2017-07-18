using UnityEngine;
using System;

namespace Wrld.Common.Maths
{
    /// <summary>
    /// Static methods to convert from and to lat-long-alt and ECEF world positions.
    /// These helpers are not associated with the types that encapsulate these and instead deal with individual / simple data.
    /// </summary>
    public class EcefHelpers
    {
        const double EarthRadius = 6378100.0;

        /// <summary>
        /// Converts latitude, longitude and altitude into a world ECEF position.
        /// </summary>
        /// <param name="latitudeInRadians">Latitude in radians</param>
        /// <param name="longitudeInRadians">Longitude in radians</param>
        /// <param name="altitudeInMeters">Altitude in meters</param>
        public static DoubleVector3 LatLongAltToEcefDegrees(double latitudeInRadians, double longitudeInRadians, double altitudeInMeters)
        {
            latitudeInRadians *= Math.PI / 180.0;
            longitudeInRadians *= Math.PI / 180.0;

            // Note: implementation is in .h due to client request.
            double radius = EarthRadius + altitudeInMeters;

            double x = radius * Math.Cos(latitudeInRadians) * Math.Cos(longitudeInRadians);
            double y = radius * Math.Cos(latitudeInRadians) * Math.Sin(longitudeInRadians);
            double z = radius * Math.Sin(latitudeInRadians);

            return new DoubleVector3(-y, z, x);
        }

        /// <summary>
        /// Converts an ECEF world position and a heading (direction you are facing) into a tangent basis
        /// at that point, which can further be used to make calculations in that tangent space.
        /// </summary>
        /// <param name="worldPointEcef">World position in the ECEF system</param>
        /// <param name="absoluteHeadingDegrees">Absolute heading in degrees</param>
        public static EcefTangentBasis EcefTangentBasisFromPointAndHeading(DoubleVector3 worldPointEcef, float absoluteHeadingDegrees)
        {
            var heading = new Vector3(0.0f, 1.0f, 0.0f);

            var tempBasis = new EcefTangentBasis();
            tempBasis.Set(worldPointEcef, heading);

            var up = tempBasis.Up;
            var q = Quaternion.AngleAxis(absoluteHeadingDegrees, up);
            heading = q.RotatePoint(heading);

            var basis = new EcefTangentBasis();
            basis.Set(worldPointEcef, heading);
            return basis;
        }
    }
}
