using Wrld.Common.Maths;
using System;
using Wrld.Helpers;
using System.Runtime.InteropServices;

namespace Wrld.Space
{
    /// <summary>
    /// Static helpers to convert between ECEF and latitude-Longitude-Altitude.
    /// These can then further be used to interact with other Wrld systems
    /// </summary>
    public static class CoordinateConversions
    {
        /// <summary>
        /// Convert latitude, longitude and altitude to Ecef. Returns the converted ECEF coordinate
        /// </summary>
        /// <param name="latitudeInRadians">Latitude in radians</param>
        /// <param name="longitudeInRadians">Longitude in radians</param>
        /// <param name="altitudeInMetres">Altitude in metres</param>
        public static DoubleVector3 ConvertLatLongAltitudeToEcef(double latitudeInRadians, double longitudeInRadians, double altitudeInMetres)
        {
            // Note: implementation is in .h due to client request.
            double radius = EarthConstants.Radius + altitudeInMetres;

            double x = radius * Math.Cos(latitudeInRadians) * Math.Cos(longitudeInRadians);
            double y = radius * Math.Cos(latitudeInRadians) * Math.Sin(longitudeInRadians);
            double z = radius * Math.Sin(latitudeInRadians);

            return new DoubleVector3(-y, z, x);
        }

        /// <summary>
        /// Convert an Ecef position to LatLongAltitude. Returns converted LatLongALtitude.
        /// </summary>
        /// <param name="x">Ecef x coordinate</param>
        /// <param name="y">Ecef y coordinate</param>
        /// <param name="z">Ecef z coordinate</param>
        public static LatLongAltitude ConvertEcefToLatLongAltitude(double x, double y, double z)
        {
            double p = Math.Sqrt((z * z) + (-x * -x));

            double lat = Math.Atan(y / p);
            double lon = Math.Atan2(-x, z);
            double alt = (p / Math.Cos(lat)) - EarthConstants.Radius;

            return LatLongAltitude.FromRadians(lat, lon, alt);
        }

        /// <summary>
        /// Convert an Ecef position to LatLongAltitude. Returns converted LatLongALtitude.
        /// </summary>
        /// <param name="ecef">The ECEF coordinates to convert</param>
        public static LatLongAltitude ConvertEcefToLatLongAltitude(DoubleVector3 ecef)
        {
            return ConvertEcefToLatLongAltitude(ecef.x, ecef.y, ecef.z);
        }
    }

    /// <summary>
    /// Value type that holds a latitude & longitude in degrees.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LatLong
    {
        

        /// <summary>
        /// Constructor for LatLong value type
        /// </summary>
        /// <param name="latitudeInDegrees">Latitude in degrees</param>
        /// <param name="longitudeInDegrees">Longitude in degrees</param>
        public LatLong(double latitudeInDegrees, double longitudeInDegrees)
        {
            m_latitudeInDegrees = latitudeInDegrees;
            m_longitudeInDegrees = longitudeInDegrees;
        }

        /// <summary>
        /// Convert the current latitude from degrees to radians and return it
        /// </summary>
        public double GetLatitudeInRadians() { return Helpers.MathsHelpers.Deg2Rad(m_latitudeInDegrees); }

        /// <summary>
        /// Convert the current longitude from degrees to radians and return it
        /// </summary>
        public double GetLongitudeInRadians() { return Helpers.MathsHelpers.Deg2Rad(m_longitudeInDegrees); }

        /// <summary>
        /// Set latitude in radians
        /// </summary>
        /// <param name="latitudeInRadians">Latitude in radians</param>
        public void SetLatitudeInRadians(double latitudeInRadians) { m_latitudeInDegrees = Helpers.MathsHelpers.Rad2Deg(latitudeInRadians); }

        /// <summary>
        /// Set longitude in radians
        /// </summary>
        /// <param name="longitudeInRadians">Longitude in radians</param>
        public void SetLongitudeInRadians(double longitudeInRadians) { m_longitudeInDegrees = Helpers.MathsHelpers.Rad2Deg(longitudeInRadians); }

        /// <summary>
        /// Set latitude in degrees
        /// </summary>
        /// <param name="latitudeInDegrees">latitude in degrees</param>
        public void SetLatitude(double latitudeInDegrees) { m_latitudeInDegrees = latitudeInDegrees; }

        /// <summary>
        /// Set longitude in degrees
        /// </summary>
        /// <param name="longitudeInDegrees">longitude in degrees</param>
        public void SetLongitude(double longitudeInDegrees) { m_longitudeInDegrees = longitudeInDegrees; }

        /// <summary>
        /// Return the latitude in degrees
        /// </summary>
        public double GetLatitude() { return m_latitudeInDegrees; }

        /// <summary>
        /// Return the longitude in degrees
        /// </summary>
        public double GetLongitude() { return m_longitudeInDegrees; }

        /// <summary>
        /// Convert the lat-long pair to an ECEF coordinate using altitude as 0.0
        /// </summary>
        public DoubleVector3 ToECEF()
        {
            return CoordinateConversions.ConvertLatLongAltitudeToEcef(Helpers.MathsHelpers.Deg2Rad(m_latitudeInDegrees), Helpers.MathsHelpers.Deg2Rad(m_longitudeInDegrees), 0.0f);
        }

        /// <summary>
        /// Get a bearing / direction between the current lat-long and the lat-long provided
        /// </summary>
        /// <param name="toPoint">the point the returned bearing should point towards</param>
        public double BearingTo(LatLong toPoint)
        {
            double deltaLong = toPoint.GetLongitudeInRadians() - GetLongitudeInRadians();
            double toLatitudeInRadians = toPoint.GetLatitudeInRadians();
            double y = Math.Sin(deltaLong) * Math.Cos(toLatitudeInRadians);
            double x = Math.Cos(GetLatitudeInRadians()) * Math.Sin(toLatitudeInRadians) - Math.Sin(GetLatitudeInRadians()) * Math.Cos(toLatitudeInRadians) * Math.Cos(deltaLong);
            double bearing = Math.Atan2(y, x);

            return MathsHelpers.Rad2Deg((bearing + Math.PI * 2) % (Math.PI * 2));
        }

        /// <summary>
        /// Estimates the length (in meters) of a great circle arc along the Earth's surface, between 
        /// the two lat-longs provided.  This does not take account of changes in surface altitude.
        /// </summary>
        /// <param name="a">The first of the points to find the distance between</param>
        /// <param name="b">The second of the points to find the distance between</param>
        /// <returns>The estimated distance between the two points, in meters</returns>
        public static double EstimateGreatCircleDistance(LatLong a, LatLong b)
        {
            return EstimateGreatCircleDistance(ref a, ref b, EarthConstants.Radius);
        }

        /// <summary>
        /// Converts a given ECEF position to a surface latitude-longitude
        /// </summary>
        /// <param name="world">World ECEF position</param>
        public static LatLong FromECEF(DoubleVector3 world)
        {
            return CoordinateConversions.ConvertEcefToLatLongAltitude(world).GetLatLong();
        }

        /// <summary>
        /// Instantiates the type and returns an instance
        /// </summary>
        /// <param name="latitudeInDegrees">Latitude in degrees</param>
        /// <param name="longitudeInDegrees">Longitude in degrees</param>
        public static LatLong FromDegrees(double latitudeInDegrees, double longitudeInDegrees)
        {
            return new LatLong(latitudeInDegrees, longitudeInDegrees);
        }

        /// <summary>
        /// Converts a lat-long in radians to degrees and returns an instance
        /// </summary>
        /// <param name="latitudeInRadians">Latitude in radians</param>
        /// <param name="longitudeInRadians">Longitude in radians</param>
        public static LatLong FromRadians(double latitudeInRadians, double longitudeInRadians)
        {
            return new LatLong(Helpers.MathsHelpers.Rad2Deg(latitudeInRadians), Helpers.MathsHelpers.Rad2Deg(longitudeInRadians));
        }

        public static implicit operator LatLongAltitude(LatLong ll)
        {
            return new LatLongAltitude(ll.m_latitudeInDegrees, ll.m_longitudeInDegrees, 0.0);
        }

        [DllImport(NativePluginRunner.DLL)]
        private static extern double EstimateGreatCircleDistance(
            ref LatLong a,
            ref LatLong b,
            double sphereRadius
            );

        private double m_latitudeInDegrees;
        private double m_longitudeInDegrees;
    };

    /// <summary>
    /// Value type that holds a latitude & longitude in degrees and altitude in metres
    /// </summary>
    public struct LatLongAltitude
    {
        /// <summary>
        /// Creates an instance of the type. No conversion takes place since all values are stored as is.
        /// </summary>
        /// <param name="latitudeInDegrees">Latitude in degrees</param>
        /// <param name="longitudeInDegrees">Longitude in degrees</param>
        /// <param name="altitudeInMetres">Altitude in metres</param>
        public LatLongAltitude(double latitudeInDegrees, double longitudeInDegrees, double altitudeInMetres)
        {
            m_latLong = new LatLong(latitudeInDegrees, longitudeInDegrees);
            m_altitude = altitudeInMetres;
        }

        /// <summary>
        /// Retrieve the LatLong without an altitude parameter
        /// </summary>
        public LatLong GetLatLong() { return m_latLong; }

        /// <summary>
        /// Get the store altitude in metres
        /// </summary>
        public double GetAltitude() { return m_altitude; }

        /// <summary>
        /// Set the current altitude in metres
        /// </summary>
        /// <param name="altitudeInMetres">Altitude in metres</param>
        public void SetAltitude(double altitudeInMetres) { m_altitude = altitudeInMetres; }

