using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine.XR.Interaction.Toolkit;
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
//using System.Diagnostics;

// Azure Speech-to-text

public class SpeechRecognition : MonoBehaviour
{
    CameraFieldOfView cameraFieldOfView;    
    SceneIntroduction SceneIntroduction;
    SpeechSynthesis speechSynthesis;
    private Camera mainCamera;
    private GameObject EnVisionVR;
    private GameObject AudioSourceObject;
    private AudioSource audioSource;
    static public bool sceneDescript;
    static public bool prevSceneDescriptState_;
    static public bool mainObjectActivate;
    static public bool prevMainObjectState_;
    static public bool searchObjectActivate;
    static public bool prevSearchObjectState;
    static public bool cancelActivate;
    static public bool prevCancelState_;
    static public string match_obj_string;
    private bool prevPrimaryButtonState_ = false;
    private bool primaryButtonDown;
    public XRController rightHand;
    public InputHelpers.Button primaryButton;
    private float doubleclickDelay = 0.5f, passedTimeSinceLastClick;
    private int unrecognizeCount = 0;
    //private bool SingleClick = false, DoubleClick = false;
    private bool previouslyPressed = false;

    // Action handled by EnVisionManager
    public Action<string> OnLogEventAction;

    public void Start()
    {
        sceneDescript = false;
        prevSceneDescriptState_ = false;
        mainObjectActivate = false;
        prevMainObjectState_ = false;
        searchObjectActivate = false;
        prevSearchObjectState = false;
        cancelActivate = true;
        prevCancelState_ = false;
        SceneIntroduction = GetComponent<SceneIntroduction>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        EnVisionVR = GameObject.Find("EnVisionVR");
        AudioSourceObject = GameObject.Find("AudioSourceObject");
        cameraFieldOfView = GetComponent<CameraFieldOfView>();
        speechSynthesis = GetComponent<SpeechSynthesis>();
        //audioSource.transform.position = mainCamera.transform.position;
    }
    private async void Update()
    {
        //audioSource.transform.position = mainCamera.transform.position;
        //audioSource.spatialBlend = 0;
        passedTimeSinceLastClick += Time.deltaTime;
        primaryButtonDown = false;//OVRInput.Get(OVRInput.Button.One);
        rightHand.inputDevice.IsPressed(primaryButton, out primaryButtonDown);
        prevSceneDescriptState_ = sceneDescript;
        prevMainObjectState_ = mainObjectActivate;
        prevSearchObjectState = searchObjectActivate;
        prevCancelState_ = cancelActivate;

        //if (Input.GetKeyDown(KeyCode.Space))
        if (primaryButtonDown && !prevPrimaryButtonState_)
        {
            if (previouslyPressed)
            {
                //pressedFirstTime = false;
                if (passedTimeSinceLastClick < doubleclickDelay)
                {
                    DoubleClick();
                    previouslyPressed = false;
                    passedTimeSinceLastClick = 0;
                }
            }
            else
            {
                previouslyPressed = true;
                passedTimeSinceLastClick = 0;
            }
        }

        if (previouslyPressed && passedTimeSinceLastClick > doubleclickDelay)
        {
            SingleClick();
            previouslyPressed = false;
        }

        //SingleClick = true;
        //DoubleClick = false;
        //passedTimeSinceLastClick = 0;
        //cancelActivate = false;
        ////UnityEngine.Debug.Log("Space key was pressed.");
        //UnityEngine.Debug.Log("Speak into your microphone.");

        //// Attach the audio source to the new game object
        //AudioClip clip = Resources.Load<AudioClip>("VA_activate");
        //audioSource.clip = clip;

        //// Play the audio clip
        //audioSource.Play();

        ////cancelActivate = true;

        //var speechConfig = SpeechConfig.FromSubscription("75c037ced5b843f2a2f2d9de0d21b55b", "uksouth");
        //speechConfig.SpeechRecognitionLanguage = "en-US";
        //using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        //SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        //SpeechToText(speechRecognizer);
        //}

        //if (DoubleClick)
        ////if (passedTimeSinceLastClick < doubleclickDelay)
        //{

        //    //pressedFirstTime = false;
        //    //passedTimeSinceLastClick = 0;
        //    UnityEngine.Debug.Log("Cancel");
        //    UnityEngine.Debug.Log("Stopped all audio sources in the scene.");
        //    cancelActivate = true;
        //    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        //    foreach (AudioSource audioSource in allAudioSources)
        //    {
        //        if (audioSource.gameObject == CameraController)
        //        {
        //            audioSource.Stop();
        //        }
        //    }
        //    AudioClip clip_cancel = Resources.Load<AudioClip>("Cancel_success");
        //    audioSource.clip = clip_cancel;
        //    // Play the audio clip
        //    audioSource.Play();
        //}




        //if (passedTimeSinceLastClick < doubleclickDelay)
        //{
        //    passedTimeSinceLastClick = 0;
        //    UnityEngine.Debug.Log("Cancel");
        //    //cancelActivate = true;
        //    //// Find all AudioSources in the scene
        //    //AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        //    //foreach (AudioSource audioSource in allAudioSources)
        //    //{
        //    //    if (audioSource.gameObject == CameraController)
        //    //    {
        //    //        audioSource.Stop();
        //    //    }
        //    //}
        //    UnityEngine.Debug.Log("Stopped all audio sources in the scene.");
        //    AudioClip clip_cancel = Resources.Load<AudioClip>("Cancel_success");
        //    audioSource.clip = clip_cancel;
        //    // Play the audio clip
        //    audioSource.Play();
        //}

        //else
        //{
        //    passedTimeSinceLastClick = 0;
        //    cancelActivate = false;
        //    //UnityEngine.Debug.Log("Space key was pressed.");
        //    UnityEngine.Debug.Log("Speak into your microphone.");

        //    // Attach the audio source to the new game object
        //    AudioClip clip = Resources.Load<AudioClip>("VA_activate");
        //    audioSource.clip = clip;

        //    // Play the audio clip
        //    audioSource.Play();

        //    //cancelActivate = true;

        //    var speechConfig = SpeechConfig.FromSubscription("75c037ced5b843f2a2f2d9de0d21b55b", "uksouth");
        //    speechConfig.SpeechRecognitionLanguage = "en-US";
        //    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        //    SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        //    SpeechToText(speechRecognizer);
        //} 

        //}
        sceneDescript = false;
        mainObjectActivate = false;
        searchObjectActivate = false;
        //cancelActivate = false;
        prevPrimaryButtonState_ = primaryButtonDown;
    }

