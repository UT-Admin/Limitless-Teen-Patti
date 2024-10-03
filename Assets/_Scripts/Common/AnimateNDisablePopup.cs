using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;




public class AnimateNDisablePopup : MonoBehaviour
{

    Image fadeInImage;
	Transform Panel;
    // Start is called before the first frame update

    private void Awake()
	{
#if GOP 
#else
				fadeInImage = GetComponent<Image>();
				Panel = this.transform.GetChild(0);
#endif
	}

	private void OnEnable()
	{
#if GOP
		Invoke(nameof(AnimatePopUp), 0.001f);


#elif TPF||TPV

		Panel.DOScale(new Vector3(1f, 1f, 1f), 0.3f).From();
		fadeInImage.DOFade(0, 0.5f).From();
#else

		Panel.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f).From();
				fadeInImage.DOFade(0, 0.5f).From();
#endif
	}

#if GOP
	private void AnimatePopUp()
	{
		fadeInImage = GetComponent<Image>();
		Panel = this.transform.GetChild(0);
		Panel.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f).From();
		fadeInImage.DOFade(0, 0.5f).From();
	}
#endif
}