        /// <summary>
        /// Convert the latitude from degrees to radians and return it
        /// </summary>
        public double GetLatitudeInRadians() { return m_latLong.GetLatitudeInRadians(); }

        /// <summary>
        /// Convert the longitude from degrees to radians and return it
        /// </summary>
        public double GetLongitudeInRadians() { return m_latLong.GetLongitudeInRadians(); }

        /// <summary>
        /// Set the Latitude in radians
        /// </summary>
        /// <param name="latitudeInRadians">Latitude in radians</param>
        public void SetLatitudeInRadians(double latitudeInRadians) { m_latLong.SetLatitudeInRadians(latitudeInRadians); }

        /// <summary>
        /// Set the Longitude in radians
        /// </summary>
        /// <param name="longitudeInRadians">Longitude in radians</param>
        public void SetLongitudeInRadians(double longitudeInRadians) { m_latLong.SetLongitudeInRadians(longitudeInRadians); }

        /// <summary>
        /// Set latitude in degrees
        /// </summary>
        /// <param name="latitudeInDegrees">latitude in degrees</param>
        public void SetLatitude(double latitudeInDegrees) { m_latLong.SetLatitude(latitudeInDegrees); }

        /// <summary>
        /// Set longitude in degrees
        /// </summary>
        /// <param name="longitudeInDegrees">longitude in degrees</param>
        public void SetLongitude(double longitudeInDegrees) { m_latLong.SetLongitude(longitudeInDegrees); }

        /// <summary>
        /// Return the latitude in degrees
        /// </summary>
        public double GetLatitude() { return m_latLong.GetLatitude(); }

        /// <summary>
        /// Return the longitude in degrees
        /// </summary>
        public double GetLongitude() { return m_latLong.GetLongitude(); }

        /// <summary>
        /// Create and return an ECEF world position from the underlying latitude, longitude and altitude
        /// </summary>
        public DoubleVector3 ToECEF()
        {
            return CoordinateConversions.ConvertLatLongAltitudeToEcef(GetLatitudeInRadians(), GetLongitudeInRadians(), GetAltitude());
        }

        /// <summary>
        /// Get a bearing / direction between the current lat-long and the lat-long provided
        /// </summary>
        /// <param name="toPoint">the point the returned bearing should point towards</param>
        public double BearingTo(LatLong toPoint)
        {
            return m_latLong.BearingTo(toPoint);
        }

        /// <summary>
        /// Instantiates a LatLongAltitude and returns it without any conversions taking place.
        /// </summary>
        /// <param name="latitudeInDegrees">Latitude in degrees</param>
        /// <param name="longitudeInDegrees">Longitude in degrees</param>
        /// <param name="altitudeInMetres">Altitude in metres</param>
        public static LatLongAltitude FromDegrees(double latitudeInDegrees, double longitudeInDegrees, double altitudeInMetres)
        {
            return new LatLongAltitude(latitudeInDegrees, longitudeInDegrees, altitudeInMetres);
        }

        /// <summary>
        /// Converts a lat-long in radians to degrees and return an instance
        /// </summary>
        /// <param name="latitudeInRadians">Latitude in radians</param>
        /// <param name="longitudeInRadians">Longitude in radians</param>
        /// <param name="altitudeInMetres">Altitude in radians</param>
        public static LatLongAltitude FromRadians(double latitudeInRadians, double longitudeInRadians, double altitudeInMetres)
        {
            return new LatLongAltitude(Helpers.MathsHelpers.Rad2Deg(latitudeInRadians), Helpers.MathsHelpers.Rad2Deg(longitudeInRadians), altitudeInMetres);
        }

        /// <summary>
        /// Converts a given ECEF position to LatLongAltitude and returns it
        /// </summary>
        /// <param name="world">World ECEF position</param>
        public static LatLongAltitude FromECEF(DoubleVector3 world)
        {
            LatLong latLong = LatLong.FromECEF(world);
            double altitude = world.magnitude - EarthConstants.Radius;

            return new LatLongAltitude(latLong.GetLatitude(), latLong.GetLongitude(), altitude);
        }

        /// <summary>
        /// Static function, Lerps between two LatLongAltitude positions and returns the interpolated result based on the time parameter passed in.
        /// </summary>
        /// <param name="from">Starting position of the lerp transition.</param>
        /// <param name="to">Ending position of the lerp transition</param>
        /// <param name="time">Current time that has passed in the transition</param>
        public static LatLongAltitude Lerp(LatLongAltitude from, LatLongAltitude to, float time)
        {
            return new LatLongAltitude(
                from.GetLatitude() + ((to.GetLatitude() - from.GetLatitude()) * time),
                from.GetLongitude() + ((to.GetLongitude() - from.GetLongitude()) * time),
                from.GetAltitude() + ((to.GetAltitude() - from.GetAltitude()) * time));
        }

        private LatLong m_latLong;
        private double m_altitude;
    };
}