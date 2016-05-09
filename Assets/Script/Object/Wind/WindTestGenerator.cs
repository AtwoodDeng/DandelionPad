using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WindTestGenerator : MonoBehaviour {

	// *************  UI **************
//	[SerializeField] SpriteRenderer windBackUI;
	[SerializeField] GameObject WindTestPrefab;
//	[SerializeField] AnimationCurve ArrowScaleCurve;
	[SerializeField] int windTestNum = 100;
	SpriteRenderer[] UIarrows;
	WindTest[] WindTests;
	[SerializeField] bool isStartShow = true;

	// ============= Connected Wind =============
	[SerializeField] WindAdv wind;
	Vector3 Size;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.SwitchWind , OnSwithWind);
		EventManager.Instance.RegistersEvent(EventDefine.LevelInitialized , OnLevelInitialized);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel , OnEndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchWind , OnSwithWind);
		EventManager.Instance.UnregistersEvent(EventDefine.LevelInitialized , OnLevelInitialized);
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

	void OnLevelInitialized( Message msg )
	{
		if ( isStartShow )
			ShowUI();	
	}


	public bool UIShowed = false;

	void Start()
	{
		if ( wind == null )
		{
			if ( GameObject.FindGameObjectWithTag("Wind") != null )
				wind = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindAdv>();
			
		}
		if ( wind != null )
		{
			Size = wind.GetSize();
		}

		Init();

		if ( isStartShow )
			ShowUI();
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
			windTest.transform.SetParent( wind.transform );
			windTest.SetActive(false);

			WindTests[i] = windTest.GetComponent<WindTest>();
			WindTests[i].wind = wind;
		}


		UIShowed = false;
	}

	public void InitWindTestPos()
	{
		for ( int i = 0 ; i < windTestNum ; ++ i )
		{
//			int k = 0;
//			Vector3 ranPos = new Vector3 ( Random.Range( - Size.x / 2 , Size.x / 2 ) , Random.Range( - Size.y / 2 , Size.y / 2 ) , 0 ) + wind.transform.position;
//			while( !wind.checkIsObsticle(ranPos) &&  k < 2000 ) 
//			{
//				ranPos = new Vector3 ( Random.Range( - Size.x / 2 , Size.x / 2 ) , Random.Range( - Size.y / 2 , Size.y / 2 ) , 0 ) + + wind.transform.position;
//				++k;
//			}
			WindTests[i].EnterRandomPos();
		}
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

//		windBackUI.DOFade( 0.5f , 2f ).SetEase(Ease.OutExpo);
		InitWindTestPos();

		EventManager.Instance.PostEvent(EventDefine.ShowWind);

		UIShowed = true;
	}

	public void HideUI()
	{
//		windBackUI.DOFade( 0f , 2f ).SetEase(Ease.Linear);

		for ( int i = 0 ; i < windTestNum ; ++ i )
		{

			WindTests[i].Exit();
		}

		EventManager.Instance.PostEvent(EventDefine.HideWind);

		UIShowed = false;
	}

}
