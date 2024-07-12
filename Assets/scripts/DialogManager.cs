using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Action<string> submitDialog;

    private bool editable_ = false;
    private TMPro.TMP_Text dialogText, inputText;

    // Start is called before the first frame update
    void Awake()
    {
        dialogText = transform.Find("DialogText").GetComponent<TMPro.TMP_Text>();
        inputText = transform.Find("InputText").GetComponent<TMPro.TMP_Text>();
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (!editable_)
        {
            return;
        }

        string curInputText = inputText.text;
        string newInputText = curInputText;

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            newInputText += "0";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            newInputText += "1";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            newInputText += "2";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            newInputText += "3";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            newInputText += "4";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            newInputText += "5";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            newInputText += "6";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            newInputText += "7";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            newInputText += "8";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            newInputText += "9";
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (newInputText.Length > 0)
            {
                newInputText = newInputText.Remove(newInputText.Length - 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (newInputText.Length > 0)
            {
                submitDialog.Invoke(newInputText);
                Destroy(gameObject, 0.5f);
            }
        }

        if (curInputText.Length != newInputText.Length)
        {
            inputText.text = newInputText;
        }
    }

    public void SetDialogText(string text)
    {
        dialogText.text = text;
    }

    public void SetEditable(bool editable)
    {
        editable_ = editable;
    }

    public void SetInputText(string text)
    {
        inputText.text = text;
    }


}
