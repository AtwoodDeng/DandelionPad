using UnityEngine;
using System.Collections;

public class PlatFormControl : MonoBehaviour {
	[SerializeField] RuntimePlatform[] targetPlatform;
	[SerializeField] bool disableOnAwake;


	void Awake()
	{
		if ( disableOnAwake )
		{
			gameObject.SetActive( false );
			foreach( RuntimePlatform platform in targetPlatform )
			{
				if ( Application.platform == platform )
					gameObject.SetActive( true );
			}

		}
	}
}