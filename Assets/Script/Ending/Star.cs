using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

	[SerializeField] SpriteRenderer star;
	[SerializeField] SpriteRenderer extend;
	[SerializeField] public bool isActive;


	[SerializeField] Color inactiveColor = Color.gray;

	Color starColor;
	Color extendColor;
	void Awake()
	{
		if ( star == null )
			star = GetComponent<SpriteRenderer>();
		if ( extend == null )
			extend = GetComponentInChildren<SpriteRenderer>();
		starColor = star.color;
		extendColor = extend.color;
	}

	public void SetActive()
	{
		extend.color = extendColor;
		isActive = true;
	}

	public void SetInactive()
	{
		star.color = inactiveColor;
		Color col = extendColor;
		col.a = 0;
		extend.color = col;
		isActive = false;
	}
}
