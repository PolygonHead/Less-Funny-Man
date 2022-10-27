﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Transform characterCamera;
    public Transform funnyMan;
    public CharacterController controller;
    public Transform playerCenter;
    
    private float spinSpeed;
    private int throttle = -1;
    [SerializeField] private float mouseSensitivity = 2f; 
    private float xMove;
    private float zMove;
    private float cameraStartPositionY;
    public AudioSource startSound;
    public AudioSource continueSound;
    private bool startSoundPlayed;
    private float targetCameraHeight;
    public float maxGas = 100f;
    public float gasAmount;
    public float gasDepletionRate = 1 / 1000;
    public RectTransform sliderAmount;
    private float deltaYRotation = 0f;
    [SerializeField] private float movementSpeed = 10f; 
    private float cameraDistance = 17.5f;
    private float desiredCameraDistance = -17.5f;

    void Start()
    {
        cameraStartPositionY = characterCamera.localPosition.y;
        gasAmount = maxGas;
        Cursor.lockState = CursorLockMode.Locked;


    }
    void Update()
    {
        HandleInput();
        HandleCamera();
    }

    void HandleInput(){
        if (Input.GetKeyDown(KeyCode.Space) && gasAmount > 0)
        {
            throttle = 1;
            //if (controller.isGrounded)
            //{
            //    controller.Move(new Vector3(0, 10, 0));
            //}
        }
        else if (Input.GetKeyUp(KeyCode.Space) || gasAmount <= 0)
        {
            throttle = -1;
        }

        HandleMovement();
    }

    void HandleCamera(){
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        deltaYRotation = Mathf.Clamp(deltaYRotation + mouseY * mouseSensitivity, -70f, 70f);
        playerCenter.localRotation = Quaternion.Euler(deltaYRotation, 0, 0);
        transform.Rotate(transform.up * mouseX * mouseSensitivity);


        characterCamera.LookAt(playerCenter);
        targetCameraHeight = Mathf.Lerp(targetCameraHeight, Mathf.Clamp(-spinSpeed / 10, -9, 6), Time.deltaTime * 10);

        desiredCameraDistance = Mathf.Clamp(desiredCameraDistance + Input.GetAxis("Mouse ScrollWheel") * 3, -25f, -5f);
        cameraDistance = desiredCameraDistance;

        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit; // Do a raycast to make sure that the player is not on the ground
        Ray ray = new Ray(characterCamera.position, (playerCenter.position - characterCamera.position));
        Debug.DrawRay(characterCamera.position, (playerCenter.position - characterCamera.position) * 10, Color.yellow);
        // if (Physics.Raycast(camera.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        if (Physics.Raycast(ray, out hit, 10, layerMask))
        {
            print("Hey");
        }

        characterCamera.localPosition = new Vector3(0, targetCameraHeight + cameraStartPositionY, desiredCameraDistance);
        

    }

    void HandleMovement(){
        zMove = Input.GetAxis("Vertical") * Time.deltaTime * 20;
        xMove = Input.GetAxis("Horizontal") * Time.deltaTime * 20;
        
        spinSpeed += throttle * Time.deltaTime * 10;
        if (controller.isGrounded)
        {
            spinSpeed = 0;
            throttle = 0;
            controller.Move(new Vector3(0, -10, 0));
            if (startSoundPlayed)
            {
                continueSound.Play();
            }
            startSoundPlayed = false;
        } else if(throttle == 0 && !controller.isGrounded)
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            RaycastHit hit; // Do a raycast to make sure that the player is not on the ground
            Ray ray = new Ray(funnyMan.position, -transform.up);
            Debug.DrawRay(funnyMan.position, -transform.up * 5, Color.green);
            // if (Physics.Raycast(camera.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            if (!Physics.Raycast(ray, out hit, 5, layerMask))
            {
                throttle = -1;

            }
        }
        if (spinSpeed >= 5 && !startSoundPlayed)
        {
            startSound.Play();
            startSoundPlayed = true;
        }

        funnyMan.Rotate(0, spinSpeed, 0);
        Vector3 velocity = new Vector3(
            xMove, spinSpeed / 100, zMove
        );

        velocity = transform.TransformDirection(velocity); // Change movement direction from world space to local.
        controller.Move(velocity * Time.deltaTime * movementSpeed);    

        HandleGas();
    }

    void HandleGas(){
        if (throttle == 1)
        {
            gasAmount = Mathf.Clamp(gasAmount - Time.deltaTime * gasDepletionRate, 0, maxGas);
        }
        else if(throttle == -1)
        {
            gasAmount = Mathf.Clamp(gasAmount + Time.deltaTime * (gasDepletionRate/4f), 0, maxGas);
        }
        else
        {
            gasAmount = Mathf.Clamp(gasAmount + Time.deltaTime * (gasDepletionRate /1.5f), 0, maxGas);
        }

        sliderAmount.anchorMax = new Vector2(gasAmount / 100, 1);
    }
}