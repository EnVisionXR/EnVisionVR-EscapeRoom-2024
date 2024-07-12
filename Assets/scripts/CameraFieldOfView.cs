using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Threading;
//using UnityEditor.PackageManager;

// Initialize Azure TTS and Implement FoV Main Object Function

public class CameraFieldOfView : MonoBehaviour
{
    SpeechSynthesis SpeechSynthesis;
    SceneIntroduction SceneIntroduction;
    public Camera mainCamera;
    RightHandFunctions buttonCheckFieldOfView;
    public Dictionary<string, float> importanceValues;
    public Dictionary<string, float> newImportanceValues;
    public List<GameObject> readObjects;
    Dictionary<string, string> descriptions;
    SpeechSynthesizer synthesizer;
    private AudioSource audioSource;
    public bool localizationMode = false;
    public float volume = 1f;
    public float spatialBlend = 1f;
    private GameObject hitobj1;
    private GameObject hitobj2;
    public SpatialSoundController soundController;
    private ObjectLocalization soundPlayer;
    // Add a private variable to store the most recently spoken object
    public GameObject recentlySpokenObject;

    public XRController rightHand;
    public XRController leftHand;
    public InputHelpers.Button primaryButton;
    public InputHelpers.Button secondaryButton;
    public InputHelpers.Button triggerButton;
    public bool leftprimaryButtonDown;
    public bool leftsecondaryButtonDown;
    public bool lefttriggerButtonDown;
    public bool primaryButtonDown;
    public bool secondaryButtonDown;
    public bool triggerButtonDown;
    private float timer = 0f; // Timer to track the elapsed time
    public Thread synthesisThread;

    void Start()
    {
        mainCamera = Camera.main;
        LoadImportanceValues();
        readObjects = new List<GameObject>();
        // Create SpeechConfig instance
        //SpeechConfig speechConfig = SpeechConfig.FromSubscription("2042afb7c5bc4fe3bbe81e0b0af4833d", "uksouth");
        SpeechSynthesis = GetComponent<SpeechSynthesis>();

        // Create SpeechSynthesizer instance
        //synthesizer = new SpeechSynthesizer(speechConfig);

        // Get the AudioSource component attached to the object
        audioSource = GetComponent<AudioSource>();
        soundController = GetComponent<SpatialSoundController>();
        SceneIntroduction = GetComponent<SceneIntroduction>();
        //SpeechRecognition = GetComponent<SpeechRecognition>();
        
        soundPlayer = GameObject.Find("EnVisionVR").GetComponent<ObjectLocalization>();
        buttonCheckFieldOfView = GetComponent<RightHandFunctions>();
    }

    async void Update()
    {
        // Check if the button on the right hand controller is pressed
        primaryButtonDown = false;//OVRInput.Get(OVRInput.Button.One);
        rightHand.inputDevice.IsPressed(primaryButton, out primaryButtonDown);
        
        secondaryButtonDown = false;
        rightHand.inputDevice.IsPressed(secondaryButton, out secondaryButtonDown);
        
        triggerButtonDown = false;
        rightHand.inputDevice.IsPressed(triggerButton, out triggerButtonDown);
        
        leftprimaryButtonDown = false;
        leftHand.inputDevice.IsPressed(primaryButton, out leftprimaryButtonDown);
        
        leftsecondaryButtonDown = false;
        leftHand.inputDevice.IsPressed(secondaryButton, out leftsecondaryButtonDown);
        
        lefttriggerButtonDown = false;
        leftHand.inputDevice.IsPressed(triggerButton, out lefttriggerButtonDown);
    }


