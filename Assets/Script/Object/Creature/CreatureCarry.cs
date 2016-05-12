using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CreatureCarry : Creature {
	[SerializeField] Animator animator;
	[SerializeField] float appearDuration = 1f;
	[SerializeField] Vector3 appearMove;
	[SerializeField] EventDefine showEvent;
	[SerializeField] Vector3 jumpPoint;
	[SerializeField] Vector3 jumpHigh;
	[SerializeField] GameObject landObj;
	[SerializeField] Vector3 landPoint;
	Petal carryPetal = null;

	Rigidbody rigid = null;

	bool isCarried = false;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		EventManager.Instance.RegistersEvent( showEvent , Show );
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
		EventManager.Instance.UnregistersEvent( showEvent , Show );
	}

	void Show( Message msg )
	{
		Debug.Log("Carry Show");

		transform.DOLocalMoveY( 0.6f , 1f ).SetRelative(true);
		SetAllSprite( true );
	}

	void OnCollisionEnter(Collision coll)
	{
		Petal p = coll.gameObject.GetComponent<Petal>();
		if ( coll.gameObject != null && p != null )
		{
			if ( p.state == PetalState.Fly )
			{
				Carry( coll.gameObject.GetComponent<Petal>() );
			}
		}
	}


	void Carry(Petal p )
	{
		if ( p == null ) return;
		if ( isCarried ) return;

		isCarried = true;
		GetComponent<Collider>().isTrigger = true;

		carryPetal = p;
		carryPetal.transform.SetParent( transform , true );
//		carryPetal.transform.DOLocalMove( Vector3.up * 0.33f , 1f ).SetEase(Ease.InCubic);
		carryPetal.Keep();

		Sequence seq = DOTween.Sequence();
		seq.AppendInterval( 0.3f );
		seq.Append( transform.DOLocalMoveY( 0.3f , 1f ).SetRelative(true) );
		seq.Join( carryPetal.transform.DOLocalMove( Vector3.up * 1f , 0.7f ).SetEase(Ease.InCubic) );
		seq.Join( eyes[0].transform.DOLocalMoveY( 0.2f , 0.5f ).SetRelative(true));
		seq.Join( eyes[1].transform.DOLocalMoveY( 0.2f , 0.5f ).SetRelative(true));
		seq.Append( transform.DOLocalMove( jumpPoint , 2f ).SetEase(Ease.InOutCubic) );
		seq.Join( carryPetal.transform.DOLocalMove( Vector3.up * 0.7f  , 0.3f ).SetEase(Ease.InCubic) );
		seq.Join( eyes[0].transform.DOLocalMoveY( - 0.2f , 0.4f  ).SetRelative(true));
		seq.Join( eyes[1].transform.DOLocalMoveY( - 0.2f , 0.4f).SetRelative(true));
		seq.Append( transform.DOScaleY ( 0.8f , 0.2f ) );
		seq.Append( transform.DOScaleY ( 1f , 0.2f ) );
		seq.Join( transform.DOLocalMove( jumpHigh ,  0.5f ).SetEase(Ease.OutQuad).SetRelative( true ));
		seq.AppendCallback( SwitchRock );

	}

	void SwitchRock()
	{
		transform.SetParent( landObj.transform , true );
		Sequence seq = DOTween.Sequence();
		seq.Append( transform.DOLocalMove( landPoint , 3f ).SetEase(Ease.InQuad));
		seq.Append( transform.DOLocalMove ( Vector3.up * 0.1f , 0.5f ).SetRelative( true ) );
		seq.Append( carryPetal.transform.DOLocalMove( Vector3.up * 0.66f , 0.3f ).SetEase(Ease.InCubic));
		seq.AppendCallback( ReleasePetal );
	}

	void ReleasePetal()
	{
		carryPetal.Release();
		carryPetal.transform.DOMoveZ(0,0);
		carryPetal.Blow( new Vector2( - 0.1f , -1f ) * 75f , Petal.BlowType.Normal );
		carryPetal = null;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube( transform.parent.TransformPoint( jumpPoint ) , Vector3.one);
		Gizmos.DrawWireCube( transform.parent.TransformPoint( jumpPoint + jumpHigh ) , Vector3.one);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube( landObj.transform.TransformPoint( landPoint ), Vector3.one );
	}
}
