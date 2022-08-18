using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Transform characterCamera;
    public Transform funnyMan;
    public CharacterController controller;

    private float spinSpeed;
    private int throttle = -1;
    private float xMove;
    private float zMove;
    private Vector3 cameraStartPosition;
    public AudioSource startSound;
    public AudioSource continueSound;
    private bool startSoundPlayed;
    private float targetCameraHeight;

    void Start(){
        cameraStartPosition = characterCamera.localPosition;
    }
    void Update()
    {
        characterCamera.LookAt(funnyMan);
        zMove = Input.GetAxis("Vertical") * Time.deltaTime * 20;
        xMove = Input.GetAxis("Horizontal") * Time.deltaTime * 20;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            throttle = 1;
            if (controller.isGrounded){
                controller.Move(new Vector3(0, 10, 0));
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            throttle = -1;
        }
        spinSpeed += throttle * Time.deltaTime * 10;
        if (controller.isGrounded){
            spinSpeed = 0;
            throttle = 0;
            if (startSoundPlayed){
                continueSound.Play();
            }
            startSoundPlayed = false;
        }
        if (spinSpeed >= 5 && !startSoundPlayed){
            startSound.Play();
            startSoundPlayed = true;
        }

        targetCameraHeight = Mathf.Lerp(targetCameraHeight, Mathf.Clamp(-spinSpeed / 10, -9, 6), Time.deltaTime * 10);
        characterCamera.localPosition = cameraStartPosition + new Vector3(0, targetCameraHeight, 0);
        funnyMan.Rotate(0, spinSpeed, 0);
        controller.Move(new Vector3(xMove, spinSpeed / 100, zMove));
    }
}
