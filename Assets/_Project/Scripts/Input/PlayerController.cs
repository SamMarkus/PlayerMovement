// Script from git-amend

using UnityEngine;
using KBCore.Refs;
using Unity.Cinemachine;
using System;
using UnityEngine.Windows;

namespace PlayerMovement
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineCamera freeLookVCam;
        [SerializeField, Anywhere] InputReader inputReader;

        [Header("Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        const float ZeroF = 0f;

        Transform mainCam;

        float currentSpeed;
        float velocity;

        Vector3 movement;

        void Awake()
        {
            mainCam = Camera.main.transform;
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            freeLookVCam.OnTargetObjectWarped(
                transform,
                positionDelta: transform.position - freeLookVCam.transform.position - Vector3.forward
            );

            rb.freezeRotation = true;
        }

        void Start()
        {
            inputReader.EnablePlayerActions();
        }

        void Update()
        {
            movement = new Vector3(inputReader.Direction.x, 0f, inputReader.Direction.y);
            UpdateAnimator(); 
        }

        private void FixedUpdate()
        {
            // HandleJump();
            HandleMovement();
        }

        private void UpdateAnimator()
        {
            //Noop
        }

        private void HandleMovement()
        {
            var movementDirection = 
                new Vector3(x: inputReader.Direction.x, y: 0f, z: inputReader.Direction.y).normalized; 
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);

                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for snappy stop
                rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation =
                Quaternion.RotateTowards(from: transform.rotation,
                                         to: targetRotation,
                                         maxDegreesDelta: rotationSpeed * Time.deltaTime);
            transform.LookAt(worldPosition: transform.position + adjustedDirection);
        }

        private void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(current: currentSpeed,
                                                   target: value,
                                                   currentVelocity: ref velocity,
                                                   smoothTime: smoothTime);
        }
    }
}
