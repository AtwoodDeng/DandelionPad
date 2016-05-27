using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventChangeOrthSize : OnEvent {
	[SerializeField] float initOrth;
	[SerializeField] float finalOrth;
	[SerializeField] CameraManager camerManager;
	[SerializeField] bool isInit = false;

	void Start()
	{
		if ( isInit )
			camerManager.AllCameraSetOrthoSize( initOrth );
	}

	protected override void Do (Message msg)
	{
		if ( camerManager != null )
		{
			DOTween.To(
				(x) => camerManager.AllCameraSetOrthoSize( x )
				, initOrth
				, finalOrth
				, time ).SetDelay(delay);
		}
	}
}
