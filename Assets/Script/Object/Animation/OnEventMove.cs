using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventMove : OnEvent {

	[SerializeField]Vector3 moveTo;
	[SerializeField] bool isRelative;
	[SerializeField] bool isFrom;
	[SerializeField] EventDefine postEvent;
	[SerializeField] int loopTime = 1;
	[SerializeField] LoopType loopType;
	[SerializeField] bool isInit = false;

	Vector3 initPos;
	void Awake()
	{
		if ( isInit && isFrom )
		{
			initPos = transform.position;
			Vector3 startPosition = moveTo;
			if ( isRelative )
			{
				startPosition += initPos;
			}
			transform.position = startPosition;
		}
	}

	protected override void Do (Message msg)
	{
		if ( isFrom )
		{
			if ( isInit )
			{
				transform.DOLocalMove( initPos , time ).SetEase(easeType).SetDelay(delay).OnComplete(Post).SetLoops(loopTime,loopType);
			}else
			{
				transform.DOLocalMove( moveTo , time ).From().SetRelative(isRelative).SetEase(easeType).SetDelay(delay).OnComplete(Post).SetLoops(loopTime,loopType);
			}
		}
		else {
			transform.DOLocalMove( moveTo , time ).SetRelative(isRelative).SetEase(easeType).SetDelay(delay).OnComplete(Post).SetLoops(loopTime,loopType);
		}
	}

	void Post()
	{
		EventManager.Instance.PostEvent( postEvent );
	}
}
