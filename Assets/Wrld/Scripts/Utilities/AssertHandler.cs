using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wrld
{
    public class AssertHandler
    {
        public delegate void HandleAssertCallback(IntPtr message, IntPtr file, int line);

        [MonoPInvokeCallback(typeof(HandleAssertCallback))]
        public static void HandleAssert(IntPtr message, IntPtr file, int line)
        {
            var output = Marshal.PtrToStringAnsi(message);
            var fileName = Marshal.PtrToStringAnsi(file);

            Debug.Log(string.Format("Wrld ASSERT {0} ({1}): {2}", fileName, line, output));

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            Debug.Break();
        }
    }

}