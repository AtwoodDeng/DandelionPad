using UnityEngine;
using System.Collections;

public class OnEventPlaySound : OnEvent {
	[SerializeField] AudioSource source;

	void Awake()
	{
		if ( source == null )
			source = GetComponent<AudioSource>();
		
	}

	protected override void Do (Message msg)
	{
		if ( source != null )
			source.Play();
	}
}
