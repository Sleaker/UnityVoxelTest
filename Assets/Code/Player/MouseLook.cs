using UnityEngine;

namespace Voxel.Player
{
    [AddComponentMenu("Camera-Control/Mouse Look")]
    public class MouseLook : MonoBehaviour
    {
        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float SensitivityX = 15F;
        public float SensitivityY = 15F;

        public float MinimumX = -360F;
        public float MaximumX = 360F;

        public float MinimumY = -85F;
        public float MaximumY = 85F;

        float rotationY;

        void Update()
        {
            if (Screen.lockCursor)
            {
                switch (axes)
                {
                    case RotationAxes.MouseXAndY:
                    {
                        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X")*SensitivityX;

                        rotationY += Input.GetAxis("Mouse Y")*SensitivityY;
                        rotationY = Mathf.Clamp(rotationY, MinimumY, MaximumY);

                        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                    }
                        break;
                    case RotationAxes.MouseX:
                        transform.Rotate(0, Input.GetAxis("Mouse X")*SensitivityX, 0);
                        break;
                    default:
                        rotationY += Input.GetAxis("Mouse Y")*SensitivityY;
                        rotationY = Mathf.Clamp(rotationY, MinimumY, MaximumY);
                        transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                        break;
                }
            }
        }

        void Start()
        {
            // Make the rigid body not change rotation
            if (rigidbody)
                rigidbody.freezeRotation = true;
        }
    }
}
