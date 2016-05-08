using UnityEngine;
using System.Collections;

public class Final : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		Debug.Log("Enter Final");
		Petal petal = col.gameObject.GetComponent<Petal>();
		if ( petal != null )
		{
			EventManager.Instance.PostEvent(EventDefine.EnterFinal);
		}
	}
}
