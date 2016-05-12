using UnityEngine;
using System.Collections;

public class RockDistanceSensor : MonoBehaviour {

	[SerializeField] GameObject anotherRock;

	[SerializeField] float departDistance;
	[SerializeField] float closeDistance;

	float initDis;
	void Start()
	{
		lastDis = initDis = (anotherRock.transform.position - transform.position).magnitude;
	}
	float lastDis;
	[SerializeField] bool isDeparted;
	// Update is called once per frame
	void Update () {
		float distance = (anotherRock.transform.position - transform.position).magnitude;
		if ( lastDis > closeDistance + initDis && distance < closeDistance + initDis && isDeparted)
		{
			EventManager.Instance.PostEvent( EventDefine.RockClose );
			isDeparted = false;
		}

		if ( lastDis < departDistance + initDis && distance > departDistance + initDis && !isDeparted )
		{
			EventManager.Instance.PostEvent( EventDefine.RockDepart );
			isDeparted = true;
		}

		lastDis = distance;
	}
}
