using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace TP
{
    public class AdsFailedPoupu : UIHandler
    {

        public static AdsFailedPoupu instance;
        public TMP_Text _message;
        private void Awake()
        {
            instance = this;
        }
        public override void HideMe()
        {
            UIController.Instance.RemoveFromOpenPages(this);
            gameObject.SetActive(false);
        }

        public override void OnBack()
        {
            HideMe();
        }

        public override void ShowMe()
        {
            UIController.Instance.AddToOpenPages(this);
            gameObject.SetActive(true);
           StartCoroutine(HideAfterFewsecs());
          
        }
           

        public void  message(string message)
        {
            _message.text = message;
 
        }

        public IEnumerator HideAfterFewsecs()
        {
            yield return new WaitForSeconds(2f);
           HideMe();
        }

    }
}




