using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransactionRowItem : MonoBehaviour
{
    [SerializeField] private TMP_Text serialNumber;
    [SerializeField] private TextMeshProUGUI amountTxt;
    [SerializeField] private TextMeshProUGUI typeTxt;
    [SerializeField] private Button clickBtn;

    public void Init(string serialNumberVal ,string amountVal, string typeVal, UnityAction OnClick)
    {
        serialNumber.text = serialNumberVal;
        amountTxt.text = amountVal;
        typeTxt.text = typeVal;
        clickBtn.onClick.RemoveAllListeners();
        clickBtn.onClick.AddListener(OnClick);
    }
}

