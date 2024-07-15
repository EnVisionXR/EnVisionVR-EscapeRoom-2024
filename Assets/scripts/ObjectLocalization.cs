using System.Media;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System.Security.AccessControl;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;

// Beeping sound and voice instructions for localization

public class ObjectLocalization : MonoBehaviour
{
    private GameObject EnVisionVR;
    private bool isFound = false;
    private bool previousFound = false;
    private GameObject CameraController;
    SpeechSynthesis SpeechSynthesis;
    SceneIntroduction SceneIntroduction;
    private Transform virtualObjectTransform;
    public Transform leftcontroller;
    public Transform rightcontroller;
    private Transform controller;
    public string hand = "right";
    public float maxDistance = 10f;
    public float maxInterval = 3f;
    private AudioSource audioSource;
    private float timer = 0f; // Timer to track the elapsed time
    private bool isTriggered = false;
    public InputHelpers.Button secondaryButton;
    public bool secondaryButtonDown;
    public XRController rightHand;
    private CameraFieldOfView camerafieldofview;
    private float directionTimer = 0f;
    public GameObject foundObject = null;
    //public GameObject queryObject = null;

    // Action handled by EnVisionManager
    public Action<string> OnLogEventAction;

    private void Start()
    {
        EnVisionVR = GameObject.Find("EnVisionVR");
        SpeechSynthesis = GetComponent<SpeechSynthesis>();
        SceneIntroduction = GetComponent<SceneIntroduction>();
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
        audioSource.volume = 0.2f;

        camerafieldofview = GameObject.Find("EnVisionVR").GetComponent<CameraFieldOfView>();
    }

