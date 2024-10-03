using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

[RequireComponent(typeof(ToggleGroup))]
public class BetterToggleGroup : ToggleGroup
{
    [System.Serializable]
    public class ToggleEvent : UnityEvent<Toggle> { }

    [SerializeField]
    public ToggleEvent onActiveTogglesChanged;

    protected override void Start()
    {
        /*foreach (Transform transformToggle in gameObject.transform)
        {
            var toggle = transformToggle.gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isSelected) => {
                if (!isSelected)
                {
                    return;
                }
                var activeToggle = Active();
                DoOnChange(activeToggle);
            });
        }*/

        foreach (Transform transformToggle in gameObject.transform)
        {
            var toggle = transformToggle.gameObject.GetComponent<Toggle>();

            if (toggle.group != null && toggle.group != this)
            {
                Debug.LogError($"EventToggleGroup is trying to register a Toggle that is a member of another group.");
            }
            toggle.group = this;
            toggle.onValueChanged.AddListener(HandleToggleValueChanged);
        }
    }
    public Toggle Active()
    {
        return ActiveToggles().FirstOrDefault();
    }

    void HandleToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            onActiveTogglesChanged?.Invoke(this.ActiveToggles().FirstOrDefault());
        }
        else
        {
            CheckIfAllOff();
        }
    }

    void CheckIfAllOff()
    {
        bool allOff = true;
        foreach (Toggle toggle in this.ActiveToggles())
        {
            if (toggle.isOn)
                allOff = false;
        }

        if (allOff)
            onActiveTogglesChanged.Invoke(null);
    }
}