using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class InkSpread : MonoBehaviour {

	[SerializeField] tk2dSprite sprite;
	[SerializeField] AnimationCurve fadeCurve;
//	[SerializeField] AnimationCurve sizeCurve;

	static public List<InkSpread> CreateInkSpreadByVelocity( Vector3 pos , Vector2 velocity , GameObject prefab , float time)
	{
		float scaleSense = 0.01f;
		float distanSense = 0.5f;
		float flyDistance = 0.014f;
		float fadeDelay = 0.05f;
		float initScaleTime = 0.18f;
		float spreadTime = 5f;

		List<InkSpread> inkList = new List<InkSpread>();
			
		//Secondary Ink
		for( float i = 0f ; i < 4f ; i = i + 1f )
		{
			if ( Random.Range( 0 , 1f ) < 1.2f - i * 0.2f )
			{
				float mScaleSense =  scaleSense / ( 1f + i * 2f ) ;
				float mDistanSense = distanSense ;
				float mFlyDistance = flyDistance * i * Random.Range( 0.66f , 1.5f );
				float mFadeDelay = fadeDelay * i;
				GameObject ink = Instantiate ( prefab ) as GameObject;
				ink.transform.parent = LogicManager.LevelManager.GetLevelObject().transform;
				pos.z = 0;
				ink.transform.position = pos + Global.V2ToV3( velocity ) * mFlyDistance 
					+ Global.V2ToV3( Global.GetRandomDirection() * Random.Range( 0.1f , mDistanSense));
				float toScale = ( 0.1f + velocity.magnitude * mScaleSense ) * Random.Range( 0.8f , 1.2f ) ;
				ink.transform.localScale = Vector3.zero ;

				Sequence seq = DOTween.Sequence();
				seq.AppendInterval( fadeDelay * i );
				seq.Append( ink.transform.DOScale( toScale, initScaleTime ).SetEase( Ease.InCirc ) );
				seq.Append( ink.transform.DOScale( toScale * 1.2f , spreadTime ).SetEase( Ease.InQuart ));


				InkSpread inkCom = ink.GetComponent<InkSpread>();

				inkList.Add( inkCom );
			}
		}

		return inkList;
	}

	void Awake()
	{
		if ( sprite == null )
			sprite = GetComponent<tk2dSprite>();

		sprite.SetSprite( "InkSpread" + Random.Range( 1 , 4 ).ToString());

		transform.RotateAround( Vector3.forward , Random.Range( 0 , 360f ));

	}

	public void Fade( float Time  )
	{
		StartCoroutine( FadeCor( Time  ) ); 
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
