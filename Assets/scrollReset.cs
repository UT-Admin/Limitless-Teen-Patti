using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollReset : MonoBehaviour
{
    public ScrollRect scroll;


    private void OnEnable()
    {
     
    }

    private void OnDisable()
    {
        
    }

    public void ScrollReset()
    {
        scroll.verticalNormalizedPosition = 1;
    }
}
