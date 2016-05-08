using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class OnEventFade : OnEvent {

	[SerializeField] float fadeTo;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] Image img;
	[SerializeField] bool isFrom;
	[SerializeField] bool once = false;
	[SerializeField] bool hideInBegin = false;

	protected bool ifCalled = false;

	void Awake()
	{
		if ( hideInBegin )
		{
			if ( sprite == null )
			{
				sprite = GetComponent<SpriteRenderer>();
			}
			if ( sprite != null )
			{
				sprite.DOFade( 0 , 0 );
			}


			if ( img == null )
			{
				img = GetComponent<Image>();
			}
			if ( img != null )
			{
				img.DOFade(0,0);
			}
		}
	}

	protected override void Do (Message msg)
	{
		if ( once && ifCalled )
			return;

		ifCalled = true;

		if ( sprite == null )
		{
			sprite = GetComponent<SpriteRenderer>();
		}
		if ( sprite != null )
		{
			sprite.enabled = true;
			if ( isFrom)
				sprite.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).From().OnComplete(CheckTheAlpha);
			else
				sprite.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).OnComplete(CheckTheAlpha);
		}


		if ( img == null )
		{
			img = GetComponent<Image>();
		}
		if ( img != null )
		{
			img.enabled = true;
			if ( isFrom )
				img.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).From().OnComplete(CheckTheAlpha);
			else
				img.DOFade( fadeTo , time ).SetDelay(delay).SetEase(easeType).OnComplete(CheckTheAlpha);
		}

	}

	void CheckTheAlpha()
	{
		if ( sprite != null && sprite.color.a <= 0.01f )
			sprite.enabled = false;
		if ( img != null && img.color.a <= 0.01f )
			img.enabled = false;
	}
}
