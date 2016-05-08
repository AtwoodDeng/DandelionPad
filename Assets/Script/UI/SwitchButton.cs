using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwitchButton : MonoBehaviour {

	[SerializeField] Sprite OnImage;
	[SerializeField] Sprite OffImage;

	[SerializeField] Image myImage;

	[SerializeField] EventDefine OnEvent;
	[SerializeField] EventDefine OffEvent;
	void Awake()
	{
		if ( myImage == null )
			myImage = GetComponent<Image>();
		if ( myImage != null )
		{
			myImage.sprite = OffImage;
		}
	}
	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(OnEvent , ButtonOn);
		EventManager.Instance.RegistersEvent(OffEvent , ButtonOff);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(OnEvent , ButtonOn);
		EventManager.Instance.UnregistersEvent(OffEvent , ButtonOff);
	}

	void ButtonOn( Message msg )
	{
		if ( myImage != null )
			myImage.sprite = OnImage;
	}

	void ButtonOff( Message msg )
	{
		if ( myImage != null )
			myImage.sprite = OffImage;
	}
}
