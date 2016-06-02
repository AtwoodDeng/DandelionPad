using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WindTestGenerator : MonoBehaviour {

	// *************  UI **************
//	[SerializeField] SpriteRenderer windBackUI;
	[SerializeField] GameObject WindTestPrefab;
//	[SerializeField] AnimationCurve ArrowScaleCurve;
	[SerializeField] protected int windTestNum = 100;
	SpriteRenderer[] UIarrows;
	protected WindTest[] WindTests;
	[SerializeField] bool isStartShow = true;
	[SerializeField] EventDefine showEvent;

	// ============= Connected Wind =============
	[SerializeField] WindAdv wind;
//	Vector3 Size;

	void OnEnable()
	{
//		EventManager.Instance.RegistersEvent(EventDefine.SwitchWind , OnSwithWind);
		EventManager.Instance.RegistersEvent(showEvent , OnShowEvent);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel , OnEndLevel);
	}

	void OnDisable()
	{
//		EventManager.Instance.UnregistersEvent(EventDefine.SwitchWind , OnSwithWind);
		EventManager.Instance.UnregistersEvent(showEvent , OnShowEvent);
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel , OnEndLevel);
	}

	void OnEndLevel( Message msg )
	{
		HideUI();
	}

	void OnSwithWind( Message msg )
	{
		UISwitch();
	}

	void OnShowEvent( Message msg )
	{
		if ( msg.eventName != EventDefine.None )
			ShowUI();	
	}


	public bool UIShowed = false;

	void Start()
	{
		if ( wind == null )
		{
			wind = (WindAdv)FindObjectOfType( typeof( WindAdv));
			
		}
//		if ( wind != null )
//		{
//			Size = wind.GetSize();
//		}

		Init();

		if ( isStartShow )
			ShowUI();

//		if ( isStartShow )
//			ShowUI();
	}

	public void Init()
	{
//		windBackUI.transform.localScale = new Vector3( Size.x * 100f / windBackUI.sprite.texture.width 
//			, Size.y * 100f / windBackUI.sprite.texture.height );
//		windBackUI.DOFade(0,0);
		WindTests = new WindTest[ windTestNum ];
		for ( int i = 0 ; i < windTestNum ; ++ i )
		{
			GameObject windTest = Instantiate( WindTestPrefab ) as GameObject;
			windTest.transform.SetParent( this.transform );
			windTest.SetActive(false);

			WindTests[i] = windTest.GetComponent<WindTest>();
			WindTests[i].wind = wind;
			WindTests[i].parent = this;
		}

		UIShowed = false;
	}

	virtual public void InitWindTestPos()
	{
		for ( int i = 0 ; i < windTestNum ; ++ i )
		{
			ResetWindTest( WindTests[i] );
		}
	}

	virtual public void ResetWindTest( WindTest windTest )
	{
		if ( UIShowed )
			windTest.EnterRandomPosByWind();
	}

	float lastSwitchTime = -Mathf.Infinity;
	public void UISwitch()
	{
		if ( Time.time - lastSwitchTime < 1.9f  )
			return;
		lastSwitchTime = Time.time;

		if ( UIShowed )
			HideUI();
		else 
			ShowUI();
	}
	 public void ShowUI()
	{
		UIShowed = true;

//		windBackUI.DOFade( 0.5f , 2f ).SetEase(Ease.OutExpo);
		InitWindTestPos();

//		EventManager.Instance.PostEvent(EventDefine.ShowWind);

	}

	 public void HideUI()
	{
//		windBackUI.DOFade( 0f , 2f ).SetEase(Ease.Linear);
		for ( int i = 0 ; i < windTestNum ; ++ i )
		{
			WindTests[i].Exit( false );
		}

//		EventManager.Instance.PostEvent(EventDefine.HideWind);

		UIShowed = false;
	}
}
