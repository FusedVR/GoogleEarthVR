using UnityEngine;
using System;

namespace Wrld.Helpers
{
    public static class MathsHelpers
    {
        public static float expDecayFactor(float halfLife, float deltaTime)
        {
            const float LN2 = 0.6931471805599453f;
            float c = LN2 / halfLife;
            return Mathf.Exp(-c * deltaTime);
        }

        public static double Rad2Deg(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        public static double Deg2Rad(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        public static float SinEaseInOut(float t)
        {
            return 1.0f - 0.5f * (1.0f + Mathf.Cos(t * Mathf.PI));
        }
    }
}
    