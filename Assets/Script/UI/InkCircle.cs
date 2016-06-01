using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InkCircle : MonoBehaviour {

	[SerializeField] tk2dSprite sprite;
	[SerializeField] AnimationCurve fadeCurve;
	[SerializeField] float spinRate = 10f;
	[SerializeField] float fadeInTime ;
	[SerializeField] float fadeOutTime;
	[SerializeField] float minDistance;

	float initAlpha;

	void Awake()
	{
		if ( sprite == null )
			sprite = GetComponent<tk2dSprite>();

		initAlpha = sprite.color.a;
	}

	void LateUpdate()
	{
		transform.Rotate( Vector3.forward , spinRate * Time.deltaTime );
	}

	public void FadeIn()
	{
		if ( fadeOutCor != null )
			StopCoroutine( fadeOutCor );
		gameObject.SetActive( true );	

		DOTween.To( (x) => {

			Color col = sprite.color;
			col.a = x;
			sprite.color = col;
		}
			, 0 
			, initAlpha
			, fadeInTime
		);
	}

	public void UpdateCircle( Vector2 screenPos , float distance )
	{
		Vector3 position = Camera.main.ScreenToWorldPoint( screenPos );
		position.z = 0;

		transform.position = position;

		transform.localScale = Vector3.one * ( distance < minDistance ? minDistance : distance ) / 250f ;
	}

	Coroutine fadeOutCor ;
	public void FadeOut( )
	{
		fadeOutCor = StartCoroutine( FadeCor( fadeOutTime  ) ); 
	}

	IEnumerator FadeCor( float fadeTime  )
	{
		float timer = 0;
		float initAlpha = sprite.color.a;

		while( true )
		{
			timer += Time.deltaTime * LogicManager.UITimeRate;

			if ( timer > fadeTime )
				break;

			float alpha = fadeCurve.Evaluate( timer / fadeTime ) * initAlpha;

			Color col = sprite.color;
			col.a = alpha;
			sprite.color = col;

			yield return null;
		}

		gameObject.SetActive(false);
		fadeOutCor = null;
	}
}
