using UnityEngine;
using System.Collections;


public class Area : MonoBehaviour {
	[SerializeField] protected Collider2D col2D;
	[SerializeField] protected Collider col3D;

	[SerializeField] protected PetalType toType;

	void Awake()
	{
		if ( col2D == null )
			col2D = GetComponent<Collider2D>();
		if ( col3D == null )
			col3D = GetComponent<Collider>();
	}

	void OnTriggerEnter2D(Collider2D col )
	{
		Petal petal = col.GetComponent<Petal>();
		if ( petal != null )
		{
			petal.myGrowInfo.type = toType;
			petal.myGrowInfo.affectPointArea = this;
		}

	}

	void OnTriggerEnter(Collider col )
	{
		Petal petal = col.GetComponent<Petal>();
		if ( petal != null )
		{
			petal.myGrowInfo.type = toType;
			petal.myGrowInfo.affectPointArea = this;
		}

	}



}
