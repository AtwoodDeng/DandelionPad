using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FunctionWindows : MonoBehaviour {

	[SerializeField] Text NumberText;
	[SerializeField] AudioSource clickSound;
//	[SerializeField] Button windButton;
	[SerializeField] Button zoomButton;

	void Update()
	{
		NumberText.text = LogicManager.Instance.RemainBlowTime.ToString();
	}

	float zoomButtonTime;
//	public void OnWindButton()
//	{
//		if ( Time.time -  windButtonTime < windViewInterval  )
//			return;
//		
//		EventManager.Instance.PostEvent(EventDefine.SwitchWind);
//		windButtonTime = Time.time;
//
//		if ( clickSound != null )
//			clickSound.Play();
//
//		if ( windButton != null )
//			windButton.interactable = false;
//		StartCoroutine(ActiveButton(windViewInterval, windButton));
//	}


	IEnumerator ActiveButton(float delay , Button button)
	{
		yield return new WaitForSeconds(delay);
		if ( button != null )
			button.interactable = true;
	}

	public void OnZoomButton()
	{
		if ( Time.time -  zoomButtonTime < Global.zoomFadeTime  )
			return;

		EventManager.Instance.PostEvent(EventDefine.SwitchZoom);
		zoomButtonTime = Time.time;

		if ( clickSound != null )
			clickSound.Play();

		if ( zoomButton != null )
			zoomButton.interactable = false;
		StartCoroutine(ActiveButton(Global.zoomFadeTime , zoomButton));
	}


	public void OnRetryButton()
	{
		EventManager.Instance.PostEvent(EventDefine.Retry );
		if ( clickSound != null )
			clickSound.Play();
	}

	public void OnButtonEnd()
	{
		EventManager.Instance.PostEvent(EventDefine.EndLevel);
		if ( clickSound != null )
			clickSound.Play();
	}
}
