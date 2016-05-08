using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {

	[SerializeField] Vector2 direction;
	[SerializeField] MaxMin intense;
	[SerializeField] float intenseChangeRate;
	[SerializeField] MaxMin torqueIntense;
	[SerializeField] float switchTime = 5f;
	[SerializeField] float directionChangeRate = 0.1f;
	[SerializeField] float directionLerpRate = 0.02f;

	[SerializeField] float tempIntense;
	[SerializeField] float tempIntenseChangeRate;
	[SerializeField] Vector3 tempDirection;

	[SerializeField] AnimationCurve distrubutionX;
	[SerializeField] AnimationCurve distrubutionY;

	IEnumerator changeWind;
	void Awake()
	{
		direction = new Vector2( Mathf.Cos( transform.rotation.eulerAngles.z ) 
			,  Mathf.Sin( transform.rotation.eulerAngles.z )  );
		tempIntense = Random.Range(intense.min, intense.max);
		tempIntenseChangeRate = intenseChangeRate;
		tempDirection = direction;

		changeWind = ChangeWind(0.1f,intenseChangeRate,direction);
		StartCoroutine(changeWind);
	}

	void OnTriggerStay2D(Collider2D col)
	{
		Petal petal = col.GetComponent<Petal>();
		if (petal != null )
		{
			float distrubution = Mathf.Pow( distrubutionX.Evaluate( petal.transform.position.x ) , 1f )
								 * Mathf.Pow( distrubutionY.Evaluate(petal.transform.position.y) , 1f );

			petal.AddForce( tempIntense * tempDirection.normalized * distrubution);
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	void LateUpdate()
	{
	}

	IEnumerator ChangeWind(float time, float intenseA, Vector2 newDirection)
	{

		float timer = 0;
		while(true)
		{
			timer += Time.deltaTime;
			tempIntenseChangeRate += intenseA;
			tempIntense = Mathf.Clamp(tempIntense + Random.Range(intenseChangeRate, -intenseChangeRate), intense.min, intense.max);
			tempDirection = Vector2.Lerp(tempDirection, newDirection, directionLerpRate);


			if (timer > time)
				break;
			yield return null;
		}

		changeWind = ChangeWind(Random.Range(switchTime/2f, switchTime) 
			, Random.Range(-intenseChangeRate, intenseChangeRate)
			, (direction.normalized + Global.GetRandomDirection().normalized * directionChangeRate).normalized);
		StartCoroutine(changeWind);
	}


}
