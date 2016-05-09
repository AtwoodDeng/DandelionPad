using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Land : MonoBehaviour {
	[SerializeField] float PosLayer = 0 ;
	float m_layer;
	Vector3 oriPosition;
//
//	[SerializeField] ComputeShader computeShader;
//	[SerializeField] Shader fadeOutShader;
//
//	[SerializeField] Texture2D InkTexture;
//	[SerializeField] Texture2D resultTexture;
//
//	Material m_Material;
//
//	static int ThreedNum = 32;
//	int m_nNumGroupsX = 1;
//	int m_nNumGroupsY = 1;
//	int kernelIndex = 0;
//
//	ComputeBuffer[] m_computeBuffer;
//	Texture m_Texture;
//	Texture tem_Texture;
//
//	void Awake()
//	{
//		m_Material = new Material(fadeOutShader);
//		m_Texture = new Texture2D( resultTexture.width , resultTexture.height );
//		tem_Texture = new Texture2D( resultTexture.width , resultTexture.height );
//
//		GetComponent<SpriteRenderer>().material = m_Material;
//		m_Material.mainTexture = m_Texture;
//
//		m_nNumGroupsX = resultTexture.width / ThreedNum;
//		m_nNumGroupsY = resultTexture.height / ThreedNum;
//	}
//
//	void Update()
//	{
//		m_Material.SetTexture("_MainTex" , m_Texture);
//	}
	[SerializeField] tk2dSprite thisSprite;
	[SerializeField] SpriteRenderer effectSprite;
	[SerializeField] GameObject grassPrefab;

	[System.Serializable]
	public struct EffectParameter
	{
		public Shader effectShader;
		public Texture2D resultTex;
		public Texture2D coverTex;
		public Color mainColor;
		public Vector4 coverInit;
		public Vector4 CoverMove;
		public float fadePosition;
		public float fadeRange;
		public AnimationCurve sizeCurve;
		public MaxMin delay;
		public MaxMin expand;
		public int dropNum;
		public float growTime;
	}
		

	[SerializeField] EffectParameter effectParameter;
	[SerializeField] Vector3 grassTopOffset;
	[SerializeField] Vector3 grassScale = Vector3.one;
	[SerializeField] bool enableGrowFlower = true;
	[SerializeField] float density = 2f;
	[SerializeField] AnimationCurve delayCurve = new AnimationCurve();
	Texture m_texture;
	Material m_material;

	public bool isGrowedGrass = false;
	public bool isBloomedFlower = false;

	public bool isBack = false;
	public bool isGrowGrassOnStart = false;
	public float GrowOnStartDelay = 0;

	public static int CoverRecordNum = 10;
	Vector4[] CoverRec = new Vector4[CoverRecordNum];

	List<Flower> flowers = new List<Flower>();

	public struct CoverInfo
	{
		public Vector4 to;
		public Vector4 tem;
		public float process;
		public float delay;
	}

	CoverInfo[] coverInfos = new CoverInfo[CoverRecordNum];
//	Vector4[] coverInitRec = new Vector4[CoverRecordNum];
//	Vector4[] coverTemRec = new Vector4[CoverRecordNum];
//	float coverDelay;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn, OnGrowFlowerOn);
		EventManager.Instance.RegistersEvent(EventDefine.BloomFlower , OnBloomFlower );
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn, OnGrowFlowerOn);
		EventManager.Instance.UnregistersEvent(EventDefine.BloomFlower , OnBloomFlower );
	}

	void OnBloomFlower( Message msg )
	{
		Debug.Log("Bloom Flower");
		Flower flower = (Flower) msg.sender;
		if ( flower != null )
		{
			if ( flower.transform.parent == transform )
			{
				isBloomedFlower = true;

			}
		}
	}


	void OnGrowFlowerOn(Message msg )
	{
		PetalInfo info = (PetalInfo)msg.GetMessage( "info" );
		if ( info.parent.gameObject == this.gameObject )
		{
			if ( !isGrowedGrass )
			{
				GrowGrass(info);
			}
		}

		Flower flower = (Flower)msg.GetMessage("flower");
		if ( flower != null )
		{
			flowers.Add( flower );
		}
	}

	void Awake()
	{
		Init();
		InitEffect();
		InitLayer();
	}

	void Start()
	{
		if ( isGrowGrassOnStart )
			GrowGrassOnStart();
	}

	/// <summary>
	/// Set up Tag & ori position  & grass prefab 
	/// </summary>
	void Init()
	{
		gameObject.tag = "Land";

		oriPosition = transform.localPosition;

		if ( grassPrefab == null )
		{
			grassPrefab = Resources.Load( "Prefab/Rock/Grass") as GameObject;
		}

	}

	/// <summary>
	/// Init the position layer and sprite layer(Inback), must run after init the effect!
	/// </summary>
	void InitLayer()
	{
		m_layer = (PosLayer) * 0.1f;

		if ( isBack ) 
		{
			if ( effectSprite != null )
				effectSprite.sortingLayerName = "BackRock";
		}else{
			if ( effectSprite != null )
				effectSprite.sortingLayerName = "Rock";
		}
	}

	/// <summary>
	/// set up the effect related stuffs
	/// </summary>
	void InitEffect()
	{
		if ( thisSprite == null )
			thisSprite = GetComponentInChildren<tk2dSprite>();
		if ( GetComponent<MeshRenderer>() != null )
			GetComponent<MeshRenderer>().enabled = false;

		if ( effectSprite == null )
			effectSprite = GetComponentInChildren<SpriteRenderer>();

		if ( effectParameter.resultTex == null && effectSprite != null )
			effectParameter.resultTex = (Texture2D) effectSprite.material.mainTexture;

		if (effectParameter.effectShader != null )
			m_material = new Material(effectParameter.effectShader);
		if ( effectSprite != null )
			effectSprite.material = m_material;
		if ( m_material != null )
		{
			m_material.SetTexture("_MainTex" , effectParameter.resultTex );
			m_material.SetTexture("_CoverTex" , effectParameter.coverTex );
			m_material.SetColor("_Color" , effectParameter.mainColor );
			m_material.SetFloat("_FadePos" , effectParameter.fadePosition );
			m_material.SetFloat("_FadeRange" , effectParameter.fadeRange );
			m_material.SetInt("_CountNum" , effectParameter.dropNum );

			for( int i = 0 ; i < CoverRecordNum ; ++ i )
			{
				Vector4 toVec =  new Vector4();

				if ( i == 0 )
				{
					toVec.x = toVec.y = 0;
				}else
				{
					toVec.x = Random.Range( - effectParameter.coverInit.x , effectParameter.coverInit.x);
					toVec.y = Random.Range( - effectParameter.coverInit.y , effectParameter.coverInit.y);
				}
				toVec.z = 1.0f / effectParameter.coverTex.width * effectParameter.resultTex.width ;
				toVec.w = 1.0f / effectParameter.coverTex.height * effectParameter.resultTex.height;

				coverInfos[i].tem = toVec;
				coverInfos[i].delay = Random.Range(effectParameter.delay.min,effectParameter.delay.max);

				float temScale = Random.Range( effectParameter.expand.min , effectParameter.expand.max);
				toVec.z /= temScale;
				toVec.w /= temScale;

				coverInfos[i].to = toVec;
				coverInfos[i].process = 0;

				toVec.x = 9999f;
				toVec.y = 9999f;

				m_material.SetVector( "_CoverRec" + i.ToString() , toVec );
			}

		}
		if ( CoverRec == null)
			CoverRec = new Vector4[CoverRecordNum];
		for( int i = 0 ; i < CoverRecordNum ; ++ i )
		{
			CoverRec[i] = new Vector4( 0 , 0 , 1f , 1f );
		}
			
	}

	float GetGaussValue( int i , int j )
	{
		float x = i - (Global.GaussSize - 1 ) * 0.5f;
		float y = j - (Global.GaussSize - 1 ) * 0.5f;
		float res = 1 / ( 2f * Mathf.PI * Global.GaussSigma * Global.GaussSigma );
		res *= Mathf.Exp( - ( x * x + y * y ) / ( 2f * Global.GaussSigma * Global.GaussSigma ) );
		return res ;
	}

	void UpdatePosition()
	{
		transform.localPosition = oriPosition + LogicManager.CameraManager.OffsetFromInit * ( m_layer );
	}

	float timer = 0 ;
	bool isGrowFinished = false;
	void UpdateSprite()
	{
		timer += Time.deltaTime;
		if ( !isGrowFinished )
		for ( int i = 0 ; i < 3 ; ++ i )
		{
			if ( timer > coverInfos[i].delay )
			{
				coverInfos[i].process += Time.deltaTime / effectParameter.growTime;
				coverInfos[i].tem.z = coverInfos[i].to.z / effectParameter.sizeCurve.Evaluate( coverInfos[i].process );
				coverInfos[i].tem.w = coverInfos[i].to.w / effectParameter.sizeCurve.Evaluate( coverInfos[i].process );

				m_material.SetVector( "_CoverRec" + i.ToString() , coverInfos[i].tem );
			}
		}


		if ( timer > effectParameter.growTime && !isGrowFinished )
		{
			FinishGrow();
		}
	}

	public bool IfCanGrowFlower()
	{
		return enableGrowFlower;
	}


	public void GrowGrass(PetalInfo info)
	{
		int count = (int ) (grassTopOffset.x * density);
		int l = 0 ; // limit the max time
		for ( int k = 0 ; k < count && l < 10000  ; l++  )
		{
			Vector3 ranPos = transform.position
				+ new Vector3( Random.Range( - grassTopOffset.x / 2f , grassTopOffset.x / 2f )
					, grassTopOffset.y , 0 );
			string[] maskList = {"Land"};
			int mask = LayerMask.GetMask(maskList);
			RaycastHit hit;
			if( Physics.Raycast( ranPos , Vector3.down , out hit , 100f , mask) )
			{
				if ( hit.collider.gameObject == this.gameObject )
				{
					float delay = delayCurve.Evaluate( (info.position - hit.point ).magnitude);
					StartCoroutine( GrowGrassOn( hit.point , hit.normal, delay));
					k++;
				}else
				{
					Debug.Log("Hit but not this " + ranPos + " " + hit.collider.name);
				}
			}
		}

		isGrowedGrass = true;
		EventManager.Instance.PostEvent(EventDefine.GrowGrass , new Message() , this );

		if ( info.type == PetalType.Init )
			EventManager.Instance.PostEvent( EventDefine.LevelInitialized , new Message() , this );
	
	}

	public void GrowGrassOnStart()
	{
		Debug.Log("Grow");
		int count = (int ) (grassTopOffset.x * density);
		int l = 0; // limit the max time 
		for ( int k = 0 ; k < count && l < 10000 ; l++ )
		{
			Vector3 ranPos = transform.position
				+ new Vector3( Random.Range( - grassTopOffset.x / 2f , grassTopOffset.x / 2f )
					, grassTopOffset.y , 0 );
			string[] maskList = {LayerMask.LayerToName( this.gameObject.layer) };
			int mask = LayerMask.GetMask(maskList);
			RaycastHit hit;
			if( Physics.Raycast( ranPos , Vector3.down , out hit , 100f , mask) )
			{
				if ( hit.collider.gameObject == this.gameObject )
				{
					float delay = delayCurve.Evaluate( (Vector3.zero - hit.point ).magnitude ) + GrowOnStartDelay;
					StartCoroutine( GrowGrassOn( hit.point , hit.normal, delay));
					k++;
				}else
				{

					Debug.Log("Hit but not this " + ranPos + " " + hit.collider.name);
				}
			}
		}

		isGrowedGrass = true;
		EventManager.Instance.PostEvent(EventDefine.GrowGrass , new Message() , this );
	}

	public IEnumerator GrowGrassOn(Vector3 pos , Vector3 normal , float delay)
	{
		Vector3 localPos = transform.InverseTransformPoint( pos );
		yield return new WaitForSeconds( delay );

		GameObject grassObj = Instantiate( grassPrefab ) as GameObject;
		grassObj.transform.SetParent( transform , true );
		grassObj.transform.localScale = GetGrowScale();
		grassObj.transform.localPosition = localPos;


		Grass grass = grassObj.GetComponent<Grass>();
		grass.Init( normal );
	}

	void FinishGrow()
	{
		effectSprite.enabled = false;

		thisSprite.color = effectParameter.mainColor;
		isGrowFinished = true;

		if ( GetComponent<MeshRenderer>() != null )
			GetComponent<MeshRenderer>().enabled = true;
	}

	public bool IsCompleted()
	{
		return ( isGrowedGrass  ) || isBack;
	}

	public bool IsAllDie()
	{
		for( int i = 0 ;i < flowers.Count ; ++ i )
		{
			if ( !flowers[i].isAllDead() ) {
				return false;
			}
		}
		return true;
	}



	void Update()
	{
		UpdatePosition();
		UpdateSprite();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Vector3 pos = transform.position;
		Vector3 left = pos + new Vector3(- grassTopOffset.x / 2f , grassTopOffset.y , 0 );
		Vector3 right = pos + new Vector3( grassTopOffset.x / 2f , grassTopOffset.y , 0 );
		Gizmos.DrawLine( left , right );
	}

	public Vector3 GetGrowScale()
	{
		float toScale = grassScale.x / transform.lossyScale.x ;
		return Vector3.one * toScale;
	}

}
