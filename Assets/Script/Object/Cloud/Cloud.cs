using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Cloud : MonoBehaviour {

	[SerializeField] SpriteRenderer sprite1;
	[SerializeField] SpriteRenderer sprite2;
	[SerializeField] float speed;
	[SerializeField] bool isFadeIn = false;
	[SerializeField] float fadeTime = 3f;

	SpriteRenderer temSprite;

	void Start()
	{
		if ( sprite1.transform.localPosition.x < sprite1.sprite.bounds.size.x / 2f )
			temSprite = sprite1;
		else
			temSprite = sprite2;

		speed = Mathf.Abs( speed );
	}
	void LateUpdate()
	{
		{
			Vector3 pos = sprite1.transform.localPosition;
			pos.x += speed * Time.deltaTime * LogicManager.PhysTimeRate;
			sprite1.transform.localPosition = pos;
		}

		{
			Vector3 pos = sprite2.transform.localPosition;
			pos.x += speed * Time.deltaTime * LogicManager.PhysTimeRate;
			sprite2.transform.localPosition = pos;
		}

		if ( temSprite.transform.localPosition.x > 0 )
		{
			SpriteRenderer other = ( temSprite == sprite1 ) ? sprite2 : sprite1;

			Vector3 pos = other.transform.localPosition;
			pos.x -= temSprite.sprite.bounds.size.x * 2;
			other.transform.localPosition = pos;

			if ( isFadeIn ) 
			{
				Debug.Log("Do Fade" + other.name );
				other.DOFade( 0 , fadeTime ).From();
			}
			temSprite = other;
		}
	}
}
