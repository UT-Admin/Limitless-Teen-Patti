using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AwsCOntroller : MonoBehaviour
{
    public static AwsCOntroller instance;

    private void Awake()
    {
        instance = this;
    }
    public void CallAwsCommand(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        UnityEngine.Debug.LogError("aws 1 "+command);
        psi.FileName = command;
        UnityEngine.Debug.LogError("aws 2");
        psi.UseShellExecute = false;
        UnityEngine.Debug.LogError("aws 3");
        psi.RedirectStandardOutput = true;
        UnityEngine.Debug.LogError("aws 4");
        psi.Arguments = "yyyyyyyyyy";
        UnityEngine.Debug.LogError("aws 5");
        Process process = new Process();
        UnityEngine.Debug.LogError("aws 6");
        process.StartInfo = psi;
        UnityEngine.Debug.LogError("aws 7");
        process.Start();
        UnityEngine.Debug.LogError("aws 8");
        process.WaitForExit();
        UnityEngine.Debug.LogError(process.StandardOutput.ReadToEnd());
    }
}
