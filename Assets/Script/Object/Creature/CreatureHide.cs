using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CreatureHide : Creature {
	[SerializeField] protected float ShowTime = 2f;
	Vector3 toPosition; // local position
//	float toAngle;  // in degreed
	[SerializeField] protected float HideTime = 0.5f;
	[SerializeField] float Angle; // in degreed
	[SerializeField] float Offset;
	[SerializeField] EventDefine showUpEvent;
	[SerializeField] EventDefine hideEvent;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		EventManager.Instance.RegistersEvent( showUpEvent , ShowUp );
		EventManager.Instance.RegistersEvent( hideEvent , Hide );
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
		EventManager.Instance.UnregistersEvent( showUpEvent , ShowUp );
		EventManager.Instance.UnregistersEvent( hideEvent , Hide );
	}

	void ShowUp( Message msg )
	{
		Sequence seq = DOTween.Sequence();
		seq.AppendInterval( Random.Range( 0 , 0.5f));
		seq.AppendCallback( ShowUp );
	}

	protected void ShowUp(  )
	{
		body.transform.DOKill();
		body.transform.DOLocalMove( GetLocalOffset(1f) , ShowTime);
//		toAngle = Angle;
		isHide = false;
	}

	protected void Hide ( )
	{
		body.transform.DOKill();
		body.transform.DOLocalMove( GetLocalOffset(0) , HideTime);
			
		isHide = true;
	}

	void Hide( Message msg )
	{
		Hide();
	}

	protected override void Init ()
	{
		base.Init ();
		toPosition = GetLocalOffset(0);
//		toAngle = Angle;
		body.transform.rotation = Quaternion.Euler( 0 , 0 , Angle );
	}


	protected bool isHide = false;
	void OnTriggerEnter( Collider col )
	{
		if ( ( col.tag == "Petal" || col.tag == "FlowerPetal" ) && !isHide )
		{

			Sequence seq = DOTween.Sequence();
			seq.AppendCallback( Hide );
			seq.AppendInterval( Random.Range( 1f , 3f ));
			seq.AppendCallback( ShowUp );
		}
	}


	public Vector3 GetLocalOffset( float process )
	{
		return new Vector3( Offset * Mathf.Sin( - Angle * Mathf.Deg2Rad ) , Offset * Mathf.Cos( Angle * Mathf.Deg2Rad )) * process;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;

		Vector3 to = GetLocalOffset(1f);
		Gizmos.DrawLine( transform.position , transform.position + to );
	}

	public override void OnFingerDown (Vector3 position)
	{
		base.OnFingerDown (position);
		Sequence seq = DOTween.Sequence();
		seq.AppendCallback( Hide );
		seq.AppendInterval( Random.Range( 1f , 3f ));
		seq.AppendCallback( ShowUp );
	}

}
