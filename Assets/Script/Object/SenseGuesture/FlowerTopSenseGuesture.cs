using UnityEngine;
using System.Collections;

public class FlowerTopSenseGuesture : SenseGuesture {
	[SerializeField] Flower flower;

	BoxCollider boxCol;
	public bool enableColliderChange = false;

	void Awake()
	{
		boxCol = GetComponent<BoxCollider>();
	}

	float timer = 0.1f;
	void Update()
	{
		timer += Time.deltaTime;
		if ( timer > 0.5f )
		{
			UpdateColliderPosition();
			timer = 0;
		}
	}

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
			break;
		}
		return check;
	}

	public override void DealSwipe (SwipeGesture guesture)
	{
		// TODO change the algorithm of top collider computation
		if ( CheckIfFlowerEvent( guesture.Fingers[0].Index ) )
		{
			flower.Blow( guesture.Move.normalized , guesture.Velocity * Global.Pixel2Unit );
		}
		UpdateColliderPosition();
	}



	public override void DealOnFingerHover (FingerHoverEvent e)
	{
		if ( e.Phase == FingerHoverPhase.Exit && CheckIfFlowerEvent(e.Finger.Index) )
		{
			Vector2 velocity = e.Finger.DeltaPosition / Time.deltaTime; 
			flower.Blow( velocity * Global.Pixel2Unit );
		}
		UpdateColliderPosition();
	}

	public override void DealOnFingerMotion (FingerMotionEvent e)
	{
		if ( e.Phase == FingerMotionPhase.Updated  && CheckIfFlowerEvent(e.Finger.Index))
		{
			Vector2 velocity = e.Finger.DeltaPosition / Time.deltaTime; 
			flower.Blow( velocity * Global.Pixel2Unit );
		}
		UpdateColliderPosition();
	}

	public void UpdateColliderPosition()
	{
		if ( boxCol == null )
			return;
		if ( enableColliderChange == false )
			return ;
		
		Petal[] petals = GetComponentsInChildren<Petal>();
		Vector3 pos = transform.position;

		float top = 0.2f + pos.y;
		float bot = - 0.2f + pos.y;
		float left = - 0.2f + pos.x;
		float right = 0.2f + pos.x;

		foreach( Petal p in petals )
		{
			if ( p.top != null && p.state == PetalState.Link )
			{
				top = Mathf.Max( top , p.top.position.y );
				bot = Mathf.Min( bot , p.top.position.y );
				left = Mathf.Min( left , p.top.position.x );
				right = Mathf.Max( right , p.top.position.x );
			}
		}

		boxCol.center = new Vector3( (left + right ) / 2f , ( top + bot ) / 2f , 0  ) - transform.position;
		boxCol.size = new Vector3( (right - left ) + 0.4f  , ( top - bot ) + 0.4f  , 1f ) ;

	}



}
