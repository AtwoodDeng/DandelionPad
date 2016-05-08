using UnityEngine;
using System.Collections;

public class FollowWindSenseGuesture : SenseGuesture {

	[SerializeField] FollowWind followWind;
//	[SerializeField] MaxMin velocityRange;
	[SerializeField] float touchIntense = 1f;
	[SerializeField] float touchAttachRate = 0.5f;

	void Awake()
	{
		if ( followWind == null )
			followWind = GetComponent<FollowWind>();
	}

//	public override void DealSwipe (SwipeGesture guesture)
//	{
//		if ( followWind != null )
//		{
//			Vector3 touchPosition = Camera.main.ScreenToWorldPoint( guesture.StartPosition );
//			touchPosition.z = transform.position.z;
//			float velocity = guesture.Velocity * Global.Pixel2Unit;
//
//
//			followWind.AddImpuse( Global.V2ToV3(guesture.Move.normalized) * velocity
//				, touchPosition  );
//		}
//	}

//	public override void DealTap (TapGesture guesture)
//	{
//		if ( followWind != null )
//		{
//			Vector3 touchPosition = Camera.main.ScreenToWorldPoint( guesture.StartPosition );
//			touchPosition.z = transform.position.z;
//
//			followWind.AddImpuse( guesture.ElapsedTime
//				, touchPosition  );
//		}
//	}
//


	bool CheckIfFlowerEvent( int index )
	{
		bool check = false;
		switch( Application.platform )
		{
		case RuntimePlatform.Android:
		case RuntimePlatform.IPhonePlayer:
			check = FingerGestures.Touches.Count <= 1;
			break;
		default :
			check = index == 0;
			Debug.Log("Check index " + index );
			break;
		}
		return check;
	}

	float hoverTime;
	public override void DealOnFingerHover (FingerHoverEvent e)
	{
		
		if ( followWind != null && ( Time.time - e.Finger.StarTime ) > 0.12f && CheckIfFlowerEvent(e.Finger.Index) )
		{
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint( e.Position );
//			float angle = Vector3.Dot( Vector3.forward , Vector3.Cross( transform.up , touchPosition - transform.position ) );
//			followWind.AddImpuse( angle * Time.deltaTime * touchIntense  , touchPosition );

			Vector2 velocity = e.Finger.DeltaPosition / Time.deltaTime;
			followWind.AttachVelocity( velocity , touchPosition , touchAttachRate * 0.0001f );
			if ( e.Phase == FingerHoverPhase.Enter )
			{
				followWind.PlayTouchSound();
			}
		}


	}


	public override void DealOnFingerDown (FingerDownEvent e)
	{
		if ( followWind != null && CheckIfFlowerEvent(e.Finger.Index) )
		{
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint( e.Position );
			float angle = Vector3.Dot( Vector3.forward , Vector3.Cross( transform.up , touchPosition - transform.position ) );
			followWind.AddImpuse( angle * Time.deltaTime * touchIntense  , touchPosition );
		}
	}
}
