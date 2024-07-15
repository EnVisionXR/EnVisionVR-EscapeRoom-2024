using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyUIManager : MonoBehaviour
{
    private TMPro.TMP_Text displayText;

    // Start is called before the first frame update
    void Start()
    {
        displayText = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDisplayText(string text)
    {
        displayText.text = text;
    }
}
