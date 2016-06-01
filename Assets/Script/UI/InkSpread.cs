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

	static public void CreateInkSpreadByVelocity( List<InkSpread> list , Vector3 pos , Vector2 velocity , Transform parent )
	{
		float scaleSense = 0.04f;
		float distanSense = 0.03f;
		float flyDistance = 0.008f;
		float fadeDelay = 0.05f;
		float initScaleTime = 0.18f;
		float spreadTime = 20f;
			
		int j = 0;
		Debug.Log(list.Count );
		//Secondary Ink
		for( float i = 0f ; i < 4f ; i = i + 1f )
		{
			while ( list[j].gameObject.activeSelf && j < list.Count - 1 )
			{
				Debug.Log("j = " +j );
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
				ink.transform.position = pos + Global.V2ToV3( velocity ) * mFlyDistance 
					+ Global.V2ToV3( Global.GetRandomDirection() * Random.Range( 0 , mDistanSense));
				float toScale = ( 0.1f + Mathf.Sqrt( velocity.magnitude ) * mScaleSense ) * Random.Range( 0.8f , 1.2f ) ;
				ink.transform.localScale = Vector3.zero ;

				ink.sprite.SetSprite( "InkSpread" + Random.Range( 1 , 4 ).ToString());
				ink.transform.Rotate( Vector3.forward , Random.Range( 0 , 360f ));

				Color col = ink.sprite.color;
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
