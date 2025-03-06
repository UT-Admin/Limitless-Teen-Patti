 using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimationDotDot : MonoBehaviour
{
    public TMP_Text MessageTxt;
    private Tween MessageTween;
    private string Message = "Reconnecting to previous game";

    private void OnEnable()
    {
        MessageTxt.text = MessageTxt.text.Replace(".", "");
        Message = MessageTxt.text;
        string dots = ".";
        MessageTween?.Kill();
        MessageTween = DOTween.To(() => dots, x => dots = x, "...", 3).OnUpdate(() => MessageTxt.text = Message + $"<color=#ffffff>{dots}</color>").SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        MessageTween?.Kill();
    }
}