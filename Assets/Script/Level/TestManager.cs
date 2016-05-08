using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestManager : MonoBehaviour {


	[SerializeField] List<GameObject> levels;
	[SerializeField] GameObject Final;

	void Update()
	{
		if (Input.GetKey(KeyCode.Alpha1))
		{
			ClossAllLevel();
			levels[0].SetActive(true);
			Message msg = new Message();
			msg.AddMessage("time", 1);
			EventManager.Instance.PostEvent(EventDefine.RenewSwipeTime, msg);
		}

		if (Input.GetKey(KeyCode.Alpha2))
		{
			ClossAllLevel();
			levels[1].SetActive(true);
			Message msg = new Message();
			msg.AddMessage("time", 3);
			EventManager.Instance.PostEvent(EventDefine.RenewSwipeTime, msg);
		}

		if (Input.GetKey(KeyCode.Alpha3))
		{
			ClossAllLevel();
			levels[2].SetActive(true);
			Message msg = new Message();
			msg.AddMessage("time", 1);
			EventManager.Instance.PostEvent(EventDefine.RenewSwipeTime, msg);
		}
	}

	void ClossAllLevel()
	{
		foreach(GameObject obj in levels)
		{
			obj.SetActive(false);

		}
		Final.SetActive(false);

	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EnterFinal,EnterFinal);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EnterFinal,EnterFinal);

	}

	void EnterFinal(Message msg)
	{
		Final.SetActive(true);
	}
}
