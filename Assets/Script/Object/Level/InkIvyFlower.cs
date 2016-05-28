using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InkIvyFlower : MonoBehaviour {

	public Collider collider;
	public SpriteRenderer spriteRender;
	[SerializeField] AnimationCurve growCurve;
	[SerializeField] float growChance = 0.5f;
	[SerializeField] float growTime = 2f;

	float initScale;
	float initAlpha;
	bool isGrowed = false;
	void Awake()
	{
		initAlpha = spriteRender.color.a;
		spriteRender.DOFade( 0 , 0 ) ;
		initScale = transform.localScale.x * Random.Range( 0.8f , 1.2f );
		spriteRender.transform.localRotation = Quaternion.Euler( new Vector3( 0 , 0 , Random.value * 360f ));
		collider.enabled =false;
		spriteRender.color = new Color( Random.Range( 133f , 200f ) / 255f  ,  0.9f  , 0.36f );
	}

	void OnTriggerEnter(Collider col)
	{
		Debug.Log( name + "Enter " + col.name );
		if ( col.GetComponent<Petal>() != null )
		{
			if ( col.GetComponent<Petal>().state == PetalState.Fly && Random.Range( 0 , 1f ) < growChance && !isGrowed )
			{
				StartCoroutine( Grow());
			}
		}
	}

	IEnumerator Grow()
	{
		Debug.Log("Grow");
		collider.enabled = false;
		float timer = 0;
		spriteRender.DOFade( initAlpha , growTime * 0.25f );

		while( timer < growTime )
		{
			timer += Time.deltaTime;
			float CurveTime = timer / growTime;

			transform.localScale = Vector3.one * growCurve.Evaluate( CurveTime ) * initScale;
			yield return null;
		}

		isGrowed = true;
	}
}
