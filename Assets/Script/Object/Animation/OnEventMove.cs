using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventMove : OnEvent {

	[SerializeField]Vector3 moveTo;
	[SerializeField]bool isRelative;
	[SerializeField] bool isFrom;
	[SerializeField] EventDefine postEvent;

	protected override void Do (Message msg)
	{
		if ( isFrom )
		{
			transform.DOLocalMove( moveTo , time ).From().SetRelative(isRelative).SetEase(easeType).SetDelay(delay).OnComplete(Post);
		}
		else {
			transform.DOLocalMove( moveTo , time ).SetRelative(isRelative).SetEase(easeType).SetDelay(delay).OnComplete(Post);
		}
	}

	void Post()
	{
		EventManager.Instance.PostEvent( postEvent );
	}
}
