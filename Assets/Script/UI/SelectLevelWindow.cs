using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class SelectLevelWindow : MonoBehaviour {

	[SerializeField] Button[] buttons;
	[SerializeField] VerticalLayoutGroup panel;
	[SerializeField] Color[] colors;
	[SerializeField] float fadeTime;
	[SerializeField] Button SelectLevelButton;


	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel,OnEndLevel);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel,OnEndLevel);
	}

	void OnEndLevel( Message msg )
	{
		HideButton( fadeTime );
		SelectLevelButton.image.DOFade( 0 ,fadeTime );
	}

	void Awake()
	{
		if ( buttons == null || buttons.Length < 1)
		{
			buttons = panel.GetComponentsInChildren<Button>();
		}

		InitButtons(buttons);
		HideButton(0);
	}

	void InitButtons(Button[] btns)
	{
		for( int i = 0 ; i < btns.Length ; ++ i )
		{
			Text text = btns[i].GetComponentInChildren<Text>();
			if ( text != null )
			{
				if ( colors.Length > i )
					text.color = Color.Lerp( colors[i] , Color.white , 0.4f );
			}

			if ( colors.Length > i )
			{
				btns[i].image.color = Color.Lerp( colors[i] , Color.black , 0.4f );
			}
		}
	}

	void FadeButtons(Button[] btns , float to , float time )
	{
		for( int i = 0 ; i < btns.Length ; ++ i )
		{
			btns[i].image.DOFade( to , time );
			btns[i].GetComponentInChildren<Text>().DOFade( to , time );
		}
	}

	bool isShown = false;

	public void OnSwitch()
	{
		if ( isShown )
			HideButton(fadeTime);
		else
			ShowButton(fadeTime);
	}

	public void ShowButton( float time )
	{
		FadeButtons( buttons , 1f , time  );
		isShown = true;
	}

	public void HideButton( float time )
	{
		FadeButtons( buttons , 0 , time);
		isShown = false;
	}

	public void OnGotoLevel( int i )
	{
		Message msg = new Message();
		msg.AddMessage("to" , i );
		EventManager.Instance.PostEvent(EventDefine.EndLevel , msg );

		HideButton( fadeTime );
	}
}
