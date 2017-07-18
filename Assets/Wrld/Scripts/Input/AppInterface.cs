// Copyright Wrld Ltd (2012-2014), All Rights Reserved
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld.MapInput.AppInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RotateData
	{
		public float rotation;
		public float velocity;
        public int numTouches;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct PinchData
	{
		public float scale;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct PanData
	{
		public Vector2 pointRelative;
		public Vector2 pointAbsolute;
        public Vector2 pointRelativeNormalized;
        public Vector2 velocity;
        public Vector2 touchExtents;
        public int		numTouches;
        public float majorScreenDimension;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct TapData
	{
		public Vector2 point;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchHeldData
	{
		public Vector2 point;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchData
	{
		public Vector2 point;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardData
	{
        public char keyCode;
        public uint metaKeys;
        public bool printable;
        public bool isKeyDown;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct ZoomData
	{
		public float distance;
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct TiltData
	{
		public float distance;
		public float screenHeight;
		public float screenPercentageNormalized;
	};
};
