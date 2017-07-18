using Wrld.Common.Maths;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld.MapCamera
{
    [StructLayout(LayoutKind.Sequential)]
    public class CameraState
    {
        public DoubleVector3 LocationEcef;
        public DoubleVector3 InterestPointEcef;
        public Matrix4x4 ViewMatrix;
        public Matrix4x4 ProjectMatrix;

        public CameraState() {
        }

        public CameraState(DoubleVector3 locationEcef,
            DoubleVector3 interestPointEcef,
            Matrix4x4 viewMatrix,
            Matrix4x4 projectMatrix)
        {
            LocationEcef = locationEcef;
            InterestPointEcef = interestPointEcef;
            ViewMatrix = viewMatrix;
            ProjectMatrix = projectMatrix;
        }
    }
}