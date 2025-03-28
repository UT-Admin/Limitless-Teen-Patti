﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TP;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class KeyValuePojo
{
    public string keyId;
    public string value;

    public KeyValuePojo() { }

    public KeyValuePojo(string keyId, string value)
    {
        this.keyId = keyId;
        this.value = value;
    }
}

public enum NetworkCallType
{
    GET_METHOD,
    POST_METHOD_USING_JSONDATA,
    POST_METHOD_USING_FORMDATA
}

public class WebApiManager : MonoBehaviour
{
    public delegate void ReqCallback(bool isSuccess, string error, string body);
    public delegate void ReqCallbackTex(bool isSuccess, string error, Texture2D imageTex);
    private const int timeOut = 20;

    public static WebApiManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void GetJsonNetWorkCall(string uri, string bodyJsonString, ReqCallback callback, int timeout = timeOut)
    {
        GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, uri, bodyJsonString, null, callback, timeout);
    }

    public void GetNetWorkCall(NetworkCallType callType, string uri, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
    {
        string bodyJsonString = string.Empty;
        Debug.Log($"<color=aqua>GetNetworkCall called</color>");

        if (callType == NetworkCallType.POST_METHOD_USING_JSONDATA)
            bodyJsonString = getEncodedParams(parameters);

        GetNetWorkCall(callType, uri, bodyJsonString, parameters, callback, timeout);
    }
    /*************  ✨ Codeium Command ⭐  *************/
    /// <summary>
    /// Downloads an image from a given URI and calls the callback method with the Texture2D of the image.
    /// </summary>
    /// <param name="uri">The URI of the image to download.</param>
    /// <param name="callback">The method to call with the Texture2D of the image.</param>
    /// <param name="timeout">The timeout for the download in seconds. Defaults to 20.</param>
    /******  1d464ed6-c748-4b09-9f4a-ce2e9f899968  *******/
    public void GetDownloadImage(string uri, ReqCallbackTex callback, int timeout = timeOut)
    {
        StartCoroutine(DownloadImage(uri, callback, timeout));
    }

    private void GetNetWorkCall(NetworkCallType callType, string uri, string bodyJsonString, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
    {
        Debug.Log("Check And Test The GetNetWorkCall");
        Debug.Log($"<color=aqua>GetNetworkCall called with call type {callType} url {uri}</color>");
        switch (callType)
        {
            case NetworkCallType.GET_METHOD:
                StartCoroutine(RequestGetMethod(uri, parameters, callback, timeout));
                break;
            case NetworkCallType.POST_METHOD_USING_FORMDATA:
                StartCoroutine(PostRequestUsingForm(uri, parameters, callback, timeout));
                break;
            case NetworkCallType.POST_METHOD_USING_JSONDATA:
                StartCoroutine(PostRequestUsingJson(uri, parameters, callback, timeout));
                break;
        }
    }

    public IEnumerator RequestWebMethod(string url, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
    {
        string getParameters = getEncodedParams(parameters);

        using (UnityWebRequest www = UnityWebRequest.Get(url + getParameters))
        {
            www.timeout = timeout;
            //Send request
            yield return www.SendWebRequest();

            while (!www.isDone)
                yield return www;

            while (!www.downloadHandler.isDone)
                yield return null;

            //Return result
            callback(www.result == UnityWebRequest.Result.Success, www.error, www.downloadHandler.text);
        }
    }

    private IEnumerator RequestGetMethod(string url, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
    {
        yield return null;
        if (!parameters.Exists(x => x.keyId == "DateTime"))
            parameters.Add(new KeyValuePojo { keyId = "DateTime", value = "Date___" + DateTime.UtcNow });
        string getParameters = getEncodedParams(parameters);
        using (UnityWebRequest www = UnityWebRequest.Get(url + getParameters))
        {
            www.timeout = timeout;
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            while (!www.isDone)
                yield return www;
            //while (!www.downloadHandler.isDone)
            //    yield return null;
            DebugHelper.Log("Check $$$$$" + www.error);
            DebugHelper.Log("WWW check" + www.result);
            //Return result
            callback(www.result == UnityWebRequest.Result.Success, www.error, www.downloadHandler.text);
            yield break;
        }
    }


    private IEnumerator PostRequestUsingForm(string url, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
    {
        if (!parameters.Exists(x => x.keyId == "DateTime"))
            parameters.Add(new KeyValuePojo { keyId = "DateTime", value = "Date___" + DateTime.UtcNow });
        WWWForm bodyFormData = new WWWForm();
        foreach (KeyValuePojo items in parameters)
        {
            bodyFormData.AddField(items.keyId, items.value);
            DebugHelper.Log(items.keyId + "::" + items.value);
        }

        using (UnityWebRequest www = UnityWebRequest.Post(url, bodyFormData))
        {
            www.timeout = timeout;
            yield return www.SendWebRequest();
            //                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            while (!www.isDone)
                yield return www;

            //while (!www.downloadHandler.isDone)
            //   yield return null;

            callback(www.result == UnityWebRequest.Result.Success, www.error, www.downloadHandler.text);
        }
    }
    private IEnumerator PostRequestUsingJson(string url, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = StaticStrings.timeOut, bool check = false)
    {
        if (!parameters.Exists(x => x.keyId == "DateTime"))
            parameters.Add(new KeyValuePojo { keyId = "DateTime", value = "Date___" + DateTime.UtcNow });
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        foreach (KeyValuePojo kvp in parameters)
        {
            keyValuePairs.Add(kvp.keyId, kvp.value);
        }
        string jsonData = JsonConvert.SerializeObject(keyValuePairs);
        if (!check)
        {
            DebugHelper.Log($"<color=yellow>{jsonData}\n{url}</color>");
        }
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = timeout;

            yield return request.SendWebRequest();

            callback(request.result == UnityWebRequest.Result.Success, request.error, request.downloadHandler.text);

        } // Automatically disposes of the request here

    }

    private IEnumerator DownloadImage(string url, ReqCallbackTex callback, int timeout = timeOut)
    {

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))

        {

            www.timeout = timeout;
            yield return www.SendWebRequest();

            while (!www.isDone)
                yield return www;
            while (!www.downloadHandler.isDone)
                yield return null;
            callback(www.result == UnityWebRequest.Result.Success, www.error, ((DownloadHandlerTexture)www.downloadHandler).texture);

        }
    }

    public string getEncodedParams(List<KeyValuePojo> parameters)
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePojo items in parameters)
        {
            string value = UnityWebRequest.EscapeURL(items.value);

            if (sb.Length > 0)
            {
                sb.Append("&");
            }
            sb.Append(items.keyId + "=" + value);
        }
        if (sb.Length > 0)
        {
            sb.Insert(0, "?");
        }
        return sb.ToString();
    }

    public bool HasInternet()
    {
        NetworkReachability reachability = Application.internetReachability;

        switch (reachability)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                return true;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                return true;
        }

        return false;
    }

    public string getJsonParams(List<KeyValuePojo> parameters)
    {
        var entries = parameters.Select(d =>
        string.Format("\"{0}\": \"{1}\",", d.keyId, d.value));
        return "{" + entries.ToString().Remove(entries.ToString().Length - 1) + "}";
    }
}