using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class SideShowAccept : MonoBehaviour
{ 
    bool wasActiveEarlier;
    bool wasActiveEarlieri;
    public Button Accept, Reject;
    public Animator[] sideShowEffects;
    public static SideShowAccept Instance;

    private void Awake()
    {
        Accept.onClick.AddListener(() => { OnClickAccept(); });
        Reject.onClick.AddListener(() => { OnClikcReject(); });
        Instance = this;
        
    }

    private void OnEnable()
    {


        if (TeenpattiGameUIHandler.instance.HowToplay.activeSelf)
        {
            wasActiveEarlier = true;
            TeenpattiGameUIHandler.instance.HowToplay.SetActive(false);
        }
        else
        {
            wasActiveEarlier = false;
        }

        if(TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.activeSelf)
        {
            wasActiveEarlieri = true;
            TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.SetActive(false);
        }else
        {
            wasActiveEarlieri = false;
        }

        this.gameObject.GetComponent<Image>().DOFade(0.7f, 0.5f).From(0);
    }

    private void OnDisable()
    {
        //if (wasActiveEarlier)
        //{
        //    TeenpattiGameUIHandler.instance.HowToplay.SetActive(true);
        //}

        //if(wasActiveEarlieri)
        //{
        //    TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.SetActive(true);
        //}


    }

    public void OnClickAccept()
    {

        Invoke(nameof(SetThisObjectOff), 0.7f);
        sideShowEffects[0].Play("ButtonAnimationSideShowReturn");
        sideShowEffects[1].Play("SideShowReturn");
        sideShowEffects[2].Play("SideShowBlueReturn");
        sideShowEffects[3].Play("SideShowTextReturn");
        GamePlayUI.instance.OnAcceptSideShow();

    }

    public void OnClikcReject()
    {
        Invoke(nameof(SetThisObjectOff), 0.7f);
        sideShowEffects[0].Play("ButtonAnimationSideShowReturn");
        sideShowEffects[1].Play("SideShowReturn");
        sideShowEffects[2].Play("SideShowBlueReturn");
        sideShowEffects[3].Play("SideShowTextReturn");
        GamePlayUI.instance.OnDeclineSideShow();

    }

    public void SetThisObjectOff()
    {
        this.gameObject.GetComponent<Image>().DOFade(0, 0.5f).From(0.7f).OnComplete(() => { gameObject.SetActive(false); });
        
    }



}
