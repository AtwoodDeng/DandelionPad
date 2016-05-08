using UnityEngine;
using System.Collections;

public class AddSwipe : MonoBehaviour {

	[SerializeField] bool isEntered = false;
	// Use this for initialization
	void Awake () {
	
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if (isEntered)
			return;
		Petal petal = col.gameObject.GetComponent<Petal>();
		if ( petal != null )
		{
			EventManager.Instance.PostEvent(EventDefine.AddSwipeTime);
			isEntered = true;

		}
	}
}
