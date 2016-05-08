using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventRotate : OnEvent {

	[SerializeField] Vector3 rotation;
	[SerializeField] int loopTime = 1;
	[SerializeField] LoopType loopType;

	protected override void Do (Message msg)
	{
		transform.DORotate(rotation, time ).SetDelay(delay).SetEase(easeType).SetLoops(loopTime,loopType);
	}
}
