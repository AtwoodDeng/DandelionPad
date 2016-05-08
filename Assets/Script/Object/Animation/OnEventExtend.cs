using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OnEventExtend : OnEvent {

	[SerializeField] bool initZero = true;
	[SerializeField] float extendToY;

	float m_toY;
	void Awake()
	{
		if ( initZero )
		{
			m_toY = transform.localScale.y;
			Vector3 scale = transform.localScale;
			scale.y = 0;
			transform.localScale = scale;
		}else
		{
			m_toY = extendToY;
		}
	}
		
	override protected void Do(Message msg )
	{
		transform.DOScaleY( m_toY , time ).SetDelay(delay).SetEase(easeType);
	}

}
