using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Creature : MonoBehaviour {

	[SerializeField] protected SpriteRenderer body;
	[SerializeField] protected SpriteRenderer[] eyes;
	[SerializeField] MaxMin eyeDistance;
	[SerializeField] MaxMin eyeHeight;
	[SerializeField] EventDefine AppearEvent;
	[SerializeField] MaxMin BodyScale;
	[SerializeField] MaxMin EyeScale;
	[SerializeField] MaxMin eyeTwinkle;
	[SerializeField] MaxMin eyeTwinkleInterval;

	protected bool m_enable;
	float oriY = 0;
	protected float bodyScale;

	protected virtual void OnEnable()
	{
		EventManager.Instance.RegistersEvent( AppearEvent , Appear);
	}

	protected virtual void OnDisable()
	{
		EventManager.Instance.UnregistersEvent( AppearEvent , Appear);
	}

	void Appear(Message msg )
	{
		SetAllSprite( true );
	}

	protected void SetAllSprite( bool to )
	{
		body.enabled = to;
		eyes[0].enabled = eyes[1].enabled = to;

		m_enable = to;
	
		if ( to )
		{
			// eye
			Sequence seq = DOTween.Sequence();
			float twinkleTime = Global.GetRandomMinMax( eyeTwinkle );
			float bodyScale = Random.Range( -1f , 1f ) * 0.05f;
			seq.Append( eyes[0].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Append( eyes[0].transform.DOScaleY( oriY , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( oriY , twinkleTime ) );
			seq.AppendInterval( Random.Range( 0 , 0.5f ));
			twinkleTime = Global.GetRandomMinMax( eyeTwinkle );
			bodyScale = Random.Range( -1f , 1f ) * 0.05f;
			seq.Append( eyes[0].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Append( eyes[0].transform.DOScaleY( oriY , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( oriY , twinkleTime ) );
			seq.AppendInterval( Global.GetRandomMinMax( eyeTwinkleInterval) );
			seq.SetLoops( 9999 , LoopType.Restart );

			// body
//			Sequence seqB = DOTween.Sequence();
//			float oribodyX = body.transform.localScale.x;
//			float bodyTime = Global.GetRandomMinMax( bodyShake );
//			seqB.Append( body.transform.DOScaleX( oribodyX + bodyScale , bodyTime ));
//			seqB.AppendInterval( Random.Range( 0 , 0.5f ));
//			seqB.Append( body.transform.DOScaleX( oribodyX , bodyTime ));
//			seqB.AppendInterval( Random.Range( 3f ,6f));
//			seqB.Append( body.transform.DORotate( new Vector3( 0 , 0, Random.Range( -20f , 20f )) , bodyTime * 3f)
//				.SetRelative(true).SetEase(Ease.InCirc).SetLoops( 2 , LoopType.Yoyo));
//			seqB.AppendInterval( Random.Range( 5f , 7f ));
//			seqB.SetLoops( 9999 , LoopType.Restart );


		}
		else{
			eyes[0].DOKill();
			eyes[1].DOKill();
		}

		if ( GetComponent<Collider>() != null )
			GetComponent<Collider>().enabled = to;
	}

	// Use this for initialization
	void Awake () {
		Init();
	}

	virtual protected void Init()
	{
		if ( body == null )
		{
			GameObject bodyObj = Instantiate( (GameObject) Resources.Load( "Prefab/Creature/CreatureBodyPrefab" ) ) as GameObject;
			bodyObj.transform.SetParent( transform , true );
			bodyObj.transform.localPosition = Vector3.zero;
			bodyScale = Random.Range( BodyScale.min , BodyScale.max );
			bodyObj.transform.localScale = bodyScale * Vector3.one;
			body = bodyObj.GetComponent<SpriteRenderer>();
		}

		if ( eyes == null || eyes.Length <= 0 )
		{
			float eyeScale =  Random.Range( EyeScale.min , EyeScale.max );
			eyes = new SpriteRenderer[2];
			float eyeDis = Random.Range( eyeDistance.min , eyeDistance.max );
			float eyeH = Random.Range( eyeHeight.min , eyeHeight.max );
			GameObject eyeObjLeft = Instantiate( (GameObject) Resources.Load( "Prefab/Creature/CreatureEyePrefab" ) ) as GameObject;
			eyeObjLeft.transform.SetParent( body.transform , true );
			eyeObjLeft.transform.localPosition = new Vector3( - eyeDis , eyeH );
			eyeObjLeft.transform.localScale = eyeScale * Vector3.one;
			eyes[0] = eyeObjLeft.GetComponent<SpriteRenderer>();
			GameObject eyeObjRight = Instantiate( (GameObject) Resources.Load( "Prefab/Creature/CreatureEyePrefab" ) ) as GameObject;
			eyeObjRight.transform.SetParent( body.transform , true );
			eyeObjRight.transform.localPosition = new Vector3( eyeDis , eyeH );
			eyeObjRight.transform.localScale = eyeScale * Vector3.one;
			eyes[1] = eyeObjRight.GetComponent<SpriteRenderer>();
		
		}

		SetAllSprite( false );
		oriY = transform.localScale.y;
	}


	virtual public void OnFingerDown( Vector3 position )
	{
		// do nothing
	}
}
