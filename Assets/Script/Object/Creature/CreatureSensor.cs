using UnityEngine;
using System.Collections;

public class CreatureSensor : SenseGuesture {
	[SerializeField] Creature c;

	void Awake()
	{
		
	}

	public override void DealTap (TapGesture guesture)
	{
		if ( c == null && transform.parent != null )
			c = transform.parent.GetComponent<Creature>();

		Debug.Log( "On Finger Down");
		base.DealTap( guesture );
		if ( c != null )
			c.OnFingerDown( guesture.Position );
	}
}
