using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InkIvyFlower : MonoBehaviour {

	public Collider collider;
	public SpriteRenderer spriteRender;
	[SerializeField] AnimationCurve growCurve;
	[SerializeField] float growChance = 0.5f;
	[SerializeField] float growTime = 2f;
	[SerializeField] AudioSource sound;
	[SerializeField] Color fromColor;
	[SerializeField] Color toColor;

	static int number = 0;

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
		Color col = Color.Lerp( fromColor , toColor , Random.Range( 0 , 1f ) );
		col.a = Mathf.Lerp( fromColor.a , toColor.a , Random.Range( 0 , 1f ));
		spriteRender.color = col;
		if ( sound == null )
			sound = GetComponent<AudioSource>();
		if ( sound != null )
			sound.playOnAwake = false;
	}

	void OnTriggerEnter(Collider col)
	{
//		Debug.Log( name + "Enter " + col.name );
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
		number ++;

		if ( sound != null )
		{
			sound.pitch = 0.7f + number * 0.05f;
			sound.Play();
		}

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
