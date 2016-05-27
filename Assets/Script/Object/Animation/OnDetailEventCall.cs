using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class OnDetailEventCall : MonoBehaviour {

	[SerializeField] UnityEvent func;
	[SerializeField] EventDefine triggerEvent;
	[SerializeField] string key;
	[SerializeField] GameObject compareObj;
	[SerializeField] bool equalsGameObj;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(triggerEvent, OnEvent);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(triggerEvent, OnEvent);
	}

	public void OnEvent( Message msg )
	{
		if ( equalsGameObj )
		{
			GameObject obj = (GameObject)msg.GetMessage( key );
			if ( obj == compareObj )
				func.Invoke();
		}
		else
		{
			func.Invoke();
		}
	}

}
