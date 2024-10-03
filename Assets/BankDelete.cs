using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


namespace TP
{
    public class BankDelete : MonoBehaviour
    {
        // Start is called before the first frame update
        public TMPro.TMP_Text Number;  


         

        public void DeleteBank()
        {
            string Val = Number.text;

           string  AccNumber = Regex.Match(Val, @"\d+").Value;
            Debug.LogError("Ac" + AccNumber);
            UIController.Instance.ShowLoadingScreen();
            BankDetailsModel[] bankmodels = GameController.Instance.CurrentPlayerData.GetBankDetails();
            var modelList = bankmodels.ToList();
            var data = modelList.First(x => x.AccNumber == AccNumber);
            modelList.Remove(data);
            bankmodels = modelList.ToArray();

            






        }
    }
}
