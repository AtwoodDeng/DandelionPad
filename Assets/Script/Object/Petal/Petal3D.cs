using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Petal3D : Petal {

    [SerializeField] float mass=12f;
    [SerializeField] float rotationMass=0.001f;
    [SerializeField] float blowIntense=0.33f;
    [SerializeField] float scaleIntense = 0.01f;
    [SerializeField] float drag = 0.08f;
    [SerializeField] float rotationDrag = 0.05f;
    [SerializeField] float gravity = 0.003f;
    [SerializeField] float maxVel = 0.7f;
    [SerializeField] float maxRotVel = 0.66f;
    [SerializeField] float maxScale = 2f;
    [SerializeField] MaxMin BlowImpulseRange;

    [SerializeField] Transform petalModel;
    [SerializeField] float petalModelRotateIntense = 0.01f;
    [SerializeField] float petalModelLinkIntense = 0.01f;
    [SerializeField] float petalModelLinkInterval = 2f;
    [SerializeField] float upForceIntense = 0.1f;

	[SerializeField] MaxMin growTime;
	[SerializeField] bool isFlowerPetal = false;
	[SerializeField] Vector3 growScale;

    [SerializeField] Material shareMaterial;

    [SerializeField] MaxMin flyAwayFadeTime;
	[SerializeField] float normalFadeTime;
    float petalModelLinkInit;
    Vector3 petalModelRotateToward;

    [SerializeField] Petal3DWind follow;

    Vector3 myUpDiff;

	bool isInited = false;


	void Start()
	{
		if ( state == PetalState.Init )
		{
			Init(null,0);
			// Blow( Global.GetRandomDirection() , 0 , BlowType.Normal );
			myRotationVelocity = maxRotVel * .9f;
		}
	}

	public override void Init (Flower _flower, int index) {

		if ( isInited )
			return;
		isInited = true;

        base.Init(_flower, index);

        // find component
        if ( follow == null )
            follow = GetComponentInChildren<Petal3DWind>();

        myUpDiff = Global.GetRandomDirectionV3() * 0.1f;
       // chaosFunction = StartCoroutine(GenerateChaos(0.1f));

        petalModelRotateToward = Global.GetRandomDirectionV3();

        // the initial phase position of the petal
        petalModelLinkInit = Random.Range(0,Mathf.PI * 2f);
        // randomize some parameter
        petalModelRotateIntense *= Random.Range(0.5f, 2f);
        petalModelLinkIntense *= Random.Range(0.1f, 5f);
        petalModelLinkInterval *= Random.Range(0.5f, 2f);

        m_rigidbody = GetComponent<Rigidbody2D>();

        // the up force intense is different because of diff shape
        upForceIntense *= Random.Range(0.5f, 2f);

		if ( state == PetalState.Init )
		{
			SetColliderTrigger( false );
		}

		if ( state == PetalState.Link )
		{
			SetColliderTrigger( true );
		}

     }

	float m_grwoDuration = 0;

    override public void Grow( int index )
    {
		m_grwoDuration = Random.Range(growTime.min, growTime.max);
		if ( isFlowerPetal )
		{
			float scaleTime = GetGrowTime() * .6f + Random.Range( -0.1f , 0.1f ) ;
			float bloomTime = GetGrowTime() * .4f + Random.Range( -0.1f , 0.1f ) ;
			if ( flower != null )
			{
				Flower3D f3D = (Flower3D) flower;
				Vector3 randomAngle = new Vector3(Random.Range(- growScale.x , growScale.x ), Random.Range(-growScale.y, growScale.y), Random.Range(-growScale.z, growScale.z));

				randomAngle.y = 360f * index / f3D.GetFlowerNum() * 2f  + Random.Range(-7f , 7f );
				petalModel.transform.localEulerAngles = randomAngle;

				randomAngle.z += -75f + Random.Range( - 8f , 8f );

				if ( index >= f3D.GetFlowerNum() / 2 ) {
					randomAngle.z += 30f;
					transform.localScale *= 0.66f;
				}
				petalModel.DORotate( randomAngle , bloomTime ).SetDelay( scaleTime - 0.2f ) ;
			}
//			SpriteRenderer sprite = petalModel.GetComponentInChildren<SpriteRenderer>();
//			if ( sprite != null )
//			{
//				sprite.DOFade( 0 , duration ).From();
//			}

			petalModel.transform.DOScale(Vector3.zero, scaleTime ).From().SetEase(Ease.InCubic);
		}
		else {
			// transform.localRotation = Quaternion.EulerAngles(new Vector3(0,0, 2f * Mathf.PI * index / flower.petalNum));
			transform.rotation = Quaternion.Euler( new Vector3( 0 , Random.Range( 0 , 360) , 80f ) );

			Vector3 randomAngle = new Vector3(Random.Range(- growScale.x , growScale.x ), Random.Range(-growScale.y, growScale.y), Random.Range(-growScale.z, growScale.z));

			Vector3 scale = petalModel.transform.localScale;
			petalModel.transform.localScale = scale * 0.01f;
			petalModel.transform.DOScale(scale , GetGrowTime() );

			petalModel.DORotate( randomAngle , GetGrowTime() );
		}
    }

    public override void AddForce (Vector2 force) {
        if ( state.Equals(PetalState.Fly) || state.Equals(PetalState.FlyAway)) {
            myForce += force;
        }
    }

    public void AddImpluse(Vector2 force) {
        Vector2 tem = myForce;
        myForce = force;
        UpdateVelocity( 1f );

        myForce = tem;

    }

	public void LinkVelocity( Vector2 vel )
	{
		myVelocity = vel;
		UpdateVelocity(0);
	}


    
    public override void Blow (Vector2 vel , BlowType blowType = BlowType.Normal) {
		base.Blow(vel , blowType );

//		Vector3 blowDirection = move;
//        float blowVelocity = vel * Random.Range(0.75f , 1.25f);
//		Vector3 blowImpulse = ( blowDirection * blowVelocity * blowIntense * 0.001f  );
//		blowImpulse = Vector3.ClampMagnitude(blowImpulse, BlowImpulseRange.max);
//
//
//        if ( blowImpulse.magnitude < BlowImpulseRange.min )
//            blowImpulse = blowImpulse.normalized * BlowImpulseRange.min;
//		AddImpluse( blowImpulse );

        
        if ( blowType.Equals(BlowType.Normal) )
        {
			SetColliderTrigger( false );
			if ( !isFlowerPetal )
			{
				petalModel.transform.DOScale( 0 , 0.5f ).SetDelay(normalFadeTime).OnComplete(SelfDestory);
			}
        }
        else if (blowType.Equals(BlowType.FlyAway))
        {
            float fadeTime = Random.Range( flyAwayFadeTime.min , flyAwayFadeTime.max);
			// set different animation for flower petal/ non-flower petal
			// flower petal should have a longer animation
			if ( isFlowerPetal )
				petalModel.transform.DOScale(0, fadeTime).OnComplete( SelfDestory ).SetEase(Ease.InExpo );
			else
				petalModel.transform.DOScale(0, fadeTime).OnComplete( SelfDestory ).SetEase(Ease.OutCubic );
//			if ( isFlowerPetal )
//			{
//				SpriteRenderer render = petalModel.GetComponentInChildren<SpriteRenderer>();
//				render.DOFade( 0 , fadeTime );
			//			}
        }

		// set parent to level
		transform.SetParent( LogicManager.LevelManager.GetLevelObject().transform );

		// update the velocity
		LinkVelocity( vel * blowIntense );

		// link to the wind 
        follow.windSensablParameter.shouldUpdate = true;

    }

    //Chaos

    void LateUpdate()
    {
        UpdateVelocity(Time.deltaTime);
        UpdatePosition(Time.deltaTime);

        ResetForce();
    }

    Vector2 myVelocity = Vector2.zero;
    float myRotationVelocity = 0;
    Vector2 myForce = Vector2.zero;
    float scaleVol = 0;
    float myScale = 1f;

	Rigidbody2D m_rigidbody;

    void ResetForce()
    {
        // set force back to zero
        myForce = Vector2.zero;
    }

    protected void UpdateVelocity( float dt )
    {
        // add chaos to force
        // myForce += chaosForce;

		float edt = dt * LogicManager.PhysTimeRate;

        //do nothing if the petal is still link to the flower
		if (state == PetalState.Link || state == PetalState.Land)
        {
            myVelocity = Vector3.zero;
            myRotationVelocity = 0;
			petalModel.Rotate( petalModelRotateToward * petalModelLinkIntense * Mathf.Sin( petalModelLinkInterval * Time.time + petalModelLinkInit) * edt * 30f );
        }

		else if (state == PetalState.Fly || state == PetalState.FlyAway || state == PetalState.Init)
        {
            // update the velocity
            Vector2 _force = myForce / mass - new Vector2(0,gravity);
            // up force
            _force += Mathf.Pow( myVelocity.x , 2f ) * upForceIntense * Vector2.up ;
            // drag force
            _force -= drag * myVelocity * Mathf.Pow( myVelocity.magnitude , 2f ) ;
 

			myVelocity += _force * edt / mass ;

            Vector3 myUp = transform.up + myUpDiff;
            
            //Rotate the petal
			if ( state != PetalState.Init )
            	myRotationVelocity += (  Vector3.Cross(myUp, _force).z 
				+ Vector3.Cross(- myUp , Vector3.down * rotationMass * 0.2f ).z ) / rotationMass * edt;

	         // control the velocity
	        myVelocity = Vector3.ClampMagnitude(myVelocity, maxVel);
            myRotationVelocity = Mathf.Clamp(myRotationVelocity, -maxRotVel, maxRotVel);

	        // drag the velocity
	        // myVelocity *= ( 1f - drag );
	        myRotationVelocity *= (1f - rotationDrag);
    	}

    	// rigidbody velocity don't effect;
    	if (m_rigidbody != null)	
 		   	m_rigidbody.velocity = Vector2.zero;
    }

	bool isOutOfFrame = false;
    void UpdatePosition(float dt )
    {
		float edt = dt * LogicManager.PhysTimeRate;
        // update the position and rotation
		transform.position += Global.V2ToV3(myVelocity) * edt ;
		transform.Rotate(Vector3.back, myRotationVelocity * edt ) ;

        petalModel.Rotate( petalModelRotateToward * myRotationVelocity * petalModelRotateIntense * edt * 30f);

        //change the scale(z distance)
		if ( state == PetalState.FlyAway || state == PetalState.FlyAway || state == PetalState.Init ) {
	        scaleVol += Vector3.Dot(petalModel.transform.up, Vector3.back) * scaleIntense;
	        myScale += scaleVol;
	        myScale = Mathf.Clamp(myScale, 1f/maxScale, maxScale);
		}

		// if out of the frame
		if ( state == PetalState.Fly || state == PetalState.FlyAway )
		if ( !CameraManager.Instance.ifInFrameWorld( transform.localPosition ) && !isOutOfFrame)
		{
			isOutOfFrame = true;
			if ( !destoryMessage.ContainMessage( "OutOfFrame" ))
				destoryMessage.AddMessage("OutOfFrame" , 1);
			transform.DOScale( 0 , 1f ).OnComplete(SelfDestory);
		}
    }

	override public float GetGrowTime()
	{
		return m_grwoDuration / LogicManager.AnimTimeRate;
	}

//    Vector2 chaosForce;
//    Coroutine chaosFunction;
//    // update the chaos(this will run forever)
//    IEnumerator GenerateChaos(float time)
//    {
//        float timer = 0;
//        chaosForce = Global.GetRandomDirection() * myForce.magnitude * Random.Range(0.1f, 1f);
//        while(true)
//        {
//            chaosForce = (chaosForce.normalized + Global.GetRandomDirection().normalized * 0.1f) * chaosForce.magnitude;
//
//            timer += Time.deltaTime;
//            if (timer > time)
//                break;
//            yield return null;
//        }
//        chaosFunction = StartCoroutine(GenerateChaos(Random.Range(.5f, .8f)));
//    }


	public override void OnLand (Vector3 point, Vector3 normal, GameObject obj)
	{
		if ( !isFlowerPetal )
			base.OnLand (point,normal,obj);
	}

    protected override void SelfDestory () {
		if ( state != PetalState.Dead)
		{
	        follow.windSensablParameter.shouldStore = false;
	        base.SelfDestory();
		}

        // if (chaosFunction != null)
        //     StopCoroutine(chaosFunction);

        // Renderer render = petalModel.GetComponent<Renderer>();
        // if (render != null)
        // {
        // 	render.material.DOFade(0, 1f).OnComplete(base.SelfDestory);
        // }
    }

}
