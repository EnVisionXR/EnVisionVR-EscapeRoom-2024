using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Logger
{
    private string logfile_;

    public Logger(string filename)
    {
        logfile_ = Application.persistentDataPath + "/" + filename;        
        Debug.Log("Writing log to: " + logfile_);
        
    }

    public void WriteLine(string line)
    {
        OutputLine(line);
    }

    private async Task OutputLine(string line)
    {
        using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(logfile_, true))
        {
            await outputFile.WriteLineAsync(line);
        }
    }
}
