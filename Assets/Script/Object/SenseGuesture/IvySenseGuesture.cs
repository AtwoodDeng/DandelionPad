using UnityEngine;
using System.Collections;
using DG.Tweening;
public class IvySenseGuesture : SenseGuesture {

	[SerializeField] float shakeStrength =1f;
	[SerializeField] float shakeDuration = 1f;
	[SerializeField] AudioSource growSound;
	[SerializeField] AudioSource touchSound;
	[SerializeField] GameObject myInkSpread;
	[SerializeField] bool touchInkSpread;

	bool ifGrowed = false;

	public void Awake()
	{
		Collider col = GetComponent<Collider>();
		col.enabled = false;
		if ( growSound != null )
			growSound.playOnAwake = false;
		if ( touchSound != null )
			touchSound.playOnAwake = false;
	}
	public override void DealTap (TapGesture guesture)
	{
		base.DealTap (guesture);

		if ( touchSound == null || touchSound.isPlaying == false )
		{
			transform.DOShakePosition( shakeDuration , new Vector3( shakeStrength , shakeStrength , 0 ) );

			if ( myInkSpread != null && touchInkSpread )
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint( guesture.Position );
				InkSpread.CreateInkSpread( myInkSpread , pos , transform , Random.Range( 2, 5 ) , 0.5f );
			}
		}
		if ( touchSound != null && touchSound.clip != null && !touchSound.isPlaying )
			touchSound.Play();
		
		
	}

	public void BeginGrow()
	{
		if ( !ifGrowed )
		{
			Collider col = GetComponent<Collider>();
			if ( col != null )
				col.enabled = true;
			if ( growSound != null )
				growSound.Play();
			ifGrowed = true;
		}
	}
}
