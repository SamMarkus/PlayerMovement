// Script from git-amend

using UnityEngine; 
using KBCore.Refs;
using Unity.Cinemachine;
using System.Collections;

namespace PlayerMovement
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [Header("Refernece")]
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Anywhere] CinemachineOrbitalFollow freeLookVCam;

        [Header("Settings")]
        [SerializeField, Range(0.5f, 3f)] float speedMultiplier = 1f;


        bool isRMBPressed;
        bool cameraMovementLock;

        private void OnEnable()
        {
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera; 
        }
        private void OnDisable()
        {
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        private void OnEnableMouseControlCamera()
        {
            isRMBPressed = true;

            // Lock the cursor to the center of the screen and hided it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame()); 
        }

        private void OnDisableMouseControlCamera()
        {
            isRMBPressed = false;

            // Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Reset the camera axis to prevent jumping when re-enabling mouse control
            freeLookVCam.HorizontalAxis.Reset();
            freeLookVCam.VerticalAxis.Reset();
        }

        private IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;
        }

        private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (cameraMovementLock) return;

            if (isDeviceMouse && !isRMBPressed) return;

            // If the device is mouse use fixedDeltaTime, otherwise use deltaTime
            float deviceMulitplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            // Set the camera axis values
            freeLookVCam.HorizontalAxis.Value = cameraMovement.x * speedMultiplier * deviceMulitplier;
            freeLookVCam.VerticalAxis.Value = cameraMovement.y * speedMultiplier * deviceMulitplier;
        }
    }
}
