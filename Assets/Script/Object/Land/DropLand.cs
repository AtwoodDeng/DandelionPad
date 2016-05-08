using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DropLand : SenseGuesture {

//	[SerializeField] int requireTapTime = 3;
//	[SerializeField] float dropTime = 2f;
//	[SerializeField] float dropDelay = 5f;
//	[SerializeField] SpriteRenderer sprite;
//	[SerializeField] Rigidbody2D rigidbody;
//	[SerializeField] AudioSource soundMove;
//	[SerializeField] AudioSource soundDrop;
//
//
//
//	int tapTime = 0;
//	void Awake()
//	{
//		if ( sprite == null )
//		{
//			sprite = GetComponent<SpriteRenderer>();
//		}
//		if ( rigidbody == null )
//		{
//			rigidbody = GetComponent<Rigidbody2D>();
//			rigidbody.isKinematic = true;
//		}
//
//	}
//
//	public override void DealTap (TapGesture guesture)
//	{
//		Debug.Log("Tap Drop Land");
//		tapTime ++;
//
//		transform.DOShakeRotation( 0.5f * tapTime , 3f * tapTime );
//		if (soundMove != null )
//		{
//			soundMove.volume = 0.6f * ( 1 + tapTime ) / requireTapTime;
//			soundMove.Play();
//
//		}
//		if ( tapTime >= requireTapTime )
//		{
//			sprite.DOFade( 0 , dropTime ).SetDelay(dropDelay);
//			rigidbody.isKinematic = false;
//			if (soundDrop != null )
//			{
//				soundDrop.Play();
//			}
//			if ( LogicManager.LevelManager.GetWind() != null)
//				LogicManager.LevelManager.GetWind().StartUpdateWind();
//		}
//
//	}


}