    public async void Update()
    {
        if (SpeechRecognition.cancelActivate)
        // if (SpeechRecognition.prevCancelState_)

        {
            SceneIntroduction.CancelSpeech();
        }
        if (SpeechRecognition.prevSearchObjectState)
        {
            List<GameObject> foundObjects = new List<GameObject>();
            //sceneIntroduction.isSpeaking_ = true;
            foreach (GameObject queryObject in FindObjectsOfType<GameObject>())
            {
                if (CalculateDistance(queryObject) < 7f)
                {
                    foundObjects.Add(queryObject);
                }
                //float stringDistance1 = 100;
                //float stringDistance2 = 100;
                //float stringDistance3 = 100;
                //if (CalculateLevenshteinDistance(queryObject.name, SpeechRecognition.match_obj_string) < Mathf.Max(stringDistance1, stringDistance2, stringDistance3))
                //{
                //    Debug.Log("Found:" + queryObject.name);
                //    foundObject = queryObject;
                //    foundObjects.Add(queryObject);
                //}
                //if (queryObject.name.IndexOf(SpeechRecognition.match_obj_string, StringComparison.OrdinalIgnoreCase) >= 0)
                //{
                //    Debug.Log("Found:" + queryObject.name);
                //    foundObject = queryObject;
                //    foundObjects.Add(queryObject);
                //    //break;
                //}
            }



            if (foundObjects != null)
            {
                foundObjects.Sort((a, b) => CalculateLevenshteinDistance(b.name, SpeechRecognition.match_obj_string).CompareTo(CalculateLevenshteinDistance(a.name, SpeechRecognition.match_obj_string)));
                
                //List<GameObject> relevantObjects = foundObjects.GetRange(0, 5);
                //relevantObjects.Sort((a, b) => CalculateDistance(a).CompareTo(CalculateDistance(b)));
                
                string foundObjectName = foundObjects[0].name.Replace("_", " ");
                foundObjectName = foundObjectName.Replace(" tag", "");
                Debug.Log("Found " + foundObjectName);
                SpeechSynthesis.SpeakText("Found " + foundObjects[0].name.Replace("_", " "));
                //Debug.LogError("Found " + foundObjects[0].name.Replace("_", " "));
                OnLogEventAction?.Invoke(string.Format("object_found,{0}", foundObjects[0].name));
                Transform foundObjectTransform = foundObjects[0].transform;
                TriggerBeep(foundObjectTransform, "right");
            }
            else
            {
                Debug.Log(SpeechRecognition.match_obj_string + "not found!");
            }
        }

        secondaryButtonDown = false;
        rightHand.inputDevice.IsPressed(secondaryButton, out secondaryButtonDown);
        if (isTriggered)
        {
            timer += Time.deltaTime;
            directionTimer += Time.deltaTime;

            if (hand == "left")
                controller = leftcontroller;
            else
            {
                controller = rightcontroller;
            }

            // Calculate the direction from the controller
            Vector3 controllerDirection = controller.forward;

            RaycastHit hit;
            bool hasHit = Physics.Raycast(controller.position, controllerDirection, out hit, maxDistance);
            float distanceToObject = (controller.position - virtualObjectTransform.position).magnitude;
            //Debug.Log("Distance to Object:" + distanceToObject);
            // Natural Language Instructions to Find Virtual Object
            if (directionTimer >= 4f)
            {
                // Reset the timer
                directionTimer = 0f;
                
                float deltaX = virtualObjectTransform.position.x - controller.position.x;
                float deltaY = virtualObjectTransform.position.y - controller.position.y;
                float deltaZ = virtualObjectTransform.position.z - controller.position.z;


                // Get the main camera's rotation quaternion
                Quaternion cameraRotation = camerafieldofview.mainCamera.transform.rotation;

                // Convert the quaternion to a rotation matrix
                Matrix4x4 rotationMatrix = Matrix4x4.Rotate(cameraRotation);

                // Extract the right, up, and forward vectors from the rotation matrix
                Vector3 cameraRight = rotationMatrix.GetColumn(0);
                Vector3 cameraUp = rotationMatrix.GetColumn(1);
                Vector3 cameraForward = rotationMatrix.GetColumn(2);

                // Calculate the dot products of your vector with each of these extracted vectors
                //Vector3 targetDirection = (virtualObjectTransform.position - controller.position).normalized;
                Vector3 targetDirection = (virtualObjectTransform.position - controller.position);

                float dotRight = Vector3.Dot(targetDirection, cameraRight);
                float dotUp = Vector3.Dot(targetDirection, cameraUp);
                float dotForward = Vector3.Dot(targetDirection, cameraForward);

                //Debug.Log("Abs Dot Forward:" + dotForward.ToString() + " Abs Dot Right:" + dotRight.ToString() + " Abs Dot Up:" + dotUp.ToString());

                // Determine the direction based on dot product comparisons
                if (Mathf.Abs(dotForward) >= Mathf.Abs(dotRight) && Mathf.Abs(dotForward) >= Mathf.Abs(dotUp))
                {
                    if (dotForward > 0)
                    {
                        Debug.Log("Abs Dot Forward" + Mathf.Abs(dotForward).ToString());
                        if (Mathf.Abs(dotForward)>2)
                        {
                            //Debug.LogError(dotForward.ToString("0") + " meters ahead");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", (dotForward.ToString("0") + " meters ahead")));
                            SpeechSynthesis.SpeakText(dotForward.ToString("0") + " meters ahead");
                        }
                        else if (Mathf.Abs(dotForward)>0.5 && Mathf.Abs(dotForward)<2)
                        {
                            //Debug.LogError("one meter ahead");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "one meter ahead"));
                            SpeechSynthesis.SpeakText("one meter ahead");
                        }
                        else
                        {
                            //Debug.LogError("forward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "forward"));
                            SpeechSynthesis.SpeakText("forward");
                        }
                    }
                    else
                    {
                        Debug.Log("Abs Dot Backward" + Mathf.Abs(dotForward).ToString());
                        if (Mathf.Abs(dotForward) > 2)
                        {
                            //Debug.LogError(Mathf.Abs(dotForward).ToString("0") + " meters backward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", (dotForward.ToString("0") + " meters backward")));
                            SpeechSynthesis.SpeakText(Mathf.Abs(dotForward).ToString("0") + " meters backward");
                        }
                        else if (Mathf.Abs(dotForward) > 0.5 && Mathf.Abs(dotForward) < 2)
                        {
                            //Debug.LogError("one meter backward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "one meter backward"));
                            SpeechSynthesis.SpeakText("one meter backward");
                        }
                        else
                        {
                            //Debug.LogError("backward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "backward"));
                            SpeechSynthesis.SpeakText("backward");
                        }
                            
                    }
                        
                }
                else if (Mathf.Abs(dotRight) >= Mathf.Abs(dotForward) && Mathf.Abs(dotRight) >= Mathf.Abs(dotUp))
                {
                    if (dotRight > 0)
                    {
                        Debug.Log("Abs Dot Right" + Mathf.Abs(dotRight).ToString());
                        if (Mathf.Abs(dotRight) > 2)
                        {
                            //Debug.LogError(Mathf.Abs(dotRight).ToString("0") + " meters right");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", (dotForward.ToString("0") + " meters right")));
                            SpeechSynthesis.SpeakText(Mathf.Abs(dotRight).ToString("0") + " meters right");
                        }
                        else if (Mathf.Abs(dotRight) > 0.5 && Mathf.Abs(dotRight) < 2)
                        {
                            //Debug.LogError("one meter right");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "one meter right"));
                            SpeechSynthesis.SpeakText("one meter right");
                        }
                        else
                        {
                            //Debug.LogError("right");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "right"));
                            SpeechSynthesis.SpeakText("right");
                        }
                            
                    }
                    else
                    {
                        Debug.Log("Abs Dot Left" + Mathf.Abs(dotRight).ToString());
                        SpeechSynthesis.SpeakText(Mathf.Abs(dotRight).ToString("0") + " meters left");
                        if (Mathf.Abs(dotRight) > 2)
                        {
                            //Debug.LogError(Mathf.Abs(dotRight).ToString("0") + " meters left");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", (dotForward.ToString("0") + " meters left")));
                            SpeechSynthesis.SpeakText(Mathf.Abs(dotRight).ToString("0") + " meters left");
                        }
                        else if (Mathf.Abs(dotRight) > 0.5 && Mathf.Abs(dotRight) < 2)
                        {
                            //Debug.LogError("one meter left");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "one meter left"));
                            SpeechSynthesis.SpeakText("one meter left");
                        }
                        else
                        {
                            //Debug.LogError("left");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "left"));
                            SpeechSynthesis.SpeakText("left");
                        }
                            
                    }
                    //if (dotRight > 0)
                    //    SpeechSynthesis.SpeakText("right");
                    //else
                    //    SpeechSynthesis.SpeakText("left");
                }
                else
                {
                    if (dotUp > 0)
                    {
                        Debug.Log("Abs Dot Up" + Mathf.Abs(dotUp).ToString());
                        if (Mathf.Abs(dotUp) > 2)
                        {
                            //Debug.LogError(Mathf.Abs(dotUp).ToString("0") + " meters upward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", (dotForward.ToString("0") + " meters upward")));
                            SpeechSynthesis.SpeakText(Mathf.Abs(dotUp).ToString("0") + " meters upward");
                        }
                        else if (Mathf.Abs(dotUp) > 0.5 && Mathf.Abs(dotUp) < 2)
                        {
                            //Debug.LogError("one meter upward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "one meter upward"));
                            SpeechSynthesis.SpeakText("one meter upward");
                        }
                        else
                        {
                            //Debug.LogError("upward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "upward"));
                            SpeechSynthesis.SpeakText("upward");
                        }
                            
                    }
                    else
                    {
                        Debug.Log("Abs Dot Down" + Mathf.Abs(dotUp).ToString());
                        if (Mathf.Abs(dotUp) > 2)
                        {
                            //Debug.LogError(Mathf.Abs(dotUp).ToString("0") + " meters downward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", (dotForward.ToString("0") + " meters downward")));
                            SpeechSynthesis.SpeakText(Mathf.Abs(dotUp).ToString("0") + " meters downward");
                        }
                        else if (Mathf.Abs(dotUp) > 0.5 && Mathf.Abs(dotUp) < 2)
                        {
                            //Debug.LogError("one meter downward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "one meter downward"));
                            SpeechSynthesis.SpeakText("one meter downward");
                        }
                        else
                        {
                            //Debug.LogError("downward");
                            OnLogEventAction?.Invoke(string.Format("object_localization,{0}", "downward"));
                            SpeechSynthesis.SpeakText("downward");
                        }
                            
                    }
                    //if (dotUp > 0)
                    //    SpeechSynthesis.SpeakText("upward");
                    //else
                    //    SpeechSynthesis.SpeakText("downward");
                }
            }
        
            float normalizedDistance = distanceToObject / maxDistance;
            float timeInterval = Mathf.Lerp(0.2f, maxInterval, normalizedDistance);

            // Check if the timer has reached the interval
            if (timer >= timeInterval)
            {
                // Reset the timer
                timer = 0f;

                // Play the sound
                audioSource.Play();
            }

            //Debug.Log("isFound:" + isFound.ToString() + " previousFound:" + previousFound.ToString());

            if (isFound && !previousFound)
            {
                previousFound = isFound;
                rightHand.SendHapticImpulse(0.5f, 1f);
                //SpeechSynthesis.SpeakText("Grabbing " + virtualObjectTransform.name.Replace("_", " "));
                //await Task.Delay(3000);
                isFound = false;
                //previousFound = false;
                //SpeechRecognition.cancelActivate = true;
                //DeactivateBeep();
                AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource audioSource in allAudioSources)
                {
                    if (audioSource.gameObject == EnVisionVR)
                    {
                        audioSource.Stop();
                    }
                }
                //await Task.Delay(3000);
                //SpeechRecognition.cancelActivate = true;
            }

            if (distanceToObject < 0.12f && !isFound)
            {
                previousFound = isFound;
                isFound = true;
            }

            //if (distanceToObject < 0.12f && isFound)
            //{
            //    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

            //    foreach (AudioSource audioSource in allAudioSources)
            //    {
            //        if (audioSource.gameObject == CameraController)
            //        {
            //            audioSource.Stop();
            //        }
            //    }
            //    UnityEngine.Debug.Log("Stopped all audio sources in the scene.");
            //}

            if (SpeechRecognition.cancelActivate)
            {
                // Debug.Log("Secondary button pressed!");
                camerafieldofview.localizationMode = false;
                DeactivateBeep();
                //SpeechRecognition.cancelActivate = false;
            }

            //
            //previousFound = isFound;

        }
        //else
        //{
        //}
    }

    float CalculateLevenshteinDistance(string a, string b)
    {
        int[,] distance = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
        {
            for (int j = 0; j <= b.Length; j++)
            {
                if (i == 0)
                    distance[i, j] = j;
                else if (j == 0)
                    distance[i, j] = i;
                else
                    distance[i, j] = Mathf.Min(
                        distance[i - 1, j] + 1,
                        distance[i, j - 1] + 1,
                        distance[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1)
                    );
            }
        }

        int maxLength = Mathf.Max(a.Length, b.Length);
        float similarity = 1.0f - (float)distance[a.Length, b.Length] / maxLength;

        return similarity;
    }
    public float CalculateDistance(GameObject queryObject)
    {
        float distanceToObject = (queryObject.transform.position - rightcontroller.position).magnitude;
        return distanceToObject;
    }
    public void TriggerBeep(Transform ObjectToLocateTransform, string handinput)
    {
        hand = handinput;
        if (ObjectToLocateTransform == null)
        {
            Debug.LogError("Beeping Virtual Object not assigned!");
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            isTriggered = true;
            virtualObjectTransform = ObjectToLocateTransform;
        }
    }

    public void DeactivateBeep()
    {
        isTriggered= false;
    }

}
