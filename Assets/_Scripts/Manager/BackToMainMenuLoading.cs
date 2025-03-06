using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMainMenuLoading : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        InvokeRepeating("Exit", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Exit()
    {
#if !UNITY_SERVER
        APIController.CloseWindow();
#endif
    }
}