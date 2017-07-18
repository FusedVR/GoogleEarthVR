using UnityEngine;

namespace Wrld.MapCamera
{
    public static class ZoomLevelHelpers
    {
        private static double[] ms_zoomToDistances = new double[]
        {
            27428700,
            14720762,
            8000000,
            4512909,
            2087317,
            1248854,
            660556,
            351205,
            185652,
            83092,
            41899,
            21377,
            11294,
            5818,
            3106,
            1890,
            1300,
            821,
            500,
            300,
            108,
            58,
            31,
            17,
            9,
            5
        };

        public static double ZoomLevelToDistance(double zoomLevel)
        {
            int lowerBounds = Mathf.Clamp((int)zoomLevel, 0, ms_zoomToDistances.Length - 1);
            int upperBounds = Mathf.Clamp((int)(zoomLevel + 1), 0, ms_zoomToDistances.Length - 1);
            double interp = zoomLevel - lowerBounds;
            return ms_zoomToDistances[lowerBounds] + (ms_zoomToDistances[upperBounds] - ms_zoomToDistances[lowerBounds]) * interp;
        }

        public static double DistanceToZoomLevel(double distance)
        {
            for (int zoomLevelIndex = 0; zoomLevelIndex < ms_zoomToDistances.Length; ++zoomLevelIndex)
            {
                if (distance >= ms_zoomToDistances[zoomLevelIndex])
                {
                    if (zoomLevelIndex < ms_zoomToDistances.Length - 1)
                    {
                        int nextLevel = zoomLevelIndex + 1;
                        double interval = ms_zoomToDistances[nextLevel] - ms_zoomToDistances[zoomLevelIndex];
                        double interp = (distance - ms_zoomToDistances[zoomLevelIndex]) / interval;

                        return zoomLevelIndex;
                    }

                    return zoomLevelIndex;
                }
            }

            return ms_zoomToDistances.Length - 1;
        }

    }
}

