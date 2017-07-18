using Wrld.Common.Maths;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeCameraState
    {
        public float nearClipPlaneDistance;
        public float farClipPlaneDistance;
        public float fieldOfViewDegrees;
        public float aspect;
        public float tiltDegrees;
        public float distanceToInterestPoint;
        public DoubleVector3 originECEF;
        public DoubleVector3 interestPointECEF;
        public Vector3 interestBasisRightECEF;
        public Vector3 interestBasisUpECEF;
        public Vector3 interestBasisForwardECEF;
    };
};

