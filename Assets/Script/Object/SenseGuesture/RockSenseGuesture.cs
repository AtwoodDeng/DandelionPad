using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RockSenseGuesture : SenseGuesture {

	[SerializeField] Transform shakeTransform;
	[SerializeField] float rotateStrength =1f;
	[SerializeField] float rotateDuration = 1f;
	[SerializeField] AudioSource growSound;
	[SerializeField] AudioSource touchSound;
	[SerializeField] bool ifDisableOnAwake;

	bool ifGrowed = false;

	public void Awake()
	{
		Collider col = GetComponent<Collider>();
		if ( col != null && ifDisableOnAwake )
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
			shakeTransform.DOShakeRotation( rotateDuration , new Vector3( 0 , 0 , rotateStrength ) );

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
