using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTransform : MonoBehaviour
{
   // public float endPoint;
   // float startPoint = 0;
    //public RectTransform targetToMove;
    public float time;


    public float popUpEndPoint;
    public float popUpStartPoint;

    public RectTransform popupTransform;

    public static MoveTransform move;
    // Start is called before the first frame update
    void Start()
    {
        move = this;
       // startPoint = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartMoving()
    {
       // MiddleHeaderMovement.Instance.PopUpPosition();

        //targetToMove.transform.DOLocalMoveY(endPoint, time);
        popupTransform.transform.DOLocalMoveY(popUpEndPoint, time);
    }

    public void MoveToOrigin()
    {
   //     MiddleHeaderMovement.Instance.ResetPosition();
        // targetToMove.transform.DOLocalMoveY(startPoint, time/3f);
        popupTransform.transform.DOLocalMoveY(popUpStartPoint, time);
    }
}
