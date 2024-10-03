using TP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class DynamicECSRequest : MonoBehaviour
{
    string processArgs = string.Empty;
    Action<string> onComplete;
    private void RunLoadInNewThread(object a)
    {
       
        UnityEngine.Debug.Log("commend  ::  " + processArgs);
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            Arguments = " -c \"" + processArgs + " \""
        };
        Process myProcess = new Process
        {
            StartInfo = startInfo
        };
        myProcess.Start();
        string output = myProcess.StandardOutput.ReadToEnd();
        myProcess.WaitForExit();

        UnityThread.executeInUpdate(() =>
        {
            if (onComplete != null) onComplete.Invoke(output);
            UnityEngine.Debug.Log("commend  ::  " + output);
            Destroy(this.gameObject);
        });
    }
   
    public void ExecuteProcessTerminal(string _argument, Action<string> _onComplete)
    {

        processArgs = _argument;
        onComplete = _onComplete;

        ThreadPool.QueueUserWorkItem(RunLoadInNewThread);

    }
}
