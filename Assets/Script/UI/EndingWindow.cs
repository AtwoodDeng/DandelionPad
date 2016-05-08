using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class EndingWindow : MonoBehaviour {

	[SerializeField] Image backImage;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel,OnEndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel,OnEndLevel);
		
	}

	void OnEndLevel(Message msg)
	{
		
	}
}
