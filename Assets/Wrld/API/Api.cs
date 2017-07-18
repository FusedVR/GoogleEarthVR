using Wrld.MapCamera;
using Wrld.Scripts.Utilities;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld
{
    /// <summary>
    /// The Wrld API defines two different map behaviors / systems which allow different ways
    /// to interact with Unity. Each system has its pros and cons outlined in the Examples page.
    /// </summary>
    public enum CoordinateSystem
    {
        /// <summary>
        /// This system allows easier interaction with Unity by grounding all meshes as per Unitys coordinate system.
        /// The main disadvantage is that is doesn't support interacting with faraway map points very well.
        /// </summary>
        UnityWorld,

        /// <summary>
        /// This system very closely represents the curvature of the earth and has accurate rendering and manipulation of meshes.
        /// It does not work seamlessly with the Unity coordinate system and requires additional calculation to manipulate objects.
        /// </summary>
        ECEF
    }

    public class Api
    {
        public static Api Instance = null;

        const string ApiNullAssertMessage = "API is uninitialized. Please call CreateApi(...) before making any calls to the API";
        const string InvalidApiKeyExceptionMessage = "\"{0}\" is not a valid API key.  Please get a key from https://wrld3d.com/developers/apikeys.";

        private ApiImplementation m_implementation;

        private Api(string apiKey, CoordinateSystem coordinateSystem, Transform parentTransformForStreamedObjects, ConfigParams configParams)
        {
            if (!APIKeyPrevalidator.AppearsValid(apiKey))
            {
                throw new InvalidApiKeyException(string.Format(InvalidApiKeyExceptionMessage, apiKey));
            }

            try
            {
                m_implementation = new ApiImplementation(apiKey, coordinateSystem, parentTransformForStreamedObjects, configParams);
            }
            catch (DllNotFoundException dllNotFound)
            {
                bool couldNotFindWRLDBinary = dllNotFound.Message.ToLower().Contains("streamalpha");
                bool is32Bit = IntPtr.Size == 4;

                if (couldNotFindWRLDBinary && is32Bit)
                {
                    var guiTextObject = new GameObject("OtherErrorMessage");
                    var errorMessage = guiTextObject.AddComponent<ErrorMessage>();
                    errorMessage.Title = "WRLD Error: Unsupported Build Architecture";
                    errorMessage.Text = 
                        "It looks like you're trying to run a 32 bit build of the game.  Unfortunately that isn't currently supported.\n\n" + 
                        "Please go to 'File->Build Settings' in the Unity menu and select 'x86_64' as your Architecture to continue.";
                }
                else
                {
                    throw;
                }
            }
        }

        public static void Create(string apikey, CoordinateSystem coordinateSystem, Transform parentTransformForStreamedObjects, ConfigParams configParams)
        {
            if (Instance == null)
            {
                Instance = new Api(apikey, coordinateSystem, parentTransformForStreamedObjects, configParams);
            }
            else
            {
                throw new ArgumentException("Api has already been initialized. Use Api.Instance to access it.");
            }
        }

        public void StreamResourcesForCamera(UnityEngine.Camera camera)
        {
            m_implementation.StreamResourcesForCamera(camera);
        }

        public void Update()
        {
            m_implementation.Update();
        }

        public void Destroy()
        {
            m_implementation.Destroy();
            Instance = null;
        }

        public void SetOriginPoint(Space.LatLongAltitude lla)
        {
            m_implementation.SetOriginPoint(lla);
        }

        public void OnApplicationPaused()
        {
            m_implementation.OnApplicationPaused();
        }

        public void OnApplicationResumed()
        {
            m_implementation.OnApplicationResumed();
        }

        public CameraApi CameraApi
        {
            get
            {
                return m_implementation.CameraApi;
            }
        }
    }
}
