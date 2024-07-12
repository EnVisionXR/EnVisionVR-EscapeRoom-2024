using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnVisionManager : MonoBehaviour
{
    public GameObject dialogPrefab;

    private Logger logger_;
    private int participantId_ = -1;
    private int anchorId_ = -1;
    private string condition_ = "UNSET";


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        string logFilename = "envision_" + System.DateTime.Now.ToString("yyyy_MMM_dd__hh_mm_ss") + ".log";
        logger_ = new Logger(logFilename);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowOnStartDialog());
    }

    // Update is called once per frame
    void Update()
    {        
 
    }

    private void ShowDialog(string text, bool editable, Action<string> onSubmitAction)
    {
        GameObject dialog = GameObject.Instantiate(dialogPrefab, Vector3.zero, Quaternion.identity, transform);
        Transform cameraT = Camera.main.transform;
        Vector3 cameraPos = cameraT.position;
        float cameraYaw = cameraT.rotation.eulerAngles.y;
        dialog.transform.position = cameraPos;
        dialog.transform.Rotate(Vector3.up, cameraYaw);
        dialog.transform.Translate(0, 0, 0.5f);

        DialogManager dialogManager = dialog.GetComponent<DialogManager>();
        dialogManager.SetDialogText(text);
        dialogManager.SetEditable(editable);
        dialogManager.SetInputText("");
        dialogManager.submitDialog = onSubmitAction;
    }


    private void SetParticipantId(string text)
    {
        int id = int.Parse(text);
        participantId_ = id;
        Debug.Log("Participante ID: " + id);
                
        LogEvent("participant_id," + id, true);

        StartCoroutine(ShowAnchorDialog());
    }

    private void SetAnchoId(string text)
    {
        int id = int.Parse(text);
        anchorId_ = id;
        Debug.Log("Anchor ID: " + id);

        LogEvent("anchor_id," + id, true);


        string[,] conditionArray = new string[12, 2] {  { "NVR", "EVR" },
                                                        { "EVR", "NVR" },
                                                        { "EVR", "NVR" },
                                                        { "NVR", "EVR" },
                                                        { "NVR", "EVR" },
                                                        { "EVR", "NVR" },
                                                        { "EVR", "NVR" },
                                                        { "NVR", "EVR" },
                                                        { "NVR", "EVR" },
                                                        { "EVR", "NVR" },
                                                        { "EVR", "NVR" },
                                                        { "NVR", "EVR" } };

        condition_ = conditionArray[participantId_ - 1, anchorId_ - 1];

        LogEvent("condition," + condition_, true);

        StartCoroutine(LoadEscapeRoomSceneAsync());
    }

    private IEnumerator ShowOnStartDialog()
    {
        yield return new WaitForSeconds(3.0f);

        ShowDialog("Participant ID:", true, SetParticipantId);
    }

    private IEnumerator ShowAnchorDialog()
    {
        yield return new WaitForSeconds(1.0f);

        ShowDialog("Anchor ID:", true, SetAnchoId);
    }

    private void SetAnchorPosition(int anchorId)
    {
        Transform cameraOffet = GameObject.Find("XR Rig/Camera Offset").transform;
        float yOffset = 0.3f; 
        
        if (anchorId == 1)
        {
            //anchorT = GameObject.Find("TeleportAnchors/Teleportation Anchor (7)").transform;
            cameraOffet.position = new Vector3(-6.689f, yOffset, 0.341f);
        }
        else if (anchorId == 2)
        {
            //anchorT = GameObject.Find("TeleportAnchors/Teleportation Anchor (4)").transform;
            cameraOffet.position = new Vector3(1.83f, yOffset, 0.918f);            
        }
    }

    private void ConfigureScene()
    {
        SetAnchorPosition(anchorId_);

        // If no-assistance condition, deactivate EnVisionVR
        if (condition_ == "NVR")
        {
            GameObject.Find("EnVisionVR").SetActive(false);
        }
        else
        {
            GameObject.Find("EnVisionVR").GetComponent<SpeechRecognition>().OnLogEventAction = LogEvent;
            GameObject.Find("EnVisionVR").GetComponent<SceneIntroduction>().OnLogEventAction = LogEvent;
        }
    }

    private IEnumerator LoadEscapeRoomSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EscapeRoom");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {            
            yield return null;
        }
                        
        LogEvent("scene_loaded", true);
        ConfigureScene();

        yield return null;
    }

    private void LogEvent(string logtext)
    {
        LogEvent(logtext, true);
    }

    private void LogEvent(string logtext, bool timestamp)
    {
        if (timestamp)
        {
            logtext += string.Format(",{0}", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        Debug.Log(logtext);
        logger_.WriteLine(logtext);
    }
}
