 using UnityEngine;
using System.Collections;

public class Wind3DAdv : MonoBehaviour {


	IEnumerator changeWind;
	void Awake()
	{
		RefreshWind();
	}

	void OnTriggerStay2D(Collider2D col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			Debug.Log("Stay 2d " + col.name);
			// ws.SenseWind(this,temIntense,temDirection);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			// ws.ExitWind(this);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void OnTriggerStay(Collider col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			Debug.Log("Stay 3d " + col.name);
			// ws.SenseWind(this,temIntense,temDirection);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void OnTriggerExit(Collider col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			// ws.ExitWind(this);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	public void RefreshWind()
	{
		StartCoroutine(ChangeWind());
	}

	IEnumerator ChangeWind()
	{
		yield break;
	}
}
