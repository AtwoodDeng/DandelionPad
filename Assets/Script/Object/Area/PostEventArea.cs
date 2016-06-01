using UnityEngine;
using System.Collections;

public class PostEventArea : MonoBehaviour {

	[SerializeField] EventDefine postEvent;

	void OnTriggerEnter2D(Collider2D col )
	{
		Petal petal = col.GetComponent<Petal>();
		if ( petal != null )
		{
			EventManager.Instance.PostEvent( postEvent );
		}

	}

	void OnTriggerEnter(Collider col )
	{
		Petal petal = col.GetComponent<Petal>();
		if ( petal != null  )
		{
			if ( petal.state == PetalState.Fly )
			{
				EventManager.Instance.PostEvent( postEvent );
			}
		}

	}
}
