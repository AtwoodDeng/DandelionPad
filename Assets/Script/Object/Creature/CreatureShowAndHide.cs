using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CreatureShowAndHide : CreatureHide {


	float timer = 0;
	[SerializeField] bool isJump;
	[SerializeField] MaxMin moveInterval;

	SphereCollider collider;

	void Update()
	{
		if ( m_enable )
		{
			timer -= Time.deltaTime * LogicManager.PhysTimeRate;
			if ( timer < 0 )
			{
				timer = Global.GetRandomMinMax( moveInterval );
				if ( isJump )
				{
					Sequence seq = DOTween.Sequence();
					seq.Append( body.transform.DOLocalMove( GetLocalOffset(3f) , ShowTime).SetEase(Ease.OutQuad));
					seq.Append( body.transform.DOLocalMove( GetLocalOffset(0) , HideTime).SetEase(Ease.InQuad));
				}else
				{
					if ( isHide ) {
						ShowUp( );
					}
					else
						Hide( );
				}
				
			}
			if ( collider == null )
				collider = GetComponent<SphereCollider>();
			if ( collider != null )
				collider.center = body.transform.localPosition;

		}
	}
}