    public void SingleClick()
    {
        OnLogEventAction?.Invoke("single_click");
        //UnityEngine.Debug.LogError("Single click detected, starting voice recognition...");
        cancelActivate = false;
        //UnityEngine.Debug.Log("Space key was pressed.");
        //UnityEngine.Debug.Log("Speak into your microphone.");

        // Attach the audio source to the new game object
        AudioClip clip = Resources.Load<AudioClip>("VA_activate");
        audioSource.spatialBlend = 0.0f;
        audioSource.clip = clip;

        // Play the audio clip
        audioSource.Play();

        //cancelActivate = true;

        var speechConfig = SpeechConfig.FromSubscription("75c037ced5b843f2a2f2d9de0d21b55b", "uksouth");
        speechConfig.SpeechRecognitionLanguage = "en-US";
        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        SpeechToText(speechRecognizer);
    }

    public void DoubleClick()
    {
        OnLogEventAction?.Invoke("double_click");
        //UnityEngine.Debug.LogError("Double click detected, cancelling audio...");
        //UnityEngine.Debug.Log("Double click detected");
        //UnityEngine.Debug.Log("Cancel");
        //UnityEngine.Debug.Log("Stopped all audio sources in the scene.");
        cancelActivate = true;
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.gameObject == EnVisionVR || audioSource.gameObject == AudioSourceObject)
            {
                audioSource.Stop();
            }
        }
        AudioClip clip_cancel = Resources.Load<AudioClip>("Cancel_success");
        audioSource.spatialBlend = 0.0f;
        audioSource.clip = clip_cancel;
        // Play the audio clip
        audioSource.Play();
    }

    public async Task SpeechToText(SpeechRecognizer speechRecognizer)
    {
        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        OutputSpeechRecognitionResult(speechRecognitionResult);
    }

//async static Task Main(string[] args)
//    {
//        UnityEngine.Debug.Log("I am here!");
        

