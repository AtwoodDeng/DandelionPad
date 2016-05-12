using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Creature : MonoBehaviour {

	[SerializeField] protected SpriteRenderer body;
	[SerializeField] SpriteRenderer[] eyes;
	[SerializeField] MaxMin eyeDistance;
	[SerializeField] MaxMin eyeHeight;
	[SerializeField] EventDefine AppearEvent;
	[SerializeField] MaxMin BodyScale;
	[SerializeField] MaxMin EyeScale;
	[SerializeField] MaxMin eyeTwinkle;



	protected bool m_enable;
	float oriY = 0;

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

	void SetAllSprite( bool to )
	{
		body.enabled = to;
		eyes[0].enabled = eyes[1].enabled = to;

		m_enable = to;
	
		if ( to )
		{
			Sequence seq = DOTween.Sequence();
			float twinkleTime = Global.GetRandomMinMax( eyeTwinkle );
			float oribodyX = body.transform.localScale.x;
			float bodyScale = Random.Range( -1f , 1f ) * 0.05f;
			seq.Append( eyes[0].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Join( body.transform.DOScaleX( oribodyX + bodyScale , twinkleTime ));
			seq.Append( eyes[0].transform.DOScaleY( oriY , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( oriY , twinkleTime ) );
			seq.Join( body.transform.DOScaleX( oribodyX , twinkleTime ) );
			seq.AppendInterval( Random.Range( 0 , 0.5f ));
			twinkleTime = Global.GetRandomMinMax( eyeTwinkle );
			bodyScale = Random.Range( -1f , 1f ) * 0.05f;
			seq.Append( eyes[0].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( 0f , twinkleTime ) );
			seq.Join( body.transform.DOScaleX( oribodyX + bodyScale , twinkleTime ));
			seq.Append( eyes[0].transform.DOScaleY( oriY , twinkleTime ) );
			seq.Join( eyes[1].transform.DOScaleY( oriY , twinkleTime ) );
			seq.Join( body.transform.DOScaleX( oribodyX , twinkleTime ) );
			seq.AppendInterval( Random.Range( 5f , 9f ));
			seq.SetLoops( 9999 , LoopType.Restart );

		}
		else{
			eyes[0].DOKill();
			eyes[1].DOKill();
		}
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
			bodyObj.transform.localScale = Random.Range( BodyScale.min , BodyScale.max ) * Vector3.one;
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
