using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventScale : OnEvent {
	[SerializeField] bool isFrom = false;
	[SerializeField] Vector3 toScale;


	void OnEvent(Message msg )
	{
		if ( isFrom ) {
			transform.DOScale( toScale , time ).From().SetDelay(delay).SetEase(easeType);
		}
		else {
			transform.DOScale( toScale , time ).SetDelay(delay).SetEase(easeType);
		}
	}
}
