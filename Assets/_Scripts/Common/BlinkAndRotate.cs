using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TP
{
    public class BlinkAndRotate : MonoBehaviour
    {
        [SerializeField] private GameObject shineImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Color _highlightColor;

        Sequence highlighSeq;

        private void Awake()
        {
            highlightImage.color = _highlightColor;
        }

        private void OnEnable()
        {
            //highlightImage.DOKill();

            /* highlightImage.DOFade(0.3f, 2f).From().SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnComplete(() => {
                 highlightImage.gameObject.SetActive(false);
             });
             shineImage.transform.DORotate(new Vector3(0, 0, 360), 12f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetRelative();*/

            highlighSeq = DOTween.Sequence()
                .Append(highlightImage.DOFade(0.5f, 2f).From().SetEase(Ease.Linear).SetLoops(6, LoopType.Yoyo).OnComplete(() =>
                {
                    this.gameObject.SetActive(false);
                    this.gameObject.SetActive(true);
                }))
                .Join(shineImage.transform.DORotate(new Vector3(0, 0, 360), 12f, RotateMode.FastBeyond360))//.SetLoops(1, LoopType.Incremental))
                
                ;
        }

        private void OnDisable()
        {
            if(highlighSeq != null)
                highlighSeq.Kill();
        }
    }

}