    public async void CheckObjectsInFieldOfView()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not assigned!");
            return;
        }

        GameObject[] objectsInScene = GameObject.FindObjectsOfType<GameObject>();

        List<GameObject> objectsInFieldOfView = new List<GameObject>();

        //foreach (GameObject obj in objectsInScene)
        //{
        //    Debug.Log("Object: " + obj.name + "Transform Position:" + obj.transform.position);
        //    Debug.Log("Camera Position:" + mainCamera.transform.position);
        //}

        foreach (GameObject obj in objectsInScene)
        {
            // Get the object's transform component to retrieve its position
            //Transform objTransform = obj.transform;
            //Vector3 objPosition = objTransform.position;
            //Debug.Log("Object: " + obj.name + "Transform Position:" + objPosition);
            // Check if the object is within the camera's field of view
            bool isPrint = false;
            if (obj.GetComponent<Transform>() != null)
            {
                // Get the object's transform component to retrieve its position
                Transform objTransform = obj.transform;
                Vector3 objPosition = objTransform.position;

                //if (obj.name == "Fence8" || obj.name == "Fence9" || obj.name == "Fence10")
                //{
                //    Debug.Log("Checking if " + obj.name + "is visible...");
                //    isPrint = true;
                //}
                //if (IsObjectVisible(objPosition, mainCamera.transform.position, mainCamera.transform.rotation, mainCamera.fieldOfView))
                if (true)
                {
                    objectsInFieldOfView.Add(obj);
                }
            }
        }

        // Calculate the new importance values for each object based on the distance from the camera
        foreach (GameObject obj in objectsInFieldOfView)
        {
            float dist_threshold = 2.5f;
            //if (objectsInFieldOfView.Count < 10)
            //{
            //    dist_threshold = 20f;
            //}
            float originalImportance = GetImportanceValue(obj.name);
            float newImportance = GetNewImportanceValue(obj.name);
            float distance = Vector3.Distance(obj.transform.position, mainCamera.transform.position);
            newImportance = originalImportance * (Mathf.Exp(-distance) - Mathf.Exp(-dist_threshold));
            int objectDecay = readObjects.Where(x => x.Equals(obj)).Count();
            newImportance = (float)(newImportance * Math.Pow(0.5, objectDecay));
            if (distance > dist_threshold)
                newImportance = 0;
            //importanceValues[obj.name] = newImportance; // Update the importance value in the dictionary
            newImportanceValues[obj.name] = newImportance;
        }

        // Sort the objects based on the new importance values
        objectsInFieldOfView.Sort((a, b) => GetNewImportanceValue(b.name).CompareTo(GetNewImportanceValue(a.name)));

        // Generate list of object names in the field of view
        List<GameObject> cleanedObjectsInFieldOfView = new List<GameObject>();
        for (int i = 0; i < objectsInFieldOfView.Count; i++)
        {
            GameObject obj = objectsInFieldOfView[i];
            Debug.Log("Main Object " + i.ToString() + ": " + obj.name);
            bool nameExists = cleanedObjectsInFieldOfView.Any(x => x.name.Equals(obj.name));
            if (!nameExists)
                cleanedObjectsInFieldOfView.Add(obj);
        }

        foreach (GameObject obj in cleanedObjectsInFieldOfView)
        {
            Debug.Log("Cleaned Object: " + obj.name);
        }

        if (cleanedObjectsInFieldOfView.Count==0)
        {
            Debug.Log("No object in field of view");
            SpeechSynthesis.SpeakText("No objects found, try moving around!");
        }

        // Clean up the list of objects in the field of view
        //for (int i = objectsInFieldOfView.Count - 1; i >= 0; i--)
        //{
        //    GameObject obj = objectsInFieldOfView[i];
        //    if (GetNewImportanceValue(obj.name) == 0)
        //    {
        //        objectsInFieldOfView.RemoveAt(i);
        //    }
        //}



        // Print the three objects with the highest new importance values
        for (int i = 0; i < Mathf.Min(3, cleanedObjectsInFieldOfView.Count); i++)
        {
            GameObject obj = cleanedObjectsInFieldOfView[i];
            if (GetNewImportanceValue(obj.name) > 0 && localizationMode == false)
            {
                string message = obj.name;
                string messageFull = obj.name + " is within the field of view. Position: " + obj.transform.position + " Importance: " + GetImportanceValue(obj.name);
                Debug.LogError(messageFull);
                SpeechSynthesis.SpeakText(message.Replace("_", " "));
                if (SpeechRecognition.cancelActivate)
                {
                    // Stop the audio clip
                    //SpeechSynthesis.audioSourceNeedStop = true;
                    synthesisThread.Abort();
                }
                readObjects.Add(obj);

                // Store the most recently spoken object
                recentlySpokenObject = obj;
                importanceValues[obj.name] = (float)((float)GetImportanceValue(obj.name) * 0.2);
                //(double) GetNewImportanceValue(obj.name) * 0.5;

                // Delay before playing the audio clip
                float delay = 2.0f; // Adjust the delay time as needed
                if (buttonCheckFieldOfView.beginnerMode)
                    await Task.Delay((int)(delay * 4500));

                // Play the audio clip at the object's position
                string path = obj.name;
                Transform currentTransform = obj.transform;

                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    path = currentTransform.name + "/" + path;
                }
                
                //Debug.LogError("Position for" + obj.name + ": " + obj.transform.position);
                if (currentTransform.name == "Interactables")
                {
                    await Task.Delay((int)(2100));
                    soundController.PlayAudioClip("Positive Notification", obj.transform.position);
                }
                else if (currentTransform.name == "Interior")
                {
                    await Task.Delay((int)(2100));
                    soundController.PlayAudioClip("Magic Spell", obj.transform.position);
                }
                else if (currentTransform.name == "Potions")
                {
                    await Task.Delay((int)(2100));
                    soundController.PlayAudioClip("Magic", obj.transform.position);
                }
                else if (currentTransform.name == "Exterior")
                {
                    await Task.Delay((int)(2100));
                    soundController.PlayAudioClip("Confirm", obj.transform.position);
                }
                else
                {
                    await Task.Delay((int)(2100));
                    soundController.PlayAudioClip("Sweet Notification", obj.transform.position);
                }

                string objname = objectsInFieldOfView[i].name;
                Debug.LogError("Sound of " + objname);

                await Task.Delay((int)(800f));

                //await Task.Delay((int)(delay * 500));

                // Uncomment to play beeping sound to localize the object
                //soundPlayer.TriggerBeep(obj.transform, "right");
            }
        }

        await Task.Delay((int)(500f));
    }

    //public async void SpeakText(string text)
    //{
    //    SceneIntroduction.isSpeaking_ = true;

    //    var syn_result = await synthesizer.SpeakTextAsync(text);

    //    SceneIntroduction.isSpeaking_ = false;

    //    if (syn_result.Reason == ResultReason.SynthesizingAudioCompleted)
    //    {
    //        // Synthesis completed successfully
    //        // You can handle the audio here
    //        //await synthesizer.SpeakTextAsync(text);
    //    }
    //    else if (syn_result.Reason == ResultReason.Canceled)
    //    {
    //        Debug.Log("Speech synthesis cancelled.");
    //    }
    //    //await synthesizer.SpeakTextAsync(text);
    //}

    //public async void SpeakText(string text)
    //{
    //    if (SceneIntroduction.isSpeaking_)
    //    {
    //        Debug.LogWarning("Synthesis is already in progress.");
    //        return;
    //    }

    //    synthesisThread = new Thread(() =>
    //    {
    //        SceneIntroduction.isSpeaking_ = true;

    //        //var result = synthesizer.SpeakText(text);
    //        var result = synthesizer.SpeakTextAsync(text).Result;

    //        SceneIntroduction.isSpeaking_ = false;

    //        //if (result.Reason == ResultReason.SynthesizingAudioCompleted)
    //        //{
    //        //    // Synthesis completed successfully
    //        //    // You can handle the audio here
    //        //}
    //        //else if (result.Reason == ResultReason.Canceled)
    //        //{
    //        //    var cancellation = CancellationDetails.FromResult(result);
    //        //    if (cancellation.Reason == CancellationReason.Error)
    //        //    {
    //        //        Debug.LogError($"CANCELED: ErrorCode={cancellation.ErrorCode}");
    //        //    }
    //        //}
    //    });

    //    synthesisThread.Start();
    //}

    public bool IsObjectVisible(Vector3 objectPosition, Vector3 cameraPosition, Quaternion cameraRotation, float fieldOfView)
    {
        // Calculate the direction from the camera to the object
        Vector3 dir = objectPosition - cameraPosition;

        // Calculate the angle between the camera's forward direction and the direction to the object
        float angle = Vector3.Angle(cameraRotation * Vector3.forward, dir);

        RaycastHit hitInfo = new RaycastHit();
        //Vector3 dir = objectPosition - cameraPosition;
        //int layermask = (int)(1 << 8);

        //Vector3 controllerDirection = controller.forward;
        RaycastHit hit1;
        RaycastHit hit2;
        //Debug.Log("Camera Position: " + cameraPosition);
        //Debug.Log("Dir: " + dir); 
        bool hasHit1 = Physics.Raycast(cameraPosition, dir, out hit1, 100f);
        bool hasHit2 = Physics.Raycast(cameraPosition, cameraRotation * Vector3.forward, out hit2, 100f);
        //Debug.Log("Has Hit 1: " + hasHit1);
        //Debug.Log("Has Hit 2: " + hasHit2); 
        if (hasHit1)
        {
            hitobj1 = hit1.transform.gameObject;
            //if (isPrint)
            //    Debug.Log("Hit1: " + hitobj1.name);
            //Debug.Log("Hit1: " + hitobj1.name);
        }
        if(hasHit2)
        {
            hitobj2 = hit2.transform.gameObject;
            //if(isPrint)
            //    Debug.Log("Hit2: " + hitobj2.name);
            //Debug.Log("Hit2: " + hitobj2.name);
        }

        if (!hasHit1 && !hasHit2)
        {
            return false;
        }

        if (hasHit1 || hasHit2)
        {
            bool visible_true;
            if (hasHit1 && !hasHit2)
                visible_true = hitobj1.transform.position == objectPosition;
            else if (!hasHit1 && hasHit2)
                visible_true = hitobj2.transform.position == objectPosition;
            else
                visible_true = hitobj1.transform.position == objectPosition || hitobj2.transform.position == objectPosition;
            bool isInFoV = angle <= (fieldOfView / 2f);
            //if (isPrint)
            //    Debug.Log("In FoV: " + isInFoV + " Visible: " + visible_true);
            if (isInFoV && visible_true)
            {
                // Object is within the field of view
                return true;
            }
            
        }

        //Debug.Log(hitobj.transform.position);
        //Debug.Log(objectPosition);
        //Debug.Log(angle <= fieldOfView / 2f);
        //Debug.Log(hitobj.transform.position == objectPosition);

        // Check if the angle is within the camera's field of view


        // Object is outside the field of view
        return false;
    }

    void LoadImportanceValues()
    {
        string jsonFilePath = Path.Combine(Application.dataPath, "scene_graph_importance.json");
        //Debug.LogError("Importance Values Loaded from:" + jsonFilePath);

        if (File.Exists(jsonFilePath))
        {
            
            string jsonContent = File.ReadAllText(jsonFilePath);
            SceneGraph sceneGraphContent = JsonConvert.DeserializeObject<SceneGraph>(jsonContent);

            if (sceneGraphContent != null)
            {
                importanceValues = new Dictionary<string, float>();
                newImportanceValues = new Dictionary<string, float>();
                //descriptions = new Dictionary<string, string>(); // New dictionary for descriptions
                TraverseSceneGraph(sceneGraphContent.children);

                Debug.LogError("Importance values and descriptions loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to deserialize JSON content.");
            }
        }
        else
        {
            Debug.LogError("JSON file not found at path: " + jsonFilePath);
        }
    }

    //private IEnumerator LoadImportanceValuesAsync()
    //{

    //}

    void TraverseSceneGraph(List<SceneGraph> children)
    {
        if (children == null || children.Count == 0)
            return;

        foreach (SceneGraph child in children)
        {
            if (child.importance.HasValue)
            {
                if (!importanceValues.ContainsKey(child.name))
                {
                    importanceValues.Add(child.name, child.importance.Value);
                }
                if (!newImportanceValues.ContainsKey(child.name))
                {
                    newImportanceValues.Add(child.name, child.importance.Value);
                }
            }

            TraverseSceneGraph(child.children);
        }
    }

    public float GetImportanceValue(string objectName)
    {
        if (importanceValues.ContainsKey(objectName))
        {
            return importanceValues[objectName];
        }

        //Debug.LogWarning("Importance value not found for object: " + objectName);
        return 0f;
    }

    public float GetNewImportanceValue(string objectName)
    {
        if (newImportanceValues.ContainsKey(objectName))
        {
            return newImportanceValues[objectName];
        }

        //Debug.LogWarning("Importance value not found for object: " + objectName);
        return 0f;
    }

    [System.Serializable]
    public class SceneGraph
    {
        public string name;
        public string type;
        public List<SceneGraph> children;
        public List<ComponentData> components;
        public float? importance;
        public string? description;
    }

    [System.Serializable]
    public class ComponentData
    {
        public string type;
        // Add other component properties as needed
    }
}