using UnityEngine;

// Spatial sound to indicate object location (For Version 1)

public class SpatialSoundController : MonoBehaviour
{
    SpeechRecognition SpeechRecognition;
    private AudioSource audioSource;
    public float volume = 1f;
    public float spatialBlend = 1f;

    private void Start()
    {
        SpeechRecognition = GetComponent<SpeechRecognition>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Set the audio source properties
        audioSource.spatialize = true;
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = volume;
    }

    public void PlayAudioClip(string audioClipName, Vector3 position)
    {
        // Load the audio clip from the specified file path
        // string audioFilePath = "Sweet Notification"; // Update with the actual audio clip file name
        string audioFilePath = audioClipName;
        AudioClip audioClip = Resources.Load<AudioClip>(audioFilePath);

        if (audioClip != null)
        {
            // Create a new game object for the audio source at the specified position
            GameObject audioSourceObject = new GameObject("AudioSourceObject");
            audioSourceObject.transform.position = position;
            Debug.Log("Audio Source Position:" + position);

            // Attach the audio source to the new game object
            AudioSource audioSourceInstance = audioSourceObject.AddComponent<AudioSource>();
            audioSourceInstance.clip = audioClip;

            // Set the audio source properties
            audioSourceInstance.spatialize = true;
            audioSourceInstance.spatialBlend = spatialBlend;
            audioSourceInstance.volume = volume;

            // Play the audio clip
            audioSourceInstance.Play();

            if (SpeechRecognition.cancelActivate)
            {
                // Stop the audio clip
                audioSourceInstance.Stop();
            }
            
            // Destroy the game object after the audio clip has finished playing
            Destroy(audioSourceObject, audioClip.length);
        }
        else
        {
            Debug.LogWarning("Audio clip not found: " + audioFilePath);
        }
    }
}
