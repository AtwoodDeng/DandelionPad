using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class InkSpread : MonoBehaviour {

	[SerializeField] tk2dSprite sprite;
	[SerializeField] AnimationCurve fadeCurve;
	[SerializeField] float fadeOutTime = 3f;
//	[SerializeField] AnimationCurve sizeCurve;
	float initAlpha;


	static GameObject m_prefab;
	static GameObject Prefab
	{
		get {
			if ( m_prefab == null )
				m_prefab = (GameObject)Resources.Load( Global.INKSPREAD_PATH);
			return m_prefab;
		}
	}
	static public void CreateInkSpreadHitLands( GameObject _prefab, Vector3 pos , Transform parent , int number , float scale )
	{
		if ( _prefab == null )
			_prefab = Prefab;
		
		for( int i = 0 ; i < number; ++ i )
		{
			GameObject ink = Instantiate( _prefab ) as GameObject;
			ink.transform.position = pos + Global.V2ToV3( Global.GetRandomDirection()) * Random.Range( 0 , scale * 5f );
			ink.transform.localScale = Vector3.one * scale * Random.Range( 0.5f , 2f );
			ink.transform.SetParent( parent , true);
			ink.transform.Rotate( Vector3.forward , Random.Range( 0 , 360f ));

			InkSpread inkCom = ink.GetComponent<InkSpread>();
			inkCom.sprite.SetSprite( "InkSpread" + Random.Range( 1 , 4 ).ToString());

			inkCom.Fade();

			if ( i == 0 )
			{
				AudioSource audio = ink.AddComponent<AudioSource>();
				audio.clip =  (AudioClip) Resources.Load(Global.GRASS_CRASH_SOUND_PATH );
				audio.loop =false;
				audio.volume = 0.6f;
				audio.Play();
			}
		}
	}

	static public void CreateInkSpread( GameObject _prefab, Vector3 pos , Transform parent , int number , float scale )
	{
		for( int i = 0 ; i < number; ++ i )
		{
			GameObject ink = Instantiate( _prefab ) as GameObject;
			Vector3 m_pos = pos + Global.V2ToV3( Global.GetRandomDirection()) * Random.Range( 0 , scale * 5f );
			if ( parent != null )
				m_pos.z = parent.position.z;
			else
				m_pos .z = 0;
			ink.transform.position = m_pos;
			ink.transform.localScale = Vector3.one * scale * Random.Range( 0.5f , 2f );
			ink.transform.SetParent( parent , true);
			ink.transform.Rotate( Vector3.forward , Random.Range( 0 , 360f ));

			InkSpread inkCom = ink.GetComponent<InkSpread>();

			inkCom.Fade();
		}
	}

	static public void CreateInkSpreadBlowFlower( List<InkSpread> list , Vector3 pos , Vector2 velocity , Transform parent )
	{
		float scaleSense = 0.05f;
		float distanSense = 0.03f;
		float flyDistance = 0.08f;
		float fadeDelay = 0.05f;
		float initScaleTime = 0.18f;
		float spreadTime = 20f;
			
		int j = 0;

		//Secondary Ink
		for( float i = 0f ; i < 4f ; i = i + 1f )
		{
			while ( list[j].gameObject.activeSelf && j < list.Count - 1 )
			{
				j ++;
			}

			InkSpread ink = list[j];

			if ( Random.Range( 0 , 1f ) < 1.2f - i * 0.2f )
			{
				ink.gameObject.SetActive( true );

				float mScaleSense = scaleSense / ( 1f + i * 1.33f ) ;
				float mDistanSense = distanSense *  Mathf.Sqrt( velocity.magnitude);
				float mFlyDistance = flyDistance * i * Random.Range( 0.8f , 1.2f );
				float mFadeDelay = fadeDelay * i;

				pos.z = 2f;
				ink.transform.SetParent( parent );
				ink.transform.position = pos + Global.V2ToV3( velocity.normalized ) * Mathf.Sqrt( velocity.magnitude + 10f ) * mFlyDistance 
					+ Global.V2ToV3( Global.GetRandomDirection() * Random.Range( 0 , mDistanSense));
				float toScale = ( 0.15f + Mathf.Sqrt( velocity.magnitude ) * mScaleSense ) * Random.Range( 0.8f , 1.2f ) ;
				ink.transform.localScale = Vector3.zero ;

				ink.sprite.SetSprite( "InkSpread" + Random.Range( 1 , 4 ).ToString());
				ink.transform.Rotate( Vector3.forward , Random.Range( 0 , 360f ));

				Color col = LogicManager.TrailColor;
				col.a =  ink.initAlpha;
				ink.sprite.color = col;

				Sequence seq = DOTween.Sequence();
				seq.AppendInterval( mFadeDelay );
				seq.Append( ink.transform.DOScale( toScale, initScaleTime ).SetEase( Ease.InCirc ) );
				seq.Append( ink.transform.DOScale( toScale * 1.5f , spreadTime ));

			}
		}

	}

	void Awake()
	{
		if ( sprite == null )
			sprite = GetComponent<tk2dSprite>();

		initAlpha = sprite.color.a;

	}

	public void Fade( )
	{
		StartCoroutine( FadeCor( fadeOutTime  ) ); 
	}

	IEnumerator FadeCor( float fadeTime  )
	{
		float timer = 0;
		float initAlpha = sprite.color.a;
//		{
//			Color col = sprite.color;
//			col.a = 0;
//			sprite.color = col;
//		}
//		float initScale = transform.localScale.x;

		while( true )
		{
			timer += Time.deltaTime * LogicManager.UITimeRate;

			if ( timer > fadeTime )
				break;

			float alpha = fadeCurve.Evaluate( timer / fadeTime ) * initAlpha;
//			float size = sizeCurve.Evaluate( timer / fadeTime ) * initScale;

			Color col = sprite.color;
			col.a = alpha;
			sprite.color = col;

//			transform.localScale = Vector3.one * size;

			yield return null;
		}

		gameObject.SetActive(false);
	}
}
