using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class OnEventShine : OnEvent {

	[SerializeField] Color startColor;
	[SerializeField] Color fadeToColor;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] Image img;
	[SerializeField] int loopTime;
	[SerializeField] LoopType loopType;

	protected override void Do (Message msg)
	{
		if ( sprite == null )
		{
			sprite = GetComponent<SpriteRenderer>();
		}
		if ( sprite != null )
		{
			sprite.enabled = true;
			sprite.color = startColor;
			sprite.DOKill();
			sprite.DOColor( fadeToColor , time )
				.SetDelay(delay).SetEase(easeType)
				.OnComplete(CheckTheAlpha)
				.SetLoops(loopTime,loopType);
		}


		if ( img == null )
		{
			img = GetComponent<Image>();
		}
		if ( img != null )
		{

			img.enabled = true;
			img.color = startColor;
			img.DOKill();
			img.DOColor( fadeToColor , time )
				.SetDelay(delay).SetEase(easeType)
				.OnComplete(CheckTheAlpha)
				.SetLoops(loopTime,loopType);
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
