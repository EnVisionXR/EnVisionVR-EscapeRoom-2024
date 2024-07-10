using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ObjectSearchController : MonoBehaviour
{
    // Reference to the Object Searching Function
    public ObjectLocalization objectSearchFunction;

    void Start()
    {
        // You should assign your Object Search Function script to this reference in the Unity Inspector.
        if (objectSearchFunction == null)
        {
            Debug.LogError("Object Search Function reference is not assigned.");
        }
    }

    //public void ProcessUserQuery(string userQuery)
    //{
    //    // Use a regular expression to extract the object name from the user query.
    //    // You may need to customize the regular expression to suit your specific use case.
    //    string objectName = ExtractObjectName(userQuery);

    //    if (objectName != null)
    //    {
    //        // Call the Object Search Function to search for the object with the extracted name.
    //        objectSearchFunction.SearchForObject(objectName);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No object name found in the user query.");
    //    }
    //}

    private string ExtractObjectName(string userQuery)
    {
        // Regular expression pattern to match an object name (e.g., "Find object X").
        string pattern = @"Find (\w+)";
        Match match = Regex.Match(userQuery, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }
}
