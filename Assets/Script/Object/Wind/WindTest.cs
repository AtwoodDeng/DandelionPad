using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WindTest : MonoBehaviour , WindSensable {

	[SerializeField] float WindVelSense = 30f;
	[SerializeField] float WindRotateSense = 30f;
	[SerializeField] float maxVel = 1f;
	[SerializeField] float maxRotVel = 1f;
	[SerializeField] float fadeTime = 0.7f;
	[SerializeField] float fadeTimeDiff= 0.3f;
	public WindAdv wind;
	[SerializeField] GameObject model;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] MaxMin scaleRange;
	[SerializeField] MaxMin velocityRange;


	public enum State
	{
		Enter, // enter the wind 
		Move,  // in the wind and update every frame
		Exit,  // exit the wind, but still moving 
		Invisible
	}
	public State state = State.Invisible;

	Vector2 windVelocity;
	public void SenseWind(Vector2 velocity)
	{
		windVelocity = velocity;
	}

	public void ExitWind(){
		windVelocity = Vector2.zero;
	}

	public WindTestGenerator parent;


	[SerializeField]public WindSensablParameter windSensablParameter;

	public WindSensablParameter getParameter()
	{
		return windSensablParameter;
	}

	public Vector3 getPosition()
	{
		return transform.position;
	}

	Vector3 rotateToward ;
	public void Awake()
	{
		rotateToward = Global.GetRandomDirectionV3();
		if ( sprite == null )
			sprite = GetComponentInChildren<SpriteRenderer>();
	}

	public void Update()
	{
		if( state == State.Move || state == State.Exit )
			UpdateWindTest();
	}

	Vector2 velocity;
	float rotateVel;

	public void Enter(Vector3 pos )
	{
		gameObject.SetActive(true);
		transform.localScale = Random.Range( scaleRange.min , scaleRange.max ) * Vector3.one;
		StartCoroutine(DoEnter(pos));

		velocity = Vector2.zero;
		rotateVel = 0;

	}
	IEnumerator DoEnter(Vector3 pos)
	{
		// transform.localScale = Vector3.zero;
		transform.rotation = Random.rotation;
		sprite.DOFade(0,0);

		yield return new WaitForSeconds( Random.Range( 0, fadeTimeDiff));

		state = State.Enter;

		BeginMove();
		sprite.DOFade( 1f , fadeTime );
		transform.position = pos;

//		float scale = ( wind.GetSize().x + wind.GetSize().y ) * Random.Range( 1f , 2f );
//		Vector3 oriPos = ( pos - wind.transform.position ).normalized * scale;
//		oriPos.z = -20f;
//
//		transform.position = oriPos;
//		transform.localScale = Vector3.one;
//		float delay = Random.Range( 0 , fadeTimeDiff );
//		transform.DOMove( pos , fadeTime ).OnComplete(BeginMove).SetEase(Ease.OutExpo);
//		transform.DOScale( scale / 2f , fadeTime ).From().SetEase(Ease.OutExpo);

	}

	void BeginMove()
	{
		state = State.Move;
		windSensablParameter.shouldUpdate = true;
	}
		

	public void Exit( bool isReset )
	{
		if ( gameObject.activeSelf && state == State.Move )
			StartCoroutine(DoExit(isReset));
	}

	IEnumerator DoExit( bool isReset )
	{
		state = State.Exit;
		yield return new WaitForSeconds( Random.Range( 0, fadeTimeDiff));

		if ( sprite != null && sprite.gameObject.activeSelf )
		{
			if ( isReset )
				sprite.DOFade( 0 , fadeTime ).OnComplete( Reset );
			else
				sprite.DOFade( 0 , fadeTime ).OnComplete( RealExit2 );
		}else
		{
			if ( isReset)
				Reset();
			else
				RealExit2();
		}

//		float scale = ( wind.GetSize().x + wind.GetSize().y ) * Random.Range( 1f , 2f );
//		Vector3 fadePos = ( transform.position - wind.transform.position ).normalized * scale;
//		fadePos.z = -20f;
//		float delay = Random.Range( 0 , fadeTimeDiff );
//		transform.DOMove( fadePos , fadeTime ).OnComplete(RealExit).SetEase(Ease.InExpo);
//
//		transform.DOScale( scale / 2f , fadeTime ).SetEase(Ease.InExpo);
	}

	void RealExit2()
	{
		state = State.Invisible;
		windSensablParameter.shouldUpdate = false;
		gameObject.SetActive(false);
	}

	void RealExit()
	{
		state = State.Invisible;
		windSensablParameter.shouldUpdate = false;
		gameObject.SetActive(false);
	}


	void UpdateWindTest()
	{
		// update velocity
		velocity += windVelocity * WindVelSense * Time.deltaTime * 30f * LogicManager.PhysTimeRate ;
		velocity = Vector2.ClampMagnitude( velocity , maxVel * ( 1f + windVelocity.magnitude / 2f ) );
		velocity *= 0.98f;

		velocity = Mathf.Clamp( velocity.magnitude , velocityRange.min , velocityRange.max ) * velocity.normalized;

		rotateVel += Vector3.Cross(transform.up, windVelocity * WindRotateSense ).z * Time.deltaTime * 30f * LogicManager.PhysTimeRate;
		rotateVel = Mathf.Clamp(rotateVel, maxRotVel, -maxRotVel);
		rotateVel *= 0.98f;

		// update position & rotation
		Vector3 pos = transform.localPosition;
		pos += Global.V2ToV3( velocity ) * Time.deltaTime * LogicManager.PhysTimeRate;

		if ( isOutOfWind() )
		{
			Exit(true);
		}

		if ( velocity.magnitude < 0.05f )
		{
			Exit(true);
		}
		
//		if ( wind == null )
//		{
//			wind = GetComponentInParent<WindAdv>();
//		}
//			
//		if ( wind != null )
//		{
//			if ( pos.x > wind.GetSize().x / 2 + 0.1f )
//			{
//				pos.x -= wind.GetSize().x ;
//				pos.y = Random.Range( - wind.GetSize().y / 2 , wind.GetSize().y / 2 );
//			}
//			if ( pos.x < - wind.GetSize().x / 2 - 0.1f ) {
//				pos.x += wind.GetSize().x ;
//				pos.y = Random.Range( - wind.GetSize().y / 2 , wind.GetSize().y / 2 );
//			}
//			if ( pos.y > wind.GetSize().y / 2 + 0.1f)
//			{
//				pos.y -= wind.GetSize().y ;
//				pos.x = Random.Range( - wind.GetSize().x / 2 , wind.GetSize().x / 2 );
//
//			}
//			if ( pos.y < - wind.GetSize().y / 2 - 0.1f )
//			{
//				pos.y += wind.GetSize().y ;
//				pos.x = Random.Range( - wind.GetSize().x / 2 , wind.GetSize().x / 2 );
//			}
//		}

		pos.z = Global.WIND_UI_Z;
		transform.localPosition = pos;
		model.transform.Rotate( rotateToward * rotateVel * Time.deltaTime * 30f * LogicManager.PhysTimeRate );

	}

	bool isOutOfWind()
	{
		Vector3 pos = transform.position;
		Vector3 windPos = wind.transform.position;
		return ( pos.x > windPos.x + wind.GetSize().x / 2f - 0.1f ) || ( pos.x < windPos.x - wind.GetSize().x / 2f + 0.1f )
			|| ( pos.y > windPos.y + wind.GetSize().y / 2f - 0.1f) || ( pos.y < windPos.y -  wind.GetSize().y/ 2f + 0.1f);
	}

	void OnCollisionEnter(Collision col)
	{
		Debug.Log("WindTest Collision " + col.collider.tag);
		if ( col.collider.tag == "Land")
		{
			GetComponent<Collider>().enabled = false;
			sprite.DOFade( 0 , 1f ).OnComplete(Reset);
		}
	}

	public void Reset()
	{
		parent.ResetWindTest( this );
	}


	public void EnterRandomPosByWind()
	{
		GetComponent<Collider>().enabled = true;
		Vector3 Size = wind.GetSize();
		int k = 0;
		Vector3 ranPos = new Vector3 ( Random.Range( - Size.x / 2 , Size.x / 2 ) , Random.Range( - Size.y / 2 , Size.y / 2 ) , 0 ) + wind.transform.position;
		while( !wind.checkIsObsticle(ranPos) &&  k < 2000 ) 
		{
			ranPos = new Vector3 ( Random.Range( - Size.x / 2 , Size.x / 2 ) , Random.Range( - Size.y / 2 , Size.y / 2 ) , 0 ) + wind.transform.position;
			++k;
		}
		Enter(ranPos);
	}
}
