using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinAnimationSlider : MonoBehaviour
{

    public Slider[] CoinSliders;
    public float maxSliderValue = 10f;


    public void ChangeSliderValueBasedOnAmount(float amount)
    {
        if (CoinSliders.Length != 4)
        {
            Debug.LogError("CoinSliders array length must be 4.");
            return;
        }

        foreach (Slider slider in CoinSliders)
        {
            slider.value = 0;
            slider.gameObject.SetActive(false);
        }

        int activeSliders = Mathf.CeilToInt(amount / maxSliderValue);
        activeSliders = Mathf.Clamp(activeSliders, 1, CoinSliders.Length);

        for (int i = 0; i < activeSliders; i++)
        {
            CoinSliders[i].gameObject.SetActive(true);
            if (amount > maxSliderValue)
            {
                CoinSliders[i].value = maxSliderValue;
                amount -= maxSliderValue;
            }
            else
            {
                CoinSliders[i].value = amount;
                amount = 0;
            }
        }
    }

    public void ResetSlider()
    {
        foreach (Slider slider in CoinSliders)
        {
            slider.value = 0;
            slider.gameObject.SetActive(false);
        }
    }
}


