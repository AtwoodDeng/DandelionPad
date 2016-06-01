using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CameraEffect : MonoBehaviour {

	[SerializeField] GameObject inkPrefab;
	[SerializeField] GameObject inkSpreadPrefab;
	[SerializeField] GameObject inkTrailPrefab;
	[SerializeField] GameObject inkCirclePrefab;
	[SerializeField] float trailFadeOutTime;
//	SplineTrailRenderer temInkTrail;
	Dictionary<int,Ink> inkDict = new Dictionary<int, Ink>(); // key (int) for finger Index; Ink for the sprite corresponse
	Dictionary<int,SplineTrailRenderer> inkTrailDict = new Dictionary<int, SplineTrailRenderer>(); // key (int) for finger Index; SplineTrailRender for the the trail
	List<InkSpread> inkSpreadList = new List<InkSpread>();
	List<InkCircle> inkCircleList = new List<InkCircle>();
	[SerializeField] bool ifUseInk = false;
	[SerializeField] bool ifUseInkTrail = false;
	bool m_enable = true;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.SwitchSetting , OnSwitchSetting );
		EventManager.Instance.RegistersEvent( EventDefine.BlowFlower , OnBlowFlower );
		EventManager.Instance.RegistersEvent( EventDefine.EndLevel , OnLevelEnd );
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchSetting , OnSwitchSetting );
		EventManager.Instance.UnregistersEvent( EventDefine.BlowFlower , OnBlowFlower );
		EventManager.Instance.UnregistersEvent( EventDefine.EndLevel , OnLevelEnd );
	}

	void Start()
	{
		for( int i = 0; i < 12 ; ++ i)
		{
			GameObject ink = Instantiate ( inkSpreadPrefab ) as GameObject;
			ink.transform.parent = LogicManager.LevelManager.GetLevelObject().transform;
		
			InkSpread inkCom = ink.GetComponent<InkSpread>();

			inkSpreadList.Add( inkCom );
			inkCom.gameObject.SetActive( false );
		}


		for( int i = 0; i < 3 ; ++ i)
		{
			GameObject ink = Instantiate ( inkCirclePrefab ) as GameObject;
			ink.transform.SetParent( transform , true );

			InkCircle inkCom = ink.GetComponent<InkCircle>();

			inkCircleList.Add( inkCom );
			inkCom.gameObject.SetActive( false );
		}
	}

	void OnLevelEnd( Message msg )
	{
		FadeOutInkSpread();
		FadeOutAllInkCircle();
	}

	void OnBlowFlower( Message msg )
	{
		if ( msg.ContainMessage( "petal0" ) )
		{
			FadeOutInkSpread();
			Transform parent = transform;
			Flower flower = (Flower)msg.GetMessage("Flower");
			if ( flower != null )
				parent = flower.transform;
			Petal p = (Petal) msg.GetMessage( "petal0" );
			Vector2 vel = (Vector2) msg.GetMessage( "Velocity");
			CreateInkSpread( p.transform.position , vel , parent);
		}

	}

	void OnSwitchSetting( Message msg )
	{
		bool isSetting = (bool)msg.GetMessage("isSetting");
		m_enable = !isSetting;
	}

	/// <summary>
	/// Creats the ink. Create the in by the position 
	/// </summary>
	/// <param name="posFinger">Position to create the ink.</param>
	/// <param name="finger">Which Finger .</param>
	void CreateInk( Vector3 posFinger , FingerGestures.Finger finger )
	{
		GameObject ink = Instantiate ( inkPrefab ) as GameObject;
		ink.transform.parent = LogicManager.LevelManager.GetLevelObject().transform;
		Vector3 pos = Camera.main.ScreenToWorldPoint( posFinger );
		pos .z = 0;
		ink.transform.position = pos;

		Ink inkCom = ink.GetComponent<Ink>();

		if ( inkDict.ContainsKey( finger.Index ))
		{
			if (  inkDict[finger.Index] != null )
				inkDict[finger.Index].Fade();
			inkDict[finger.Index] = inkCom;
		}
		else
			inkDict.Add( finger.Index , inkCom );
	}

	void CreateInkSpread( Vector3 worldPos , Vector2 velocity , Transform parent )
	{
		Debug.Log( inkSpreadList.Count );
		InkSpread.CreateInkSpreadBlowFlower( inkSpreadList , worldPos , velocity , parent);
	}

	void FadeOutInkSpread()
	{
		foreach( InkSpread ink in inkSpreadList)
		{
			if ( ink.gameObject.activeSelf)
				ink.Fade(  );
		}
	}

	void CreateInkTrail( Vector3 posFinger , FingerGestures.Finger finger )
	{
		GameObject ink = Instantiate( inkTrailPrefab ) as GameObject;
		ink.transform.SetParent( transform , true );
		Vector3 pos = Camera.main.ScreenToWorldPoint( posFinger );
		pos .z = 0;
		ink.transform.position = pos;


		SplineTrailRenderer temInkTrail = ink.GetComponent<SplineTrailRenderer>();
		temInkTrail.vertexColor = LogicManager.TrailColor;

		if ( inkTrailDict.ContainsKey( finger.Index ))
		{
			if (  inkTrailDict[finger.Index] != null && inkTrailDict[finger.Index].gameObject.activeSelf )
			{
				inkTrailDict[finger.Index].FadeOut(trailFadeOutTime);
			}
			inkTrailDict[finger.Index] = temInkTrail;
		}
		else
			inkTrailDict.Add( finger.Index , temInkTrail );


	}

	void UpdateInkTrail( Vector2 ScreenPos ,  FingerGestures.Finger finger )
	{
		SplineTrailRenderer str;
		if ( inkTrailDict.TryGetValue( finger.Index , out str ))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint( ScreenPos );
			pos.z = 0;
			str.UpdatePosition( pos );
		}
	
	}

	void FadeOutInkTrail(  FingerGestures.Finger finger )
	{
		SplineTrailRenderer str;
		if ( inkTrailDict.TryGetValue( finger.Index , out str ) )
		{
			if ( str.gameObject.activeSelf )
				str.FadeOut(trailFadeOutTime);
		}
	}

	/// <summary>
	/// Called by finger up detector
	/// </summary>
	/// <param name="e">E.</param>
	void OnFingerUpBack( FingerUpEvent e )
	{

		if ( !m_enable || LogicManager.isSetting  ) return;
		if ( ifUseInk )
		{
		if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
		{
			inkDict[e.Finger.Index].Fade();
			inkDict[e.Finger.Index] = null;
		}
		}
	}

	/// <summary>
	/// Called by finger motion detetor
	/// </summary>
	/// <param name="e">E.</param>
	void OnFingerMoveBack( FingerMotionEvent e )
	{

		if ( !m_enable || LogicManager.isSetting  ) return;
//		Debug.Log("On Finger Move Back " + e.Phase );
		// deal with ink
		if ( ifUseInk )
		{
			if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
			{
				if ( e.Finger.DistanceFromStart < inkDict[e.Finger.Index].affectRange() ) {
					inkDict[e.Finger.Index].Spread(Time.deltaTime);
				}
				else {
					inkDict[e.Finger.Index].Fade();
					inkDict[e.Finger.Index] = null;
				}	
			}
		}

		// deal with ink trail
		if ( ifUseInkTrail )
		{
			if ( e.Phase == FingerMotionPhase.Started && CameraManager.Instance.State == CameraState.Free )
			{
					CreateInkTrail( e.Position , e.Finger);
			}else if ( e.Phase == FingerMotionPhase.Updated )
			{
				UpdateInkTrail( e.Position , e.Finger );

			}else 
			{
				FadeOutInkTrail( e.Finger );
			}
		}
	}

	/// <summary>
	/// Called by Finger motion detector
	/// </summary>
	/// <param name="e">E.</param>
	void OnFingerStationaryBack( FingerMotionEvent e )
	{

		if ( !m_enable || LogicManager.isSetting  ) return;
		if ( e.Phase == FingerMotionPhase.Updated )
		{
			if ( ifUseInk )
			{
				if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
				{
					if ( e.Finger.DistanceFromStart < inkDict[e.Finger.Index].affectRange() ) {
						inkDict[e.Finger.Index].Spread(Time.deltaTime);
					}
					else {
						inkDict[e.Finger.Index].Fade();
						inkDict[e.Finger.Index] = null;
					}	
				}
			}
		}
	}

	/// <summary>
	/// Called by finger down detector
	/// </summary>
	/// <param name="e">E.</param>
	void OnFingerDownBack( FingerDownEvent e )
	{

		if ( !m_enable || LogicManager.isSetting  ) return;
		if ( e.Finger.Phase == FingerGestures.FingerPhase.Begin )
		{
			
			if ( enabled && CameraManager.Instance.State == CameraState.Free )
			{
				if ( ifUseInk )
				{
					CreateInk( e.Position , e.Finger );
				}
			}
			else 
			{
				EventManager.Instance.PostEvent( EventDefine.UnableToMove );
			}
		}
	}

	/// <summary>
	/// Called by finger pinch detector
	/// </summary>
	/// <param name="guesture">Guesture.</param>
	void OnPinchBack(  PinchGesture guesture )
	{
		if ( !m_enable || LogicManager.isSetting  ) return;

		if ( CameraManager.Instance.State == CameraState.Free )
		{
			if ( guesture.Phase == ContinuousGesturePhase.Started )
			{
				if ( GetTempInkCircle() != null )
				{
					GetTempInkCircle().SetColor(LogicManager.TrailColor);
					GetTempInkCircle().FadeIn();
					GetTempInkCircle().UpdateCircle( guesture.Position , guesture.Gap );
				}

			}else if ( guesture.Phase == ContinuousGesturePhase.Updated )
			{
				GetTempInkCircle().UpdateCircle( guesture.Position , guesture.Gap );
			}else if ( guesture.Phase == ContinuousGesturePhase.Ended )
			{
				FadeOutTemInkCircle();
			}
		}else
		{
			EventManager.Instance.PostEvent( EventDefine.UnableToMove );
		}
	}

	int inkCircleIndex = 0;
	InkCircle GetTempInkCircle()
	{
		inkCircleIndex = inkCircleIndex % inkCircleList.Count;
		return inkCircleList[inkCircleIndex];
	}

	void FadeOutTemInkCircle()
	{
		GetTempInkCircle().FadeOut();
		inkCircleIndex ++;
	}

	void FadeOutAllInkCircle()
	{
		for( int i = 0 ; i < inkCircleList.Count ; ++ i )
		{
			if ( inkCircleList[i].gameObject.activeSelf )
				inkCircleList[i].FadeOut();
		}
	}

}
