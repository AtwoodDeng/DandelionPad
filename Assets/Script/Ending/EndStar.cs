using UnityEngine;
using System.Collections;

public class EndStar : MonoBehaviour {
	[SerializeField] Star[] stars;

	void Awake()
	{
		if ( stars == null || stars.Length < 3 ) {
			stars = GetComponentsInChildren<Star>();
		}
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, EndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, EndLevel);
	}

	void EndLevel(Message msg )
	{
		int evalua = LogicManager.LevelManager.GetEvaluation();
		for( int i = 0 ; i < evalua ; ++ i ) 
		{
			stars[i].SetActive();
		}
		for( int i = evalua ; i < stars.Length ; ++ i )
		{
			stars[i].SetInactive();
		}
	}
}
