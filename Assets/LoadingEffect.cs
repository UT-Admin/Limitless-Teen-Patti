using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingEffect : MonoBehaviour
{
    public Image LoadingImg;
    private Sequence LoadingSequence;
    private void OnEnable()
    {
#if !UNITY_SERVER
        StartLoading();
#endif
    }

    private void OnDisable()
    {
        LoadingSequence?.Kill();
    }

    private void StartLoading()
    {
        LoadingSequence?.Kill();
        LoadingImg.fillAmount = 1;
        LoadingImg.transform.localScale = Vector3.one;
        LoadingSequence = DOTween.Sequence().Append(LoadingImg.DOFillAmount(0, 1f).OnUpdate(() => LoadingImg.transform.localScale = new(-1, 1, 1)))
            .Append(LoadingImg.DOFillAmount(1, 1f).OnUpdate(() => LoadingImg.transform.localScale = Vector3.one)).SetLoops(-1, LoopType.Restart);
    }
}
