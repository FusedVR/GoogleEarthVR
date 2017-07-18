using UnityEngine;

namespace Wrld
{
    public class FlyCameraBehaviour : MonoBehaviour
    {
        public UnityEngine.Camera Target;
        public float MaxSpeed = 500.0f;
        public float Acceleration = 150.0f;
        public float Drag = 0.95f;
        public float XSensitivity = 20.0f;
        public float YSensitivity = 20.0f;

        private Vector3 m_velocity;

        void Update()
        {
            if (Api.Instance.CameraApi.IsTransitioning)
            {
                m_velocity = Vector3.zero;
                return;
            }

            if (Input.GetMouseButton(1))
            {
                AdjustCameraRotation(-Input.GetAxis("Mouse Y") * YSensitivity, Input.GetAxis("Mouse X") * XSensitivity);
            }

            AdjustCameraPosition();
        }

        private void AdjustCameraPosition()
        {
            var accelerationDirection = Target.transform.rotation * GetAccelerationDirectionFromInputs() * Acceleration;
            var drag = -m_velocity * Drag;
            m_velocity += (accelerationDirection + drag) * Time.deltaTime;

            if (m_velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
            {
                m_velocity = m_velocity.normalized * MaxSpeed;
            }

            Target.transform.position += m_velocity * Time.deltaTime;
        }

        private Vector3 GetAccelerationDirectionFromInputs()
        {
            var result = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
            {
                result.x -= 1.0f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                result.x += 1.0f;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                result.y -= 1.0f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                result.y += 1.0f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                result.z -= 1.0f;
            }
            if (Input.GetKey(KeyCode.W))
            {
                result.z += 1.0f;
            }

            if (!Vector3.zero.Equals(result))
            {
                result.Normalize();
            }

            return result;
        }

        private void AdjustCameraRotation(float changeInPitchDegrees, float changeInYawDegrees)
        {
            var originalPitch = Target.transform.eulerAngles.x;
            var originalYaw = Target.transform.eulerAngles.y;
            var pitch = originalPitch + changeInPitchDegrees;
            var yaw = originalYaw + changeInYawDegrees;

            // Here we're reconstructing the pitch according to Unity's decomposition to Euler angles,
            // in which a camera facing the horizon has a pitch angle of 0 or 360 degrees, straight up
            // is 270 degrees and straight down is 90 degrees.  As such this is a bit more complicated
            // than a clamp between -90 and 90, but represents the same thing.
            if (originalPitch > 180.0f)
            {
                pitch = Mathf.Max(pitch, 270.0f);

                if (pitch > 360.0f)
                {
                    pitch = Mathf.Min(pitch - 360.0f, 90.0f);
                }
            }
            else
            {
                pitch = Mathf.Min(pitch, 90.0f);

                if (pitch < 0.0f)
                {
                    pitch = Mathf.Max(pitch + 360.0f, 270.0f);
                }
            }

            yaw = Mathf.Repeat(yaw, 360.0f);
            Target.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}