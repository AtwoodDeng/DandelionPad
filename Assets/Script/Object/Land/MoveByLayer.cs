using UnityEngine;
using System.Collections;

public class MoveByLayer : MonoBehaviour {

	[SerializeField] float PostLayer;

	float m_layer;
	Vector3 oriPosition;

	void Start()
	{
		m_layer = (PostLayer) * 0.1f;
		oriPosition = transform.localPosition;
	}

	void Update()
	{
		UpdatePosition();
	}

	void UpdatePosition()
	{
		transform.localPosition = oriPosition + LogicManager.CameraManager.OffsetFromInit * ( m_layer );
	}
}
