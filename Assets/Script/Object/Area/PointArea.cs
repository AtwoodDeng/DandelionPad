using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PointArea : Area {

	[SerializeField] SpriteRenderer sprite;
	[SerializeField] float appearDelay = 5f;

	public bool isFinished = false;

	void Awake()
	{
		gameObject.tag = "FinalPoint";
		if ( sprite == null )
			sprite = GetComponent<SpriteRenderer>();
		sprite.DOFade(0,0);
		if ( col2D == null )
			col2D = GetComponent<Collider2D>();
		if ( col3D == null )
			col3D = GetComponent<Collider>();
	}

	void Start()
	{
		StartCoroutine( startCor(appearDelay));
	}

	Sequence shineSequence;
	IEnumerator startCor(float delay)
	{
		yield return new WaitForSeconds(delay);
		if ( sprite != null )
		{
			shineSequence = DOTween.Sequence();
			shineSequence.Append( sprite.DOFade( 1f , 1f ));
			shineSequence.Append( sprite.DOFade( 0.5f , 1f ).SetLoops( 99999 , LoopType.Yoyo ).SetEase(Ease.InOutCubic));
		}
		yield break;
		
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn, GrowFlowerOn);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn, GrowFlowerOn);
	}

	void GrowFlowerOn(Message msg)
	{	
		PetalInfo info = (PetalInfo) msg.GetMessage("info");
		if ( info.affectPointArea == this )
		{
			Disappear();
		}
	}

	void Disappear()
	{
		if ( sprite != null )
		{
			shineSequence.Kill();
			sprite.DOFade( 0 , 1f );
		}
		if ( col3D != null )
			col3D.enabled = false;
		if ( col2D != null )
			col2D.enabled = false;
		isFinished = true;

	}

}
