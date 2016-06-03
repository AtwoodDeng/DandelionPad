using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FollowWind : MonoBehaviour , WindSensable {

    // Wind3D tempWind = null;
    [SerializeField] protected float swind; // sense of wind
    [SerializeField] protected float K;     // 
    [SerializeField] protected float drag=1f;
    [SerializeField] protected float initTime = 0.1f;
	[SerializeField] protected float senseImpuse;
	[SerializeField] MaxMin swingRange;
	[SerializeField] protected float controlDrag;
	[SerializeField] float volumeSense = 1f;
	[SerializeField] AudioSource touchSound;
	[SerializeField] AudioSource blowSound;

    protected Vector2 windVelocity = Vector2.zero;

    public void SenseWind(Vector2 velocity)
    {
        windVelocity = velocity;
    }

    public void ExitWind(){
        windVelocity = Vector2.zero;
    }

    public Vector3 getPosition() {
        return transform.position;
    }
    
    [SerializeField] public WindSensablParameter windSensablParameter;

    public WindSensablParameter getParameter()
    {
        return windSensablParameter;
    }

	virtual public void AddImpuse( float intense , Vector3 position )
	{
		Vector3 radius = position - transform.position;
		radius.z = 0;
		float torque = radius.magnitude * intense;
		torque *= senseImpuse;

		if ( touchSound != null && !touchSound.isPlaying )
		{
			touchSound.volume = torque / senseImpuse * volumeSense;
			touchSound.Play(); 
		}

		angleVol += torque;
	}

	virtual public void AddImpuse(Vector3 impuse , Vector3 position )
	{
		Vector3 m_impuse = impuse;
		m_impuse.z = 0 ;
		Vector3 radius = position - transform.position;
		radius.z = 0;
		float torque = 1e-2f * Vector3.Dot( Vector3.back , Vector3.Cross( radius , m_impuse ));
		torque *= senseImpuse;

		if ( blowSound != null )
		{
			blowSound.volume = torque / senseImpuse;
			blowSound.Play(); 
		}

		angleVol += torque;
	}

	virtual public void AttachVelocity( Vector2 velocity , Vector3 position , float rate = 0.1f )
	{
		
		Vector3 toward = position - transform.position;
		Vector2 towrd2D = Global.V3ToV2( toward );
		Vector2 velT = velocity - towrd2D.normalized * Vector2.Dot( velocity , towrd2D.normalized );
		Vector3 velPos = Vector3.Cross( toward , Vector3.forward ) .normalized;

		float toVel = Vector2.Dot( velT , Global.V3ToV2( velPos) ) / towrd2D.magnitude * Mathf.Rad2Deg;

		if (IsOutOfSwingRange(toVel) )
			return;
		
		angleVol = Mathf.Lerp( angleVol , toVel ,rate  );
	}
	static int followSoundPlayed = 0;
	public void PlayTouchSound()
	{
		if ( touchSound != null && followSoundPlayed <= 4  )
		{
			touchSound.volume = Mathf.Clamp01( Mathf.Abs( angleVol ) / 10f ) * 0.4f ; 
//			touchSound.time = Random.Range( 0.1f , touchSound.clip.length - 1f );
//			touchSound.volume = 0;
//			Sequence seq = DOTween.Sequence();
//			seq.Append( touchSound.DOFade( Mathf.Clamp01( Mathf.Abs( angleVol ) / 10f ) * 0.4f , 0.05f ) );
//			seq.AppendInterval( 0.5f );
//			seq.Append( touchSound.DOFade( 0 , 0.5f ));
			touchSound.Play();
			followSoundPlayed += 1;
			StartCoroutine( endPly(0.5f));
		}
	}

	IEnumerator endPly(float delay)
	{
		yield return new WaitForSeconds(delay);
		followSoundPlayed = -1;
	}

    void Awake()
    {
        // windSensablParameter.onRender = true;
    	// StartCoroutine(UpdateAnimation());
    }

	void Start()
	{
		Init();
	}
    float timer = 0;

    bool isInit = false;
    float initAngle = 0;
    float angleVol = 0;

    void LateUpdate()
    {
    	timer += Time.deltaTime;
	    if (timer < initTime )
	    	return;

	    if (!isInit)
	    {
            Init();
        }
		UpdateObject(Time.deltaTime);
    }

	virtual public void Init()
    {
            initAngle = Global.StandardizeAngle( transform.eulerAngles.z );
            angleVol = 0;
            isInit = true;
    }

	virtual protected void UpdateObject(float dt)
    {
		float edt = dt * LogicManager.PhysTimeRate;
        float windForce = 0;

        //test if in the wind
        if ( windVelocity != Vector2.zero)
        {
       	 	Vector3 windDirect = Global.V2ToV3(windVelocity.normalized);
       		float windEffect = Vector3.Dot(Vector3.back , Vector3.Cross(transform.up, windDirect) );
       		windForce = windEffect * swind * windVelocity.magnitude;
        }

        // do the animation of stem
        
        float offSetAngle = Mathf.DeltaAngle(transform.eulerAngles.z, initAngle);

        float KForce = - offSetAngle * K;

		angleVol += ( windForce + KForce ) * edt * 30f;
		if ( Mathf.Abs( offSetAngle ) > swingRange.max / 3f )
			angleVol *= drag;

		if (  IsOutOfSwingRange(angleVol) )
		{
			angleVol *= controlDrag;
		}

        //Rotate the object
		transform.Rotate(Vector3.back, angleVol * edt * 30f );

    }

	bool IsOutOfSwingRange(float vel)
	{
		float offSetAngle = Mathf.DeltaAngle(transform.eulerAngles.z, initAngle);
		return (swingRange.max > 0 && offSetAngle > swingRange.max && vel > 0)
			||  (swingRange.min < 0 && offSetAngle < swingRange.min && vel < 0 );
	}

    void OnBecameVisible()
    {
        windSensablParameter.shouldUpdate = true;
        enabled = true;
    }

    void OnBecameInvisible ()
    {
        // windSensablParameter.onRender = false;
        enabled = false;
    }

}
