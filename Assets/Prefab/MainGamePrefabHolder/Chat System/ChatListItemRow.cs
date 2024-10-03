using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatListItemRow : MonoBehaviour
{
    TMP_Text text;

    private void Awake()
    {
       
    }

    public void ShowText(string message)
    {
        if(text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }
        text.text = message;
        this.gameObject.SetActive(true);
    }
}
