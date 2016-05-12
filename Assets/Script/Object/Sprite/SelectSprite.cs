using UnityEngine;
using System.Collections;

public class SelectSprite : MonoBehaviour {

	[SerializeField] Sprite[] SpriteList;
	[SerializeField] SpriteRenderer targetSprite;
	[SerializeField] bool isSelectOnAwake = true;

	void Awake()
	{
		if ( targetSprite == null )
			targetSprite = GetComponent<SpriteRenderer>();
		Select();
	}

	public void Select()
	{
		if ( targetSprite != null && SpriteList != null && SpriteList.Length > 0 )
		{
			targetSprite.sprite = SpriteList[Random.Range(0,SpriteList.Length)];
		}
	}

}
