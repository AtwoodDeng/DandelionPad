using UnityEngine;
using System.Collections;

public class Ink : MonoBehaviour {

	[SerializeField] AnimationCurve sizeCurve;
	[SerializeField] AnimationCurve alphaCurve;
	[SerializeField] AnimationCurve fadeCurve;
	[SerializeField] float totalSpreadTime = 1f;
	[SerializeField] float fadeTime = 1f;
	[SerializeField] tk2dSprite sprite;

	float initSize = 1f;
	void Awake()
	{
		if ( sprite == null )
			sprite = GetComponent<tk2dSprite>();
		Spread(0);
	}

	void Start()
	{
		transform.Rotate( new Vector3( 0 , 0 , Random.Range( 0 , 360f )));
		initSize = Random.Range( 0.7f , 1.5f );
		transform.localScale = initSize * Vector3.one;
	}

	public float affectRange()
	{
		return sprite.CurrentSprite.GetBounds().extents.x * transform.localScale.x ;
	}

	float process = 0;
	bool canSpread = true;
	public void Spread(float dt)
	{
		if ( !canSpread )
			return;
		process += dt * LogicManager.UITimeRate / totalSpreadTime;

		float size = sizeCurve.Evaluate(process);
		transform.localScale = size * initSize * Vector3.one;

		float alpha = alphaCurve.Evaluate(process);
		if ( sprite != null )
		{
			Color col = sprite.color;
			col.a = alpha;
			sprite.color = col;
		}

	}

	public void Fade()
	{
		canSpread = false;
		StartCoroutine(FadeCor());
	}

	float timer = 0;
	IEnumerator FadeCor()
	{
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
	}
}
