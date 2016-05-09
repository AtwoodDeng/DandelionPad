using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using DG.Tweening;

public class FunctionWindows : MonoBehaviour {

	[SerializeField] Text NumberText;
	[SerializeField] AudioSource clickSound;
//	[SerializeField] Button windButton;
	[SerializeField] Button zoomButton;
	[SerializeField] Button retryButton;


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

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.LevelDead,OnLevelDead);
	}

	void Disable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.LevelDead,OnLevelDead);
	}

	void OnLevelDead( Message msg )
	{
		Sequence seq = DOTween.Sequence();
		seq.Append( retryButton.transform.DORotate(new Vector3(0,0,360f),2f).SetRelative(true).SetEase(Ease.InOutCubic));
		seq.AppendInterval( 0.4f );
		seq.SetLoops( 9999 , LoopType.Incremental );
	}

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
