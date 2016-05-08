 using UnityEngine;
using System.Collections;

public class Wind3D : MonoBehaviour {

	[SerializeField] public Vector2 direction;
	[SerializeField] float directionVary;
	[SerializeField] float directionBoundary = 0.2f;
	[SerializeField] float changeFrequency = 0.5f;
	public Vector2 temDirection
	{
		get {
			return direction + directionOffset;
		}
	}
	Vector2 directionOffset=Vector2.zero;
	Vector2 directionOffVol=Vector2.zero;
	Vector2 directionOffAcc=Vector2.zero;
	[SerializeField] public float Intense;
	[SerializeField] float IntenseVary;
	[SerializeField] float IntenseBoundary = 0.1f;
	public float temIntense{
		get {
			return Intense + intenseOffset;
		}
	}
	float intenseOffset=0;
	float intenseOffVol=0;
	float intenseOffAcc=0;

	public float Angle;

	IEnumerator changeWind;
	void Awake()
	{
		Angle = transform.rotation.eulerAngles.z / 180f * Mathf.PI;
		direction = new Vector2( Mathf.Cos( Angle ) 
			,  Mathf.Sin( Angle )  );

		Debug.Log("wind init " + Angle.ToString() + " " + direction.ToString());
		changeWind = ChangeWind();
		StartCoroutine(changeWind);
	}

	void OnTriggerStay2D(Collider2D col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			ws.SenseWind(temIntense * temDirection.normalized);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			ws.ExitWind();
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void OnTriggerStay(Collider col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			ws.SenseWind(temIntense * temDirection.normalized);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void OnTriggerExit(Collider col)
	{
		WindSensable ws = col.GetComponent<WindSensable>();
		if (ws != null )
		{
			ws.ExitWind();
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	IEnumerator ChangeWind()
	{
		float timer = 0;
		while(true)
		{
			timer += Time.deltaTime;

			if (Random.Range(0,1f) < Time.deltaTime / changeFrequency )
			{
				directionOffAcc = Random.insideUnitCircle * directionVary;
				intenseOffAcc = Random.Range(-1f, 1f) * IntenseVary;
			}

			directionOffVol += directionOffAcc;
			directionOffset += directionOffVol;
			if (directionOffset.magnitude > direction.magnitude * directionBoundary)
				directionOffset = directionOffset.normalized * direction.magnitude * directionBoundary;

			intenseOffVol += intenseOffAcc;
			intenseOffset += intenseOffVol;
			intenseOffVol *= 0.995f;
			intenseOffset = Mathf.Clamp(intenseOffset,-Intense*IntenseBoundary,Intense*IntenseBoundary);

			Angle = Mathf.Atan(temDirection.y/temDirection.x)*180f/Mathf.PI;
			transform.eulerAngles = new Vector3(0,0,Angle);
			
			yield return null;
		}
	}
}
