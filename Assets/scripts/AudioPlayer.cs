using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;


public class AudioPlayer : MonoBehaviour
{
    public string audioFilePath = "C:/Users/junlo/AppData/LocalLow/DefaultCompany/VR Escape Room/temp_speech.wav"; // Full file path

    private AudioSource audioSource;

    private void Start()
    {
        // Ensure the GameObject has an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Check if the audio file exists at the specified path
        if (File.Exists(audioFilePath))
        {
            StartCoroutine(LoadAudioFile());
        }
        else
        {
            Debug.LogError("Audio file not found at: " + audioFilePath);
        }
    }

    private IEnumerator LoadAudioFile()
    {
        // Create a UnityWebRequest to load the audio file
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioFilePath, AudioType.WAV);

        // Wait for the request to complete
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio clip.");
            }
        }
    }
}
