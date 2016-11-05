using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using DG.Tweening;

public class FunctionWindows : MonoBehaviour {
	
	[SerializeField] AudioSource clickSound;
	[SerializeField] AudioSource windSound;
	[SerializeField] Button windButton;
	[SerializeField] Button zoomButton;
	[SerializeField] Button retryButton;
	[SerializeField] Button[] ArrowButtons;
	[SerializeField] Button eyeButton;
	[SerializeField] Image loadLevelImage;
	[SerializeField] Text loadLevelText;

	[SerializeField] GameObject setting;
	[SerializeField] Slider musicVolumeSlider;
	[SerializeField] float windSwitchTime = 1f;

	float zoomButtonTime;

	float windButtonTime;
	bool windTipsActive = false;
	public void OnWindButton()
	{
		if ( Time.time -  windButtonTime < windSwitchTime  )
			return;

		windTipsActive = !windTipsActive;

		Message msg = new Message();
		msg.AddMessage( "WindActive" , windTipsActive );
		msg.AddMessage( "time" , windSwitchTime );
		EventManager.Instance.PostEvent(EventDefine.SwitchWind , msg);
		windButtonTime = Time.time;

		if ( clickSound != null )
			clickSound.Play();

		if ( windButton != null )
			windButton.interactable = false;

		if ( windSound != null )
			windSound.Play();
		
		StartCoroutine(ActiveButton(windSwitchTime, windButton));
	}

	bool canMove = false;
	public void MoveBegin()
	{
		canMove = true;
	}

	public void MoveEnd()
	{
		canMove = false;

	}
	public void OnButtonMoveVertical( float delta )
	{
		if ( canMove )
		CameraManager.Instance.MoveCamera( new Vector2( 0 , delta ) );
	}

	public void OnButtonMoveHorizontal( float delta )
	{
		if ( canMove )
		CameraManager.Instance.MoveCamera( new Vector2( delta , 0 ) );
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.LevelDead,OnLevelDead);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel,OnLevelEnd);
		EventManager.Instance.RegistersEvent(EventDefine.BeginLevel,OnLevelBegin);
	}

	void Disable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.LevelDead,OnLevelDead);
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel,OnLevelEnd);
		EventManager.Instance.UnregistersEvent(EventDefine.BeginLevel,OnLevelBegin);
	}

	void OnLevelBegin( Message msg )
	{

		Button[] buttons = GetComponentsInChildren<Button>();
		foreach( Button btn in buttons )
		{
			btn.image.DOFade( 0 , 2f ).From();
		}
	}

	void OnLevelEnd( Message msg )
	{
		Button[] buttons = GetComponentsInChildren<Button>();
		foreach( Button btn in buttons )
		{
			btn.image.DOFade( 0 , 2f );
		}
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
		StartCoroutine(ActiveButton(Global.zoomFadeTime * 0.8f  , zoomButton));
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

	public void OnButtonLast()
	{
		Message msg = new Message();
		msg.AddMessage("to" ,Global.LastLevelIndex()  );
		EventManager.Instance.PostEvent(EventDefine.EndLevel,msg);
		if ( clickSound != null )
			clickSound.Play();
		
	}

	void Awake()
	{
		setting.SetActive(false);

		windButton.interactable = true;
		foreach( string name in Global.inactiveWindLevel )
			if ( name == SceneManager.GetActiveScene().name )
			{
				windButton.interactable = false;
			}

		zoomButton.interactable = true;
		foreach( string name in Global.inactiveZoomLevel )
			if ( name == SceneManager.GetActiveScene().name )
			{
				zoomButton.interactable = false;
			}
		
	}

	bool isSetting = false;
	public void OnGearButton()
	{
		if ( clickSound != null )
			clickSound.Play();

		isSetting = !isSetting;

		// background image
		// backCover.DOFade( isSetting? 0.5f : 0 , settingFadeTime );

		musicVolumeSlider.value = AudioListener.volume;


		Message msg = new Message();
		msg.AddMessage( "isSetting" , isSetting );
		EventManager.Instance.PostEvent( EventDefine.SwitchSetting , msg , this );

		if ( isSetting )
		{
			setting.SetActive(true);
		}else
		{
			setting.SetActive(false);
		}

	}

	public void OnMusicVolumeChange( float vol )
	{
		if ( isSetting )
			AudioListener.volume = musicVolumeSlider.value;
	}

	public void OnMenu()
	{
		SceneManager.LoadScene( "begin" );
	}

	public void SetLoadLevelImage( Sprite sprite )
	{
		if ( loadLevelImage != null )
		{
			loadLevelImage.sprite = sprite;
			loadLevelImage.enabled = true;
			loadLevelImage.DOFade( 0 , 0 );
			loadLevelImage.DOFade( 1f , 1.3f );
		}
		if ( loadLevelText!= null )
			loadLevelText.enabled = true;
	}
}
