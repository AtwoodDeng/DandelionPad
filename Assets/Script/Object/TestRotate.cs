using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TestRotate : MonoBehaviour {

	[SerializeField] float leftAngle;
	[SerializeField] float upAngle;
	[SerializeField] float forwardAngle;

	[SerializeField] Vector3 testTo;
	[SerializeField] float angle;

	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler( Vector3.zero );

		transform.Rotate( Vector3.left , leftAngle );
		transform.Rotate( Vector3.forward , forwardAngle );
		transform.Rotate( Vector3.up , upAngle );

		transform.Rotate( testTo , angle );
	}
}
