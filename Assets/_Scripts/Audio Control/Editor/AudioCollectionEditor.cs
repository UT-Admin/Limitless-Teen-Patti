using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[ExecuteInEditMode]
[CustomEditor(typeof(AudioCollection))]
public class AudioCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        return;
        
        AudioCollection audio = (AudioCollection)target;

        List<AudioDate> audioData = audio.audioData;

        using (new GUILayout.HorizontalScope())
        {
           
;           GUILayout.Label("Play", EditorStyles.boldLabel, GUILayout.MinWidth(20), GUILayout.MaxWidth(30));
            
            GUILayout.Space(10);

            GUILayout.Label("Mute", EditorStyles.boldLabel, GUILayout.MinWidth(20), GUILayout.MaxWidth(35));

            GUILayout.Space(15);

            GUILayout.Label("AudioType", EditorStyles.boldLabel,GUILayout.ExpandWidth(false), GUILayout.MinWidth(20), GUILayout.MaxWidth(150));

            GUILayout.Label("Volume", EditorStyles.boldLabel, GUILayout.ExpandWidth(false), GUILayout.MinWidth(50), GUILayout.MaxWidth(200));

            GUILayout.Label("AudioClip", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

            GUI.enabled = true;
        }

        for (int i = 0; i < audioData.Count; i++)
        {
            AudioDate currentData = audioData[i];
            GUILayout.Space(15);

            using (new GUILayout.HorizontalScope())
            {
                if (currentData.audioClip == null)
                    GUI.enabled = false;

                if (GUILayout.Button("Play", GUILayout.ExpandWidth(false)))
                {
                    StopAllClips();
                    PlayClip(currentData.audioClip);
                }

                GUILayout.Space(10);

                GUI.enabled = true;

                currentData.mute = EditorGUILayout.Toggle(currentData.mute, GUILayout.Width(20));

                currentData.audioName = (AudioEnum)EditorGUILayout.EnumPopup(currentData.audioName, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20), GUILayout.MaxWidth(150));

                if (currentData.audioClip == null)
                    GUI.enabled = false;

                currentData.volume = GUILayout.HorizontalSlider(currentData.volume, 0, 1, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20), GUILayout.MaxWidth(150));
                currentData.volume = float.Parse(GUILayout.TextField(currentData.volume.ToString("F2"), GUILayout.Width(25)));
                currentData.volume = Mathf.Clamp(currentData.volume, 0, 1);

                GUI.enabled = true;

                currentData.audioClip = EditorGUILayout.ObjectField(currentData.audioClip, typeof(AudioClip), false) as AudioClip;
            }

            var results = audioData.FindAll(s => s.audioName == currentData.audioName);

            if (results.Count > 1)
            {
                GUIStyle s = new GUIStyle(EditorStyles.textField);
                s.normal.textColor = Color.red;
                GUILayout.Label("Assigned More than once", s);
            }

            audioData[i] = currentData;
        }

        GUILayout.Space(15);

        using(new GUILayout.HorizontalScope())
        {
            int iButtonWidth = 35;
            GUILayout.Space(Screen.width / 1.8f - iButtonWidth / 2);
            if (GUILayout.Button("+", GUILayout.Width(iButtonWidth)))
            {
                AudioDate data = new AudioDate();
                audio.audioData.Add(data);
            }
            if (GUILayout.Button("-", GUILayout.Width(iButtonWidth)))
            {

                audio.audioData.RemoveAt(audio.audioData.Count - 1);
            }

        }
        if (GUILayout.Button("Stop All Audio", GUILayout.MinWidth(300), GUILayout.MaxWidth(500)))
        {
            StopAllClips();           
        }
    }

    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );
        method.Invoke(
            null,
            new object[] { clip, startSample, loop }
        );
    }

    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { },
            null
        );
        method.Invoke(
            null,
            new object[] { }
        );
    }
}
