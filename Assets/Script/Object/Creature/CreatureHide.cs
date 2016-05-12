using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CreatureHide : Creature {
	[SerializeField] float MoveRate = 0.1f;
	Vector3 toPosition; // local position
	float toAngle;  // in degreed
	[SerializeField] float HideRate = 0.2f;
	[SerializeField] float Angle; // in degreed
	[SerializeField] float Offset;
	[SerializeField] EventDefine showUpEvent;
	[SerializeField] EventDefine hideEvent;

	float colliderRadius;
	float rate;

	List<GameObject> petals = new List<GameObject>();

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

	void ShowUp()
	{
		toPosition = GetLocalOffset(1f);
		toAngle = Angle;
		rate = MoveRate;
		isHide = false;
	}

	void Hide ()
	{
		toPosition = GetLocalOffset(0f);
		toAngle = Angle + Random.Range( -20f , 20f );
		rate = HideRate;
		isHide = true;

		body.transform.DOShakeRotation( 0.5f , 25f );
	}

	void Hide( Message msg )
	{
		Hide();
	}

	protected override void Init ()
	{
		base.Init ();
		toPosition = GetLocalOffset(0);
		toAngle = Angle;
		if ( GetComponent<SphereCollider>() != null )
			colliderRadius = GetComponent<SphereCollider>().radius;
		rate = MoveRate;
		body.transform.rotation = Quaternion.Euler( 0 , 0 , Angle );
	}

	void Update()
	{ 
		UpdateSelf();
	}

	bool isHide = false;
	void OnTriggerEnter( Collider col )
	{
		if ( ( col.tag == "Petal" || col.tag == "FlowerPetal" ) && !isHide )
		{
			petals.Add( col.gameObject );
			Hide();

			Sequence seq = DOTween.Sequence();
			seq.AppendCallback( Hide );
			seq.AppendInterval( Random.Range( 1f , 3f ));
			seq.AppendCallback( ShowUp );
		}
	}

//	void OnTriggerExit( Collider col )
//	{
//		if ( col.tag == "Petal" || col.tag == "FlowerPetal" )
//		{
//			petals.Remove( col.gameObject );
//			if ( petals.Count <= 0 )
//			{
//				ShowUp();
//			}
//		}
//	}

	void UpdateSelf()
	{
		if ( ! m_enable ) return;

		Vector3 pos = body.transform.localPosition ;
		pos = Vector3.Lerp( pos , toPosition , rate * Time.deltaTime * 30f *  LogicManager.PhysTimeRate );
		body.transform.localPosition = pos;

		//		body.transform.rotation = Quaternion.Lerp( body.transform.rotation , Quaternion.Euler( new Vector3( 0 ,0 , toAngle )) , rate * 30f * Time.deltaTime * LogicManager.PhysTimeRate);
		
	}

	Vector3 GetLocalOffset( float process )
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
