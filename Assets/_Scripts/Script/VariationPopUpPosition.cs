using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class VariationPopUpPosition : MonoBehaviour
{
    public RectTransform target;

    private void OnEnable()
    {
        transform.position = target.transform.position;
    }

    void Update()
    {
        transform.position = target.transform.position;
    }
}
