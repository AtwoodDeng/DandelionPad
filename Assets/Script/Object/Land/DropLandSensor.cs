using UnityEngine;
using System.Collections;

public class DropLandSensor : SenseGuesture {

	[SerializeField] DropLand parent;

	void Awake()
	{
		if ( parent == null )
		{
			parent = GetComponentInParent<DropLand>();
		}
	}

	public override void DealTap (TapGesture guesture)
	{
		parent.DealTap(guesture);
	}

}
