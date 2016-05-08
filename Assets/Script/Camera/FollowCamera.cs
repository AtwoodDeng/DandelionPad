using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

//	[SerializeField] Vector3 followRate;
//	
//	Vector3 initPos;
//	Vector3 camInitPos;
//
//	void OnBecameVisible()
//	{
//		enabled = true;
//		initPos = transform.position;
//		camInitPos = CameraManager.Instance.offset;
//	}
//
//	void OnBecameInvisible()
//	{
//		enabled = false;
//	}
//
//	// Update is called once per frame
//	void LateUpdate () {
//		// difference of the position of the camera
//		Vector3 cameraDiff = CameraManager.Instance.offset - camInitPos;
//		// difference of the position with the original position, don't change z position
//		Vector3 posiDiff = new Vector3(- cameraDiff.x * followRate.x , - cameraDiff.y * followRate.y , 0 );
//
//		transform.position = initPos + posiDiff;
//	}
}
