using UnityEngine;
using System.Collections;

public class InkAnimation : MonoBehaviour {

	[SerializeField] SpriteRenderer sprite;

	[System.Serializable]
	public struct EffectParameter
	{
		public Shader effectShader;
		public Texture2D resultTex;
		public Texture2D coverTex;
		public Color mainColor;
		public Vector4 coverInit;
		public float fadePosition;
		public float fadeRange;
		public AnimationCurve sizeCurve;
		public MaxMin delay;
		public MaxMin expand;
		public int dropNum;
		public float growTime;
		public bool isRandInitPos;
	}


	[SerializeField] EffectParameter para;

	Material m_material;

	public static int CoverRecordNum = 10;
	Vector4[] CoverRec = new Vector4[CoverRecordNum];


	public struct CoverInfo
	{
		public Vector4 to;
		public Vector4 tem;
		public float process;
		public float delay;
	}

	CoverInfo[] coverInfos = new CoverInfo[CoverRecordNum];
	[SerializeField] bool isStartShow = true;
	void Awake()
	{
		InitEffect();
	}

	void Start()
	{
		if ( isStartShow )
			ShowSprite();
	}

	void InitEffect()
	{
		if ( GetComponent<MeshRenderer>() != null )
			GetComponent<MeshRenderer>().enabled = false;

		if ( sprite == null )
			sprite = GetComponentInChildren<SpriteRenderer>();

		if ( para.resultTex == null && sprite != null )
			para.resultTex = (Texture2D) sprite.material.mainTexture;

		if (para.effectShader != null )
			m_material = new Material(para.effectShader);
		if ( sprite != null )
			sprite.material = m_material;
		if ( m_material != null )
		{
			m_material.SetTexture("_MainTex" , para.resultTex );
			m_material.SetTexture("_CoverTex" , para.coverTex );
			m_material.SetColor("_Color" , para.mainColor );
			m_material.SetFloat("_FadePos" , para.fadePosition );
			m_material.SetFloat("_FadeRange" , para.fadeRange );
			m_material.SetInt("_CountNum" , para.dropNum );

			for( int i = 0 ; i < CoverRecordNum ; ++ i )
			{
				Vector4 toVec =  new Vector4();

				if ( para.isRandInitPos )
				{
					if ( i == 0 )
					{
						toVec.x = toVec.y = 0;
					}else
					{
						toVec.x = Random.Range( - para.coverInit.x , para.coverInit.x);
						toVec.y = Random.Range( - para.coverInit.y , para.coverInit.y);
					}
				}else
				{
					toVec.x = para.coverInit.x;
					toVec.y = para.coverInit.y;
				}

				toVec.z = 1.0f / para.coverTex.width * para.resultTex.width ;
				toVec.w = 1.0f / para.coverTex.height * para.resultTex.height;

				coverInfos[i].tem = toVec;
				coverInfos[i].delay = Global.GetRandomMinMax( para.delay );

				float temScale = Global.GetRandomMinMax( para.expand );
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

	public void ShowSprite()
	{
		StartCoroutine( SpriteAppear() );
	}

	IEnumerator SpriteAppear()
	{
		float timer = 0;

		while ( timer < para.growTime )
		{
			timer += Time.deltaTime;	
			for ( int i = 0 ; i < para.dropNum ; ++ i )
			{
				if ( timer > coverInfos[i].delay )
				{
					coverInfos[i].process += Time.deltaTime / para.growTime;
					coverInfos[i].tem.z = coverInfos[i].to.z / para.sizeCurve.Evaluate( coverInfos[i].process );
					coverInfos[i].tem.w = coverInfos[i].to.w / para.sizeCurve.Evaluate( coverInfos[i].process );

					m_material.SetVector( "_CoverRec" + i.ToString() , coverInfos[i].tem );
				}
			}
			yield return null;
		}
	}
}
