using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
namespace TP
{
    public class BankListHolder : MonoBehaviour
    {


        [Header("NewFeature Variables")]
        public TMP_Dropdown banks;

        public Image dropdownImage;
        public Sprite[] scrollImage;

        List<BankDetailsModel> bankmodels;



        private void OnEnable()
        {
            AddDropDownOption();
        }


        public void AddDropDownOption()
        {


            bankmodels = GameController.Instance.CurrentPlayerData.GetBankDetails().ToList();
            List<string> optionsval = new List<string>();
            bankmodels = bankmodels.Where(x => x.BankStatus).ToList();//.OrderByDescending(a => a.AddedOn).ToList();
            for (int i = 0; i < bankmodels.Count; i++)
            {
                optionsval.Add(bankmodels[i].BankName + " " + bankmodels[i].AccNumber);

            }

            banks.AddOptions(optionsval);

            dropdownImage.sprite = banks.options.Count <= 1 ? scrollImage[0] : scrollImage[1];





        }

    
    }

}
