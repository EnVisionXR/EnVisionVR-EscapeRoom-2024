using System;
using System.Collections.Generic;
using System.Media;
using UnityEngine;
using System.Threading.Tasks;
using JetBrains.Annotations;
//This script defines the functions on the right controller
public class RightHandFunctions : MonoBehaviour
{
    SpeechSynthesis SpeechSynthesis;
    CameraFieldOfView cameraFieldOfView;
    // CameraController cameraController;
    bool IsCheckFoV = false;
    private bool prevTriggerButtonState_ = false;
    private bool prevPrimaryButtonState_ = false;
    private bool prevSecondaryButtonState_ = false;
    private bool raySweepState_ = false;
    public Transform controller;
    private List<GameObject> raySweepList = new List<GameObject>();
    private string raySweepListName;
    private GameObject prevObj;
    private GameObject recentlySpokenObject;
    private bool localizationMode = false;
    private bool readFast = true;
    private ObjectLocalization soundPlayer;
    public bool beginnerMode = false;

    void Start()
    {
        SpeechSynthesis = GetComponent<SpeechSynthesis>();
        cameraFieldOfView = GetComponent<CameraFieldOfView>();
        // cameraController = GetComponent<CameraController>();
        GameObject[] objectsInScene = GameObject.FindObjectsOfType<GameObject>();
        soundPlayer = GameObject.Find("EnVisionVR").GetComponent<ObjectLocalization>();
    }

    async void Update()
    {
        // Press Right Trigger Button to activate Raycast Object Searching Function
        //if (cameraFieldOfView.triggerButtonDown && !prevTriggerButtonState_)
        //{
        //    raySweepList = new List<GameObject>();
        //    raySweepListName = string.Empty;
        //}
        // Raycast Object Searching Function
        //if (cameraFieldOfView.triggerButtonDown)
        //{
        //    Vector3 controllerDirection = controller.forward;
        //    RaycastHit hit;
        //    bool hasHit = Physics.Raycast(controller.position, controllerDirection, out hit, 10f);
        //    if (hasHit)
        //    {
        //        GameObject hitobj = hit.transform.gameObject;
        //        if (raySweepList == null)
        //        {
        //            raySweepList.Add(hitobj);
        //            raySweepListName = raySweepListName + ", " + hitobj.name;
        //            GameObject prevObj = hitobj;
        //        }
        //        else
        //        {
        //            if (hitobj != prevObj)
        //            {
        //                raySweepList.Add(hitobj);
        //                raySweepListName = raySweepListName + ", " + hitobj.name;
        //                prevObj = hitobj;
        //            }
        //        }
        //    }
        //}

        // Press Right Primary Button to activate Beginner Mode
        //if (cameraFieldOfView.primaryButtonDown && !prevPrimaryButtonState_)
        //{
            //if (beginnerMode)
            //{
            //    SpeechSynthesis.SpeakText("Right primary button pressed!");
            //    await Task.Delay((int)(0));
            //    beginnerMode = false;
            //    SpeechSynthesis.SpeakText("Beginner Mode Deactivated!");
            //}
            //else
            //{
            //    beginnerMode = true;
            //    SpeechSynthesis.SpeakText("Beginner Mode Activated!");
            //}
        //}
        // Press Right Secondary Button to cancel beeping sound
        if (cameraFieldOfView.secondaryButtonDown && !prevSecondaryButtonState_)
        {
            //if (beginnerMode)
            //{
            //    SpeechSynthesis.SpeakText("Right secondary button pressed!");
            //    await Task.Delay((int)(0));
            //    SpeechSynthesis.SpeakText("Object Searching Function Deactivated!");
            //}
            localizationMode = false;
            soundPlayer.DeactivateBeep();
        }
        // Release Right Trigger Button to read objects swept by raycast
        //if (!cameraFieldOfView.triggerButtonDown && prevTriggerButtonState_)
        //{
        //    ReadRaySweepList(raySweepList);
        //}
        //prevTriggerButtonState_ = cameraFieldOfView.triggerButtonDown;
        //prevPrimaryButtonState_ = cameraFieldOfView.primaryButtonDown;
        //prevSecondaryButtonState_ = cameraFieldOfView.secondaryButtonDown;
    }

    public async void ReadRaySweepList(List<GameObject> raySweepList)
    {
        if (raySweepList.Count>1)
        //if (readFast)
        {
            if (beginnerMode)
            {
                SpeechSynthesis.SpeakText("Right trigger pressed!");
                await Task.Delay((int)(0));
                SpeechSynthesis.SpeakText("Ray Sweeping Function Activated!");
            }
            Debug.Log(raySweepListName);
            SpeechSynthesis.SpeakText(raySweepListName);
        }
        else
        {
            if (beginnerMode)
            {
                SpeechSynthesis.SpeakText("Right trigger pressed!");
                await Task.Delay((int)(0));
                SpeechSynthesis.SpeakText("Raycast Object Searching Function Activated!");
            }
            Vector3 controllerDirection = controller.forward;
            RaycastHit hit;
            bool hasHit = Physics.Raycast(controller.position, controllerDirection, out hit, 10f);
            GameObject hitobj = hit.transform.gameObject;
            string hitobjname = hit.transform.name;
            await Task.Delay((int)(0));
            SpeechSynthesis.SpeakText(hitobjname.Replace("_", " "));
            await Task.Delay((int)(2000));
            soundPlayer.TriggerBeep(hitobj.transform, "right");
         
        }
    }
}

