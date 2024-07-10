using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.Xml;

// Trigger FoV Main Object Function and Raycast Distance Function

public class LeftHandFunctions : MonoBehaviour
{
    SpeechSynthesis SpeechSynthesis;
    CameraFieldOfView cameraFieldOfView;
    SpeechRecognition SpeechRecognition;
    // CameraController cameraController;
    RightHandFunctions buttonCheckFieldOfView;
    bool IsCheckFoV = false;
    private bool prevPrimaryButtonState_ = false;
    private bool prevTriggerButtonState_ = false;
    private bool rayReadTrigger = false;
    private float timer = 0f; // Timer to track the elapsed time
    public Transform controller;
    public float maxDistance = 10f;
    public float maxInterval = 3f;
    private AudioSource audioSource;
    public List<GameObject> readObjects;
    public float newImportance;
    public float originalImportance;
    public Dictionary<string, float> newImportanceValues;
    int i = 0;
    int j = 0;
    // private bool isSpoken = false;

    void Start()
    {
        SpeechSynthesis = GetComponent<SpeechSynthesis>();
        cameraFieldOfView = GetComponent<CameraFieldOfView>();
        buttonCheckFieldOfView= GetComponent<RightHandFunctions>(); 
        SpeechRecognition = GetComponent<SpeechRecognition>();
        readObjects = new List<GameObject>();
        // Check if an AudioSource component is already attached
        if (audioSource == null)
        {
            // Create and attach a new AudioSource component
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Load the audio clip from the Resources folder
        AudioClip clip = Resources.Load<AudioClip>("beep");

        // Set the audio clip for the repeating sound
        audioSource.clip = clip;

    }

    private async void Update()
    {
        //if (cameraFieldOfView.leftprimaryButtonDown && !prevPrimaryButtonState_)
        if (SpeechRecognition.prevMainObjectState_)
        {
            if (buttonCheckFieldOfView.beginnerMode)
            {
                SpeechSynthesis.SpeakText("Left primary button pressed!");
                await Task.Delay((int)(0));
                SpeechSynthesis.SpeakText("Main Object function activated!");
            }
            TriggerCheckFoV();
        }
        OnCheckFieldOfViewButtonClick();

        //if (cameraFieldOfView.lefttriggerButtonDown && !prevTriggerButtonState_ && !isSpoken)
        //if (cameraFieldOfView.lefttriggerButtonDown && !prevTriggerButtonState_)
        //{
        //    if (buttonCheckFieldOfView.beginnerMode)
        //    {
        //        SpeechSynthesis.SpeakText("Left trigger button pressed!");
        //        SpeechSynthesis.SpeakText("Raycast distance function activated!");
        //        //isSpoken = true;
        //    }
        //}

        //if (cameraFieldOfView.lefttriggerButtonDown)
        //{
        //    timer += Time.deltaTime;

        //    // Calculate the direction from the controller
        //    Vector3 controllerDirection = controller.forward;

        //    RaycastHit hit;
        //    bool hasHit = Physics.Raycast(controller.position, controllerDirection, out hit, maxDistance);

        //    float normalizedDistance = hit.distance / maxDistance;
        //    float timeInterval = Mathf.Lerp(0.2f, maxInterval, normalizedDistance);

        //    // Check if the timer has reached the interval
        //    if (timer >= timeInterval)
        //    {
        //        // Reset the timer
        //        timer = 0f;

        //        // Play the sound
        //        audioSource.Play();
        //    }

        //}
        prevTriggerButtonState_ = cameraFieldOfView.triggerButtonDown;
        prevPrimaryButtonState_ = cameraFieldOfView.leftprimaryButtonDown;
    }

    public async void TriggerCheckFoV()
    {
        await Task.Delay((int)(0));
        IsCheckFoV = true;
    }

    public void OnCheckFieldOfViewButtonClick()
    {
        if (IsCheckFoV)
        {
            // If there is a problem with the main object function, enable CameraController.cs
            // Disable camera movement while the button is being clicked
            // cameraController.DisableCameraMovement();

            // Check objects in the camera's field of view
            cameraFieldOfView.CheckObjectsInFieldOfView();

            // Re-enable camera movement after the button click is handled
            // cameraController.EnableCameraMovement();

            IsCheckFoV = false;
        }
    }
}
