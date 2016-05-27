using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Flower3D : Flower , WindSensable {

	[SerializeField] Transform[] stems;
	[SerializeField] GameObject LeafPrefab;
	[SerializeField] GameObject FlowerPetalPrefab;
	[SerializeField] GameObject FlowerPetalFinalPrefab;
	[SerializeField] float growDelay = 1f; 
	[SerializeField] Petal3D[] flowerPetals;
	[SerializeField] float endBlowVelocity = 10f;
	[SerializeField] float minBlowVelocity = 0.3f;
	[SerializeField] AudioSource blowSound;
	[SerializeField] AudioSource flowerGrowSound;
	[SerializeField] AudioSource flowerFlyAwaySound;
	[SerializeField] AudioSource petalGrowSound;
	[SerializeField] AudioSource stemGrowSound;
	[SerializeField] FlowerTopSenseGuesture flowerTopSenseGuesture;


	BoxCollider m_Collider;

	[SerializeField] protected Grow3DParameter grow3DPara;
	[SerializeField] protected Perish3DParameter perish3DPara;

	List<GameObject> leafList = new List<GameObject>();

	[System.SerializableAttribute]
	public struct Grow3DParameter
	{
		public  MaxMin stemScaleRange;
		public  MaxMin stemRotateAngleRange;
        public  float stemWindAffect;

		public  MaxMin leafScaleRange;
		public  MaxMin leafRotateAngleRange;
		public  MaxMin leafPositionYRange;
        public  MaxMin leafNumberRange;

        public float stemWindSwind;

		public int flowerPetalNumber;
		public float flowerPetalUnlinkInterval;
	};

	[System.SerializableAttribute]
	public struct Perish3DParameter
	{
		public MaxMin perishDelay;
		public MaxMin perishTime;
		public MaxMin leafRotateAngle;
		public MaxMin leafPerishDelay;
		public MaxMin leafPerishMoveY;
		public MaxMin stemRotateAngle;
	};

    // public override void Init() {
    //     base.Init();

    //     Petal[] ps = petalRoot.GetComponentsInChildren<Petal3D>();
    //     petals.AddRange(ps);
    //     foreach(Petal p in petals)
    //     {
    //         p.gameObject.SetActive(false);
    //         p.state = PetalState.Link;
    //     }
    // }

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.LevelDead, OnLevelDead);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.LevelDead, OnLevelDead);
	}

	void OnLevelDead( Message msg )
	{
		Sequence seq = DOTween.Sequence();
		seq.AppendCallback( PerishPre );
		seq.AppendInterval( Random.Range( perish3DPara.perishDelay.min , perish3DPara.perishDelay.max ));
		seq.AppendCallback( Perish);
		
	}

	void OnEndLevel(Message msg )
	{
		StartCoroutine(BlowAll());
	}

	IEnumerator BlowAll()
	{

//		if ( petals != null && petals.Count > 0 )
//		{
//			for ( int i = 0 ; i < petals.Count ; ++ i )
//			{
//				int iPetal = i % petals.Count;
//				if ( petals[iPetal].state == PetalState.Link ) {
//					petals[iPetal].Blow( Vector2.up * 2f  + Global.GetRandomDirection() , Random.Range( 0 , endBlowVelocity ) , Petal.BlowType.Final );
//				}
//			}
//		}

		if ( flowerPetals != null && flowerPetals.Length > 0 )
		{
			for ( int i = 0 ; i < flowerPetals.Length ; ++ i )
			{
				int iFlower = i % flowerPetals.Length ;
				if ( flowerPetals[iFlower].state == PetalState.Link ) {
					flowerPetals[iFlower].Blow( Vector2.up * 1f  + Global.GetRandomDirection() , Random.Range( 0 , endBlowVelocity ) , Petal.BlowType.Normal );
				}

				yield return new WaitForSeconds( grow3DPara.flowerPetalUnlinkInterval );
			}
				
		}

	}
		
	public override void Grow () {

        StartCoroutine(GrowAfterSetUp());
    }

    IEnumerator GrowAfterSetUp()
    {
		if ( stemGrowSound != null )
		{
			stemGrowSound.pitch = LogicManager.AnimTimeRate;
			stemGrowSound.Play();
		}

		if ( m_Collider == null )
		{
			m_Collider = GetComponent<BoxCollider>();
		}
		m_Collider.isTrigger = true;

        transform.localScale /= 1000f;
        float timer = 0;
		while( timer < Mathf.Min(0.1f , growDelay) )
        {
			timer += Time.deltaTime * LogicManager.AnimTimeRate;
            yield return null;
        }

        transform.localScale *= 1000f;

        // init the stems
		// to record the grow time of stem
		float stemGrowTime = 0;
		//TODO set the angle according to wind
		float stemRotateAngle = Random.Range( grow3DPara.stemRotateAngleRange.min , grow3DPara.stemRotateAngleRange.max);

        if (windVelocity != Vector2.zero)
        {
         stemRotateAngle = Vector2.Angle(Vector2.up, Vector2.up + grow3DPara.stemWindAffect * windVelocity) * Vector2.Dot( Vector2.left , windVelocity)
         + Random.Range( grow3DPara.stemRotateAngleRange.min , grow3DPara.stemRotateAngleRange.max);
    	} 
        for(int i = 0 ; i < stems.Length ; i++)
    	{
    		Transform s = stems[i];

			if ( growParameter.petalType == PetalType.Final )
			{
				SpriteRenderer sprite = s.GetComponent<SpriteRenderer>();
				Material m = new Material( sprite.material.shader );
				Color fromColor = Color.Lerp( Color.black , Color.white , 1.0f * i / stems.Length );
				Color toColor = Color.Lerp( Color.black , Color.white , 1.0f * ( i + 1 ) / stems.Length );
//				Debug.Log("From " + fromColor + " to " + toColor );
				m.SetColor("_ColorFirst" , fromColor );
				m.SetColor("_ColorSecond" , toColor );
				m.SetFloat("_ChangeRate" , 0.5f );
				m.SetFloat("_ChangeRange" , 1.0f );
				m.name = "Stem" + i;
				sprite.material = m;
//				Color col = Color.white;
//				col.a = sprite.color.a;
//				sprite.color = col;
//				sprite.color = Color.white;
			}

            // set the scale(start from V3(1,0,1))
    		Vector3 scale = s.localScale;
    		float scaleY = 1f;
			if (i != 0 ) scaleY = Global.GetRandomMinMax( grow3DPara.stemScaleRange );
    		scale.y *= 0.001f * scaleY;
            s.localScale = scale;

            // set the rotation
            // each of the stem would rotate a little bit
    		s.Rotate(new Vector3(0,0, stemRotateAngle));


            // set the grow animation
			float myGrowTime = growParameter.growTime/stems.Length * scaleY / 0.7f / LogicManager.AnimTimeRate;
    		if (i == stems.Length - 1 )
	    		s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(stemGrowTime).SetEase(Ease.Linear).OnComplete(GrowStemComplete);
			else
				s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(stemGrowTime).SetEase(Ease.Linear);

            // update the grow time
			stemGrowTime += myGrowTime ;
    	}

        // init the leafs
    	int leafs = Random.Range( (int)grow3DPara.leafNumberRange.min, (int)grow3DPara.leafNumberRange.max);
    	for( int k = 0 ; k < leafs; ++k )
    	{
            // create the leaf
    		GameObject leaf = Instantiate(LeafPrefab) as GameObject;
    		SpriteRenderer sprite = leaf.GetComponent<SpriteRenderer>();

    		leaf.transform.parent = stems[0];
    		leaf.transform.localPosition = new Vector3(0,Random.Range(grow3DPara.leafPositionYRange.min, grow3DPara.leafPositionYRange.max),0);
    		
            // set the scale(a random value)
            float scale = Random.Range(grow3DPara.leafScaleRange.min, grow3DPara.leafScaleRange.max) ;
    		leaf.transform.localScale = Vector3.one * scale / stems[0].localScale.x;
            
            // set the rotation ( a random value)
            float angleRange = ( - grow3DPara.leafRotateAngleRange.min + grow3DPara.leafRotateAngleRange.max) ;
            float randomeAngle = grow3DPara.leafRotateAngleRange.min + k * angleRange / leafs  + Random.Range(0, angleRange / (leafs-1) );
    		leaf.transform.Rotate(new Vector3(0,0, randomeAngle));
			leaf.GetComponent<FollowWind>().Init();
    		
    		// set the scale animation
    		// make all the leaf grow in same speed
    		// and make the bigger leaf grow earlier
 			float process = (scale - grow3DPara.leafScaleRange.min + 0.2f) / ( grow3DPara.leafScaleRange.max - grow3DPara.leafScaleRange.min);
			float growTime = process * growParameter.growTime / grow3DPara.leafScaleRange.max / LogicManager.AnimTimeRate;
            growTime = Mathf.Clamp(growTime, 0.5f, 3f);
    		leaf.transform.DOScale( 0.01f , growTime  )
				.SetDelay( (1 - scale / grow3DPara.leafScaleRange.max ) * growParameter.growTime / LogicManager.AnimTimeRate )
    		.From();
            sprite.DOFade(0, growTime / 2).SetDelay( (1 - scale / grow3DPara.leafScaleRange.max ) * growParameter.growTime ).From();

			leafList.Add( leaf );
    	}
    }

    void GrowStemComplete()
    {
		Vector3 rootLS = petalRoot.transform.localScale;
		Vector3 rootGS = petalRoot.transform.lossyScale / transform.lossyScale.x ;
		petalRoot.transform.localScale = new Vector3( rootLS.x / rootGS.x , rootLS.y / rootGS.y , rootLS.z / rootGS.z);


		GrowFlowerPatel();
    }

	void GrowFlowerPatel()
	{
		StartCoroutine(GrowFlowerPatalCor(grow3DPara.flowerPetalNumber));
	}

	IEnumerator GrowFlowerPatalCor(int num)
	{
		flowerPetals = new Petal3D[num];

		if ( flowerGrowSound != null )
		{
			flowerGrowSound.pitch = LogicManager.AnimTimeRate;
			flowerGrowSound.Play();
		}
		float maxGrowTime = 0.5f ;
		for(int i = 0 ; i < num ; ++ i )
		{
			GameObject prefab = (growParameter.petalType == PetalType.Final )? FlowerPetalFinalPrefab : FlowerPetalPrefab;
			GameObject fPetalObj = Instantiate( prefab ) as GameObject;
			fPetalObj.transform.SetParent( petalRoot );
			fPetalObj.transform.localPosition = Vector3.zero;
			fPetalObj.transform.localScale = Vector3.one;

			flowerPetals[i] = fPetalObj.GetComponent<Petal3D>();
			if ( flowerPetals[i] != null )
			{
				flowerPetals[i].Init( this , i ) ;
			}
				
			maxGrowTime = Mathf.Max( maxGrowTime , flowerPetals[i].GetGrowTime() );
		}
		yield return new WaitForSeconds(maxGrowTime / LogicManager.AnimTimeRate);

		if ( growParameter.petalType == PetalType.Final ) 
		{
			Message msg = new Message();
			msg.AddMessage( "flower" , this );
			Debug.Log("Grow Final Flower");
			EventManager.Instance.PostEvent(EventDefine.GrowFinalFlower , msg );
			EventManager.Instance.PostEvent(EventDefine.BloomFlower , new Message() , this );
			yield break;
		}else
		{
			if ( flowerFlyAwaySound != null )
			{
				flowerFlyAwaySound.pitch = LogicManager.AnimTimeRate;
				flowerFlyAwaySound.Play();
			}

			for ( int i = 0 ; i < flowerPetals.Length ; ++ i )
			{
				flowerPetals[i].Blow( Vector2.right + 0.4f * Global.GetRandomDirection().normalized , 0 , Petal.BlowType.FlyAway );
				yield return new WaitForSeconds(grow3DPara.flowerPetalUnlinkInterval / LogicManager.AnimTimeRate );
			}

			if ( petalGrowSound != null )
			{
				petalGrowSound.pitch = LogicManager.AnimTimeRate;
				petalGrowSound.Play();
			}
			EventManager.Instance.PostEvent(EventDefine.BloomFlower , new Message() , this );
			StartCoroutine(GrowPetalCor());

		}

	}

	public void BlowFlowerPetals()
	{
		StartCoroutine(BlowFlowerPetalsCor());
	}

	IEnumerator BlowFlowerPetalsCor()
	{

		for ( int i = 0 ; i < flowerPetals.Length ; ++ i )
		{
			flowerPetals[i].Blow( Global.GetRandomDirection() , 0 , Petal.BlowType.FlyAway );
			yield return new WaitForSeconds(grow3DPara.flowerPetalUnlinkInterval / LogicManager.AnimTimeRate);
		}
		yield break;
	}

	protected bool canBlow ( Vector2 velocity)
	{
		if ( velocity.magnitude < minBlowVelocity )
			return false;
		if ( petals.Count < petalNum - 1 )
			return false;
		Petal petal = petals[0];
		if ( petal != null && Time.time - initPetalTime < petal.GetGrowTime())
			return false;
		
		return base.canBlow();
	}

	public override void Blow (Vector2 velocity)
	{
		if ( canBlow( velocity ) )
		{
			if ( blowSound != null )
			{
				blowSound.Play();
				blowSound.volume = velocity.magnitude / 15f ;
			}
			base.Blow (velocity);


			// make the flower react to the blow
			FollowWind stemFollowWind = stems[0].GetComponent<FollowWind>();
			if ( stemFollowWind != null )
			{
				stemFollowWind.AddImpuse( velocity , petalRoot.position );
			}
		}
	}


	public void Perish()
	{
		float perishTime = Random.Range( perish3DPara.perishTime.min , perish3DPara.perishTime.max );

		for( int i = 0 ; i < 1 ; ++ i )
		{
			stems[i].transform.DOScale( 0 , perishTime ).SetDelay( Random.Range( 0, 1f ));
			if ( stems[i].GetComponent<FollowWind>() != null )
				stems[i].GetComponent<FollowWind>().enabled = false;
		}

		for( int i = 0 ; i < leafList.Count ; ++ i )
		{
			leafList[i].transform.DOScale( 0 , perishTime ).SetDelay( Random.Range( 0, 1f ));
			if ( leafList[i].GetComponent<FollowWind>() != null )
				leafList[i].GetComponent<FollowWind>().enabled = false;
			if ( leafList[i].GetComponent<SpriteRenderer>() != null )
				leafList[i].GetComponent<SpriteRenderer>().DOFade( 0 , perishTime );
		}


	}

	public void PerishPre()
	{
		float stemRotateAngle = Random.Range( perish3DPara.stemRotateAngle.min , perish3DPara.stemRotateAngle.max ) * ( ( Random.Range( -1f , 1f ) < 0) ? -1f : 1f );

		float perishTime = Random.Range( perish3DPara.perishTime.min , perish3DPara.perishTime.max );

		for( int i = 0 ; i < stems.Length ; ++ i )
		{
			stems[i].transform.DOLocalRotate( new Vector3( 0 , 0 , stemRotateAngle ) , perishTime ).SetRelative(true).SetDelay( Random.Range( 0,1f));
		}

		for( int i = 0 ; i < leafList.Count ; ++ i )
		{
			float leafRotateAngle = Random.Range( perish3DPara.leafRotateAngle.min , perish3DPara.leafRotateAngle.max ) * ( ( Random.Range( -1f , 1f ) < 0) ? -1f : 1f );	
			leafList[i].transform.DOLocalRotate(new Vector3( 0 , 0 , leafRotateAngle ) , perishTime ).SetRelative(true).SetDelay( Random.Range( 0,1f));
			leafList[i].transform.DOLocalMoveY( Random.Range( perish3DPara.leafPerishMoveY.min ,perish3DPara.leafPerishMoveY.max ) , perishTime )
				.SetRelative(true)
				.SetDelay( Random.Range( perish3DPara.leafPerishDelay.min ,perish3DPara.leafPerishDelay.max ));
		}
	}

	public int GetFlowerNum()
	{
		return grow3DPara.flowerPetalNumber;
	}


    Vector2 windVelocity;

    public void SenseWind(Vector2 velocity)
    {
        windVelocity = velocity;
    }

    public void ExitWind(){
        windVelocity = Vector2.zero;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    [SerializeField] WindSensablParameter windSensablParameter;

    public WindSensablParameter getParameter()
    {
        return windSensablParameter;
    }

    void OnBecameVisible()
    {
        windSensablParameter.shouldUpdate = true;
        enabled = true;
    }

    void OnBecameInvisible ()
    {
        windSensablParameter.shouldUpdate = false;
        enabled = false;
    }

}
