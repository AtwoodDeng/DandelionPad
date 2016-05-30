using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CameraEffect : MonoBehaviour {

	[SerializeField] GameObject inkPrefab;
	[SerializeField] GameObject inkSpreadPrefab;
	[SerializeField] GameObject inkTrailPrefab;
	[SerializeField] float trailFadeOutTime;
	SplineTrailRenderer temInkTrail;
	Dictionary<int,Ink> inkDict = new Dictionary<int, Ink>(); // key (int) for finger Index; Ink for the sprite corresponse
	[SerializeField] bool ifUseInk = false;
	[SerializeField] bool ifUseInkTrail = false;
	bool m_enable = true;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.SwitchSetting , OnSwitchSetting );
		EventManager.Instance.RegistersEvent( EventDefine.BlowFlower , OnBlowFlower );
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchSetting , OnSwitchSetting );
		EventManager.Instance.UnregistersEvent( EventDefine.BlowFlower , OnBlowFlower );
	}

	void OnBlowFlower( Message msg )
	{
		if ( msg.ContainMessage( "petal0" ) )
		{
			Petal p = (Petal) msg.GetMessage( "petal0" );
			CreateInkSpread( p.transform.position );
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
	void CreateInkSpread( Vector3 posFinger , FingerGestures.Finger finger )
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

	void CreateInkSpread( Vector3 worldPos )
	{
		GameObject ink = Instantiate ( inkSpreadPrefab ) as GameObject;
		ink.transform.parent = LogicManager.LevelManager.GetLevelObject().transform;
		worldPos.z = 0 ;
		ink.transform.position = worldPos;

		Ink inkCom = ink.GetComponent<Ink>();
		inkCom.Fade();
		
	}

	void CreateInkTrail( Vector3 posFinger )
	{
		Debug.Log("Create Ink ");
		GameObject ink = Instantiate( inkTrailPrefab ) as GameObject;
		ink.transform.SetParent( transform , true );
		Vector3 pos = Camera.main.ScreenToWorldPoint( posFinger );
		pos .z = 0;
		ink.transform.position = pos;

		temInkTrail = ink.GetComponent<SplineTrailRenderer>();
		
	}


	void OnFingerUpBack( FingerUpEvent e )
	{
		if ( !m_enable ) return;
		if ( ifUseInk )
		{
		if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
		{
			inkDict[e.Finger.Index].Fade();
			inkDict[e.Finger.Index] = null;
		}
		}
	}

	void OnFingerMoveBack( FingerMotionEvent e )
	{
		if ( !m_enable ) return;
		Debug.Log("On Finger Move Back " + e.Phase );
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
				if ( temInkTrail == null )
				{
					CreateInkTrail( e.Position );
				}
			}else if ( e.Phase == FingerMotionPhase.Updated )
			{
				if ( temInkTrail != null )
				{
				Vector3 pos = Camera.main.ScreenToWorldPoint( e.Position );
				pos.z = 0;
				temInkTrail.transform.position = pos;
				}

			}else 
			{
				if ( temInkTrail != null )
				{
					temInkTrail.FadeOut( trailFadeOutTime );
					temInkTrail = null;
				}
			}
		}
	}

	void OnFingerStationaryBack( FingerMotionEvent e )
	{
		if ( !m_enable ) return;
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

	void OnFingerDownBack( FingerDownEvent e )
	{
		if ( !m_enable ) return;
		if ( e.Finger.Phase == FingerGestures.FingerPhase.Begin )
		{
			
			if ( enabled && CameraManager.Instance.State == CameraState.Free )
			{
				if ( ifUseInk )
				{
					CreateInkSpread( e.Position , e.Finger );
				}
			}
			else 
			{
				EventManager.Instance.PostEvent( EventDefine.UnableToMove );
			}
		}
	}

}
