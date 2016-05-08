using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float Boundary = 0.1f;
	public float speed = 5f;
	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 move = Vector3.zero;
		if (Input.mousePosition.x > Screen.width *(1f -Boundary))
	     {
	         move.x += speed * Time.deltaTime;
	     }
	     
	     if (Input.mousePosition.x < Screen.width *(Boundary))
	     {
	         move.x -= speed * Time.deltaTime;
	     }
	     
	     if (Input.mousePosition.y > Screen.height * ( 1f - Boundary))
	     {
	         move.y += speed * Time.deltaTime;
	     }
	     
	     if (Input.mousePosition.y < Screen.height * Boundary )
	     {
	         move.y -= speed * Time.deltaTime;
	     }
     	transform.position += move;
	}
}
