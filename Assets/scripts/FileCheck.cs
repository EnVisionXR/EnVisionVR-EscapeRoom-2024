using System.IO;
using UnityEngine;


public class FileCheck : MonoBehaviour
{
    private static string savePath; // Set this to your specific save path

    void Start()
    {
        CheckFileExistence();
    }

    private void CheckFileExistence()
    {
        savePath = Application.persistentDataPath + "/temp_speech.wav";
        //savePath = Application.persistentDataPath;
        // Replace "your_audio_file.wav" with the name of your audio file
        //string audioFilePath = Path.Combine(savePath, "temp_speech.wav");

        if (File.Exists(savePath))
        {
            Debug.Log("Audio file exists at: " + savePath);
        }
        else
        {
            Debug.Log("Audio file does not exist at: " + savePath);
        }
    }
}