//        await Task.Delay(300);
//        OutputSpeechRecognitionResult(speechRecognitionResult);
//    }

    public async void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                UnityEngine.Debug.Log($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                OnLogEventAction?.Invoke(string.Format("speech_rec,\"{0}\"", speechRecognitionResult.Text));
                // Attach the audio source to the new game object

                //string pattern = @"Where is the (\w+)\s+(\w+)\s+(\w+)";
                string pattern = @"Where is the (.+)$";
                //string obj_pattern = @"(\w+) object"

                string userQuery = speechRecognitionResult.Text;
                Match match_obj = Regex.Match(userQuery, pattern);
                Match match_scene = Regex.Match(userQuery, @"Where am I");
                Match match_mainobj = Regex.Match(userQuery, @"What is near me");
                Match match_cancel = Regex.Match(userQuery, @"Cancel");

                if (match_obj.Success)
                {
                    unrecognizeCount = 0;
                    //cancelActivate = true;
                    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

                    foreach (AudioSource audioSource in allAudioSources)
                    {
                        if (audioSource.gameObject == EnVisionVR)
                        {
                            audioSource.Stop();
                        }
                    }
                    AudioClip clip_stop = Resources.Load<AudioClip>("VA_stop");
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip_stop;
                    // Play the audio clip
                    audioSource.Play();
                    match_obj_string = match_obj.Groups[1].Value;
                    match_obj_string = match_obj_string.Replace("?", string.Empty);
                    UnityEngine.Debug.Log("Searching for: " + match_obj_string);
                    OnLogEventAction?.Invoke(string.Format("interaction_triggered,\"{0}\"", match_obj_string));
                    //UnityEngine.Debug.Log("match_obj: " + match_obj);
                    //UnityEngine.Debug.Log(match_obj_string + "match_obj.Groups[1].Value");
                    //UnityEngine.Debug.Log("match_obj.Groups[2].Value:" + match_obj.Groups[2].Value);
                    searchObjectActivate = true;
                    break;
                }

                if (match_scene.Success)
                {
                    unrecognizeCount = 0;
                    cancelActivate = true;
                    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

                    foreach (AudioSource audioSource in allAudioSources)
                    {
                        if (audioSource.gameObject == EnVisionVR)
                        {
                            audioSource.Stop();
                        }
                    }
                    AudioClip clip_stop = Resources.Load<AudioClip>("VA_stop");
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip_stop;
                    // Play the audio clip
                    audioSource.Play();
                    UnityEngine.Debug.Log("Activate Scene Description");
                    OnLogEventAction?.Invoke("scene_description_triggered");
                    sceneDescript = true;
                    break;
                }

                if (match_mainobj.Success)
                //if (userQuery.IndexOf("object", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    unrecognizeCount = 0;
                    cancelActivate = false;
                    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

                    foreach (AudioSource audioSource in allAudioSources)
                    {
                        if (audioSource.gameObject == EnVisionVR)
                        {
                            audioSource.Stop();
                        }
                    }
                    AudioClip clip_stop = Resources.Load<AudioClip>("VA_stop");
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip_stop;
                    // Play the audio clip
                    audioSource.Play();
                    UnityEngine.Debug.Log("Activate FoV Main Object");
                    OnLogEventAction?.Invoke("object_localization_triggered");
                    mainObjectActivate = true;
                    break;
                }

                //if(match_cancel.Success)
                if (userQuery.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    unrecognizeCount = 0;
                    AudioClip clip_stop = Resources.Load<AudioClip>("VA_stop");
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip_stop;
                    // Play the audio clip
                    audioSource.Play();
                    await Task.Delay((int)(500));
                    UnityEngine.Debug.Log("Cancel");
                    cancelActivate = true;
                    // Find all AudioSources in the scene
                    AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

                    foreach (AudioSource audioSource in allAudioSources)
                    {
                        if (audioSource.gameObject == EnVisionVR)
                        {
                            audioSource.Stop();
                        }
                    }
                    UnityEngine.Debug.Log("Stopped all audio sources in the scene.");
                    OnLogEventAction?.Invoke("cancel_triggered");
                    AudioClip clip_cancel = Resources.Load<AudioClip>("Cancel_success");
                    audioSource.spatialBlend = 0.0f;
                    audioSource.clip = clip_cancel;
                    // Play the audio clip
                    audioSource.Play();
                    break;
                }

                unrecognizeCount += 1;

                // Attach the audio source to the new game object
                AudioClip clip_fail = Resources.Load<AudioClip>("VA_fail");
                audioSource.spatialBlend = 0.0f;
                audioSource.clip = clip_fail;
                // Play the audio clip
                audioSource.Play();

                if (unrecognizeCount > 2)
                {
                    speechSynthesis.SpeakText("Sorry, I didn't get that. Please try saying 'Where am I', 'What is near me', or 'Where is the <objectname>'.");
                    UnityEngine.Debug.Log("Sorry, I didn't get that. Please try saying 'Where am I', 'What is near me', or 'Where is the <objectname>'.");
                    OnLogEventAction?.Invoke("unrecognized");
                }
                break;
            case ResultReason.NoMatch:
                // Attach the audio source to the new game object
                AudioClip clip_fail2 = Resources.Load<AudioClip>("VA_fail");
                audioSource.spatialBlend = 0.0f;
                audioSource.clip = clip_fail2;
                // Play the audio clip
                audioSource.Play();

                UnityEngine.Debug.Log($"NOMATCH: Speech could not be recognized.");
                OnLogEventAction?.Invoke("no_match");
                speechSynthesis.SpeakText("Sorry, I didn't recognize your voice. Please try again.");
                cancelActivate = false;
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                UnityEngine.Debug.LogError($"CANCELED: Reason={cancellation.Reason}");
                OnLogEventAction?.Invoke(string.Format("interaction_triggered,\"{0}\"", cancellation.Reason));
                speechSynthesis.SpeakText("Sorry, speech recognition was cancelled. Please try again.");
                if (cancellation.Reason == CancellationReason.Error)
                {
                    UnityEngine.Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    UnityEngine.Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    UnityEngine.Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
        // sceneDescript = false;
        // prevSceneDescriptState_ = sceneDescript;
    }
}