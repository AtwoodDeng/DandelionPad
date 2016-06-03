using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventRotate : OnEvent {

	[SerializeField] Vector3 rotation;
	[SerializeField] int loopTime = 1;
	[SerializeField] LoopType loopType;
	[SerializeField] bool isRelate = false;
	[SerializeField] bool once = false;

	bool isDone = false;
	protected override void Do (Message msg)
	{
		Debug.Log("once " + once + " isDone " + isDone);
		if ( !once || !isDone )
		{
			transform.DORotate(rotation, time ).SetRelative(isRelate).SetDelay(delay).SetEase(easeType).SetLoops(loopTime,loopType);
			isDone = true;
		}
	}
}
