using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class CameraManager : MonoBehaviour {

	public CameraManager() { s_Instance = this; }
	public static CameraManager Instance { get { return s_Instance; } }
	private static CameraManager s_Instance;

	[SerializeField] CameraState m_state;
	public CameraState State
	{
		get {
			return m_state;
		}
	}

//	public float Boundary = 0.1f;
//	public float speed = 5f;

	[SerializeField] GameObject inkPrefab;
	Dictionary<int,Ink> inkDict = new Dictionary<int, Ink>(); // key (int) for finger Index; Ink for the sprite corresponse

	public Vector3 frame;
	public Vector3 frameOffset;
	public Vector3 initPos = Vector3.zero;
	public float senseIntense = 0.2f;

	[SerializeField] float EndFadeSize = 10f;
	[SerializeField] float EndFadeTime = 2f;
	[SerializeField] float EndFadeDelay = 0;
	[SerializeField] float CameraFollowFadeTime = 0.75f;
	[SerializeField] float MaxFollowTime = 8f;

	[SerializeField] float normalOtherSize = 7f;
	[SerializeField] float focusOtherSize = 4f;
	[SerializeField] float ZoomOutOtherSize = 10f;
	[SerializeField] float PinchSense = -0.02f;

	[SerializeField] float focusTolerance = 5f;
	[SerializeField] bool ifSendMoveMessage = false;
	/// <summary>
	/// frame tolerance of the world (works on petal3d check the frame of the level)
	/// </summary>
	[SerializeField] float FrameTolarance = -2f;

	[SerializeField] bool isZoomOutTutorial = false;

	[SerializeField] float orth2FieldK = 5f;
	[SerializeField] float orth2FieldA = 50f;
	[SerializeField] bool isLockY = false;


	Vector3 frameSize;
	public Vector3 FrameSize
	{
		get {
			return frameSize;
		}
	}


	bool m_enable = true;

	public Vector3 OffsetFromInit{
		get {
			Vector3 tem;
			if ( m_state == CameraState.Free )
				tem = LogicManager.LevelManager.GetLevelObject().transform.position - initPos;
			else
				tem = levelPosStartFollow - initPos;
			tem.z = 0;
			return  tem;
		}
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn , OnGrowFlower );
		EventManager.Instance.RegistersEvent(EventDefine.GrowFinalFlower , OnGrowFinalFlower );
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower , OnBlow);
		EventManager.Instance.RegistersEvent(EventDefine.PetalDestory , OnPetalDestory);
		EventManager.Instance.RegistersEvent(EventDefine.BloomFlower , OnBloomFlower);
		EventManager.Instance.RegistersEvent(EventDefine.SwitchZoom , OnSwitchZoom);
		EventManager.Instance.RegistersEvent(EventDefine.SwitchSetting , OnSwitchSetting );
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn , OnGrowFlower );
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFinalFlower , OnGrowFinalFlower );
		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower , OnBlow);
		EventManager.Instance.UnregistersEvent(EventDefine.PetalDestory , OnPetalDestory);
		EventManager.Instance.UnregistersEvent(EventDefine.BloomFlower , OnBloomFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchZoom , OnSwitchZoom);
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchSetting , OnSwitchSetting );
	}

	void OnGrowFinalFlower( Message msg )
	{
		if ( State == CameraState.FollowTrans )
			StartFollowTime = Time.time + 5f ;
		
	}

	void OnGrowFlower( Message msg )
	{
		Petal petal = (Petal)msg.sender;
		if ( petal != null )
		{
			for( int i = focusPetals.Count -1 ; i >= 0 ; --i )
			{
				if ( focusPetals[i].state != PetalState.LandGrow )
					focusPetals.RemoveAt( i );
			}
			focusPetals.Add( petal );
			if ( State == CameraState.FollowTrans )
				StartFollowTime = Time.time;
		}
	}

	void OnSwitchSetting( Message msg )
	{
		bool isSetting = (bool)msg.GetMessage("isSetting");
		m_enable = !isSetting;
	}

	List<Petal> focusPetals = new List<Petal>();
	void OnBlow(Message msg )
	{
		if ( !m_enable ) return;
		int i = 0 ;

		List<Petal> newPetal = new List<Petal>();

		while ( msg.ContainMessage( "petal" + i.ToString()))
		{
			Petal p = (Petal) msg.GetMessage("petal" + i.ToString());
			if ( p.state == PetalState.Fly )
				newPetal.Add( p );
			i++;
		}

		focusPetals.AddRange(newPetal);

		if ( newPetal.Count > 0 )
			CameraStartFollow( newPetal[ Random.Range( 0, newPetal.Count)].transform );
	}

	void OnBloomFlower( Message msg )
	{
		if ( !m_enable ) return;
		Petal p = msg.sender as Petal;
	
//		CameraStopFollow();

		if ( p != null )
		{
			for ( int i = 0 ; i < focusPetals.Count ; ++ i )
			{
				if ( focusPetals[i].ID == p.ID )
				{
					focusPetals.Clear();
					break;
				}
			}
		}
	}

	void OnPetalDestory(Message msg )
	{
		if ( msg.ContainMessage( "petal" ) )
		{
			Petal p = msg.GetMessage( "petal" ) as Petal;
			for ( int i = 0 ; i < focusPetals.Count ; ++ i )
			{
				if ( focusPetals[i].ID == p.ID  )
				{
				// do this if the petal failed to grow the flower
					if ( msg.ContainMessage("FailToGrow" ))
					{
						focusPetals.Clear();
						CameraStopFollow();
						break;
					}
				}
			}
		}
	}


	void OnSwitchZoom( Message msg )
	{
		ZoomSwitch();
	}

	[SerializeField] Vector3 levelPosStartFollow;
	[SerializeField] float OthrSizeStartFollow;
	void CameraStartFollow(Transform trans )
	{
		if ( !m_enable ) return;
		if ( ( m_state == CameraState.Free || m_state == CameraState.Disable ) && StartFollowTime == Mathf.Infinity )
		{
			Debug.Log("Follow " + trans.name );
			RecordLevelPosition();
			StartFollowTime = Time.time;
			m_state = CameraState.FollowTrans;

			VignetteAndChromaticAberration effect = GetComponentInChildren<VignetteAndChromaticAberration>();
			if ( effect != null )
			{
				//TODO remove the hard code effect intensity
				DOTween.To( () => effect.intensity , (x) => effect.intensity = x , 0.38f , CameraFollowFadeTime ).SetEase(Ease.InCubic);
			}
		}
	}

	float StopFollowTime = Mathf.Infinity;
	void CameraStopFollow()
	{
		if ( !m_enable ) return;
		if ( m_state == CameraState.FollowTrans && StopFollowTime == Mathf.Infinity )
		{
			Debug.Log("Stop Follow Camera");

			Sequence seq = DOTween.Sequence();

			AllCameraDoOrthoSize( normalOtherSize , CameraFollowFadeTime , 0 , Ease.OutCubic );
			VignetteAndChromaticAberration effect = GetComponentInChildren<VignetteAndChromaticAberration>();
			if ( effect != null )
			{
				seq.Join( DOTween.To( () => effect.intensity , (x) => effect.intensity = x , 0.24f , CameraFollowFadeTime ).SetEase(Ease.InCubic));
			}
			seq.Join(LogicManager.LevelManager.GetLevelObject().transform.DOMove(levelPosStartFollow,CameraFollowFadeTime));
			seq.AppendInterval( 0.1f );
			seq.AppendCallback(CameraStopFollowComplete);

			StopFollowTime = Time.time;
			StartFollowTime = Mathf.Infinity;
		}
	}

	void AllCameraDoOrthoSize( float to , float duration, float delay , Ease easeType )
	{
		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			if ( c.orthographic ) 
				c.DOOrthoSize( to , duration ).SetEase(easeType).SetDelay(delay);
			else
				c.DOFieldOfView( to * orth2FieldK + orth2FieldA , duration ).SetEase(easeType).SetDelay(delay);
		}
	}

	public void AllCameraSetOrthoSize( float _to  )
	{
		float to = Mathf.Clamp( _to , focusOtherSize , ZoomOutOtherSize );

		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			if ( c.orthographic )
				c.orthographicSize = to;
			else 
				c.fieldOfView = to * orth2FieldK + orth2FieldA;
		}
	}

	float GetCameraOrthoSize()
	{
		return Camera.main.orthographicSize;
	}

	void CameraStopFollowComplete()
	{
		if ( m_state == CameraState.FollowTrans )
		{
			m_state = CameraState.Free;
			StopFollowTime = Mathf.Infinity;
		}
	}

	void OnFirstFlower( Message msg )
	{
		if ( !m_enable ) return;
		m_state = CameraState.Free;
	}

	void OnEndLevel(Message msg )
	{
		if ( !m_enable ) return;
		RecordLevelPosition();
		//fade this camera
		Sequence seq = DOTween.Sequence();
		seq.AppendInterval( EndFadeDelay );
		seq.AppendCallback( EndLevelDelayAction );
		seq.Append( GetComponent<Camera>().DOOrthoSize( EndFadeSize , EndFadeTime ));
		seq.Append( LogicManager.LevelManager.GetLevelObject().transform.DOMove( Vector3.zero , EndFadeTime ));

		AllCameraDoOrthoSize( EndFadeSize , EndFadeTime , EndFadeDelay , Ease.Linear );
	}

	void EndLevelDelayAction()
	{
		m_state = CameraState.Disable;
	}

	void Awake()
	{
		initPos.z = 0;

		AllCameraSetOrthoSize( normalOtherSize );
	}

	void Start()
	{
		LogicManager.LevelManager.GetLevelObject().transform.position = - initPos;
		RecordLevelPosition();
		SetupFrameSize();

	}

	void SetupFrameSize ()
	{
		Vector3 accessTopRight = Camera.main.ScreenToWorldPoint( Vector3.zero );
		Vector3 accessBotLeft = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width , Screen.height ));
		Vector3 accesableSize = accessBotLeft - accessTopRight;

		accesableSize.x += frame.x ;
		accesableSize.y += frame.y ;

		frameSize = accesableSize;

	}
	void RecordLevelPosition()
	{
		levelPosStartFollow = LogicManager.LevelManager.GetLevelObject().transform.position;
		OthrSizeStartFollow = GetCameraOrthoSize();
	}

	float StartFollowTime = Mathf.Infinity;

	void LateUpdate () {
		UpdateFollow();
	}


	void UpdateFollow()
	{
		if ( !m_enable ) return;
		if ( m_state == CameraState.FollowTrans )
		{
			// check the follow time, if follow too long then stop
			float timeFromStart = Time.time - StartFollowTime;
			if ( ( Time.time - StartFollowTime ) > MaxFollowTime )
			{
				CameraStopFollow();
				Debug.Log("Stop 1");
			}

			Transform lvlTrans = LogicManager.LevelManager.GetLevelObject().transform;

			// if the camera is out of range, then stop
			if ( !ifInFrame( - lvlTrans.position ,  focusTolerance ) )
			{
				CameraStopFollow();
				Debug.Log("Stop 2");
			}

			// if the follow game object is destoried, then stop
			if ( !isFollowAvailable() )
			{
				CameraStopFollow();
				Debug.Log("Stop 3");
			}

			float timeFromStop = Time.time - StopFollowTime;
			Vector3 toPos = lvlTrans.position;
			float toOrthoSize = GetCameraOrthoSize();

			float rate = 0.02f;
			float orthSizeRate = 0.005f;
			// if the follow not equal to null
			// then the camera should follow the object
			if ( isFollowAvailable() ){
				toPos = GetFollow();
				// update the orth size
				toOrthoSize = GetFollowMaxDistance() * 0.6f + focusOtherSize;

				if ( timeFromStart > 0 ) {
					rate = Mathf.Clamp01( timeFromStart / CameraFollowFadeTime );
					if ( rate >= 1f )
						StartFollowTime = Mathf.Infinity;
				}

			}
			// if the follow equal to null
			// then the camera should go back to its initial position;
			else{
				toPos = levelPosStartFollow;
				toOrthoSize = OthrSizeStartFollow;
				if ( timeFromStop > 0 )
				{
					orthSizeRate = rate = Mathf.Clamp01( timeFromStop / CameraFollowFadeTime );
				}
			}

			Vector3 pos = lvlTrans.position;
			pos = Vector3.Lerp( pos , toPos , rate );
			if ( isLockY )
				pos.y = lvlTrans.position.y;
			lvlTrans.position = pos;

			AllCameraSetOrthoSize( Mathf.Lerp( GetCameraOrthoSize() , toOrthoSize , orthSizeRate ) );

		}
	}

	Vector3 GetFollow()
	{
		int count = 0;
		Vector3 to = Vector3.one;

		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && p.state == PetalState.Keep )
			{
				to = Vector3.Lerp( - ( p.transform.position - LogicManager.Level.transform.position) , to , 1f * count / ( count + 1 ));
				count ++;
			}
		}

		if ( count > 0 )
			return to;
		
		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && p.state == PetalState.LandGrow )
			{
				to = Vector3.Lerp( - ( p.transform.position - LogicManager.Level.transform.position) , to , 1f * count / ( count + 1 ));
				count ++;
			}
		}

		if ( count > 0 )
			return to - Vector3.up * 1.5f;

		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && p.state == PetalState.Fly )
			{
				to = Vector3.Lerp( - ( p.transform.position - LogicManager.Level.transform.position) , to , 1f * count / ( count + 1 ));
				count ++;
			}
		}

		return to;
	}

	float GetFollowMaxDistance()
	{
		float left = Mathf.Infinity;
		float right = -Mathf.Infinity;
		float buttom = Mathf.Infinity;
		float top = -Mathf.Infinity;
		bool isReturn = false;

		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && p.state == PetalState.Keep )
			{
				if ( left > p.transform.localPosition.x ) left = p.transform.localPosition.x;
				if ( right < p.transform.localPosition.x ) right = p.transform.localPosition.x;
				if ( buttom > p.transform.localPosition.y ) buttom = p.transform.localPosition.y;
				if ( top > p.transform.localPosition.y ) top = p.transform.localPosition.y;
				isReturn = true;
			}
		}
		if ( isReturn )
			return Mathf.Max( right - left  , top - buttom );
		
		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && p.state == PetalState.LandGrow )
			{
				if ( left > p.transform.localPosition.x ) left = p.transform.localPosition.x;
				if ( right < p.transform.localPosition.x ) right = p.transform.localPosition.x;
				if ( buttom > p.transform.localPosition.y ) buttom = p.transform.localPosition.y;
				if ( top > p.transform.localPosition.y ) top = p.transform.localPosition.y;
				isReturn = true;
			}
		}

		if ( isReturn )
			return Mathf.Max( right - left  , top - buttom );

		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && p.state == PetalState.Fly )
			{
				if ( left > p.transform.localPosition.x ) left = p.transform.localPosition.x;
				if ( right < p.transform.localPosition.x ) right = p.transform.localPosition.x;
				if ( buttom > p.transform.localPosition.y ) buttom = p.transform.localPosition.y;
				if ( top > p.transform.localPosition.y ) top = p.transform.localPosition.y;
				isReturn = true;
			}
		}

		return Mathf.Max( right - left  , top - buttom );

	}

	bool isFollowAvailable()
	{
		foreach( Petal p in focusPetals )
		{
			if ( p.isActiveAndEnabled && ( p.state == PetalState.Fly || p.state == PetalState.LandGrow) )
				return true;
		}
		return false;
	}

	Vector3 GetNewPosition( Vector3 _pos , Vector3 delta )
	{
		Vector3 pos = _pos + delta;
		if ( pos.x < frameOffset.x - frame.x / 2f  && delta.x < 0 )
			pos.x = _pos.x ;
		if ( pos.x > frameOffset.x + frame.x / 2f  && delta.x > 0 )
			pos.x = _pos.x ;

		if ( pos.y < frameOffset.y - frame.y / 2f  && delta.y < 0 )
			pos.y = _pos.y ;
		if ( pos.y > frameOffset.y + frame.y / 2f  && delta.y > 0 )
			pos.y = _pos.y ;

		return pos;
	}

	bool isZoomOut = false;
	void ZoomSwitch()
	{
		
		if ( isZoomOut ) 
			ZoomIn();
		else 
			ZoomOut();
	}

	void ZoomOut()
	{
		if ( !m_enable ) return;
		if ( isZoomOut ) return;

		AllCameraDoOrthoSize( ZoomOutOtherSize , Global.zoomFadeTime , 0 , Ease.OutCubic );
		EventManager.Instance.PostEvent( EventDefine.ZoomOut );
		isZoomOut = true;
	}

	void ZoomIn()
	{
		if ( !m_enable ) return;
		if ( !isZoomOut ) return;

		AllCameraDoOrthoSize( normalOtherSize , Global.zoomFadeTime , 0 , Ease.OutCubic );
		EventManager.Instance.PostEvent( EventDefine.ZoomIn );

		isZoomOut = false;
	}

//	Vector3 ConstrainPosition( Vector3 _pos , Vector3 delta )
//	{
//		Vector3 pos = _pos;
//
//		pos.x = Mathf.Clamp( pos.x , frameOffset.x - frame.x / 2f ,  frameOffset.x + frame.x / 2f );
//		pos.y = Mathf.Clamp( pos.y , frameOffset.y - frame.y / 2f ,  frameOffset.y + frame.y / 2f );
//		pos.z = 0;
//		return pos;
//	}


/// <summary>
/// Check if the position in the frame of the camera
/// </summary>
/// <returns><c>true</c>, if in frame was ifed, <c>false</c> otherwise.</returns>
/// <param name="pos">Position.</param>
/// <param name="tolarent">Tolarent.</param>
	bool ifInFrame( Vector3 pos , float tolarent )
	{
		if ( pos.x < (frameOffset.x - frame.x / 2f - tolarent) || pos.x > (frameOffset.x + frame.x / 2f + tolarent ) ) {
			return false;
		}
		if ( pos.y < (frameOffset.y - frame.y / 2f - tolarent) || pos.y > (frameOffset.y + frame.y / 2f + tolarent ) ){
			return false;
		}
		return true;
	}

	public bool ifInFrameWorld( Vector3 pos )
	{
		float tolarent = FrameTolarance;
		if ( pos.x < frameOffset.x - frameSize.x / 2 - tolarent) return false;
		if ( pos.x > frameOffset.x + frameSize.x / 2 + tolarent) return false;
		if ( pos.y < frameOffset.y - frameSize.y / 2 - tolarent) return false;
		return true;
	}
		
/// <summary>
/// Creats the ink. Create the in by the position 
/// </summary>
/// <param name="posFinger">Position to create the ink.</param>
/// <param name="finger">Which Finger .</param>
	void CreatInk( Vector3 posFinger , FingerGestures.Finger finger )
	{
		if ( !m_enable ) return;
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


	void OnFingerUpBack( FingerUpEvent e )
	{
		if ( !m_enable ) return;
		if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
		{
			inkDict[e.Finger.Index].Fade();
			inkDict[e.Finger.Index] = null;
		}
	}

	void OnFingerHoverBack( FingerHoverEvent e )
	{
		if ( !m_enable ) return;
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

	void OnFingerStationaryBack( FingerMotionEvent e )
	{
		if ( !m_enable ) return;
		if ( e.Phase == FingerMotionPhase.Updated )
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

	void OnFingerDownBack( FingerDownEvent e )
	{
		if ( !m_enable ) return;
//		GameObject selection = e.Selection;
		if ( e.Finger.Phase == FingerGestures.FingerPhase.Begin )
		{
			if ( enabled && m_state == CameraState.Free )
				CreatInk( e.Position , e.Finger );
			else 
				EventManager.Instance.PostEvent( EventDefine.UnableToMove );
		}

	}
		
	void OnFingerMoveBack( FingerMotionEvent e )
	{
		if ( !m_enable ) return;
		if ( enabled && m_state == CameraState.Free )
		{
			// update the level position according to the finger movement
			bool check = false;

			switch( Application.platform )
			{
			case RuntimePlatform.Android:
			case RuntimePlatform.IPhonePlayer:
				check = FingerGestures.Touches.Count >= 2;
				break;
			default :
				check = e.Finger.Index >= 1;
				break;
			}

			if ( check )
			{
				if ( e.Phase == FingerMotionPhase.Updated )
				{
					GameObject levelObj = LogicManager.LevelManager.GetLevelObject();
					Vector3 pos = - levelObj.transform.position;
					// move the camera
					Vector3 worldDelta = - Global.V2ToV3( e.Finger.DeltaPosition ) * Global.Pixel2Unit * senseIntense;
					// pos -= Global.V2ToV3( e.Finger.DeltaPosition ) * Global.Pixel2Unit * senseIntense;
					// restrict the range
					pos = GetNewPosition( pos , worldDelta );
					levelObj.transform.position = - pos;
					if ( ifSendMoveMessage ){
						EventManager.Instance.PostEvent(EventDefine.MoveCamera );
						ifSendMoveMessage = false;
					}
				}else if ( e.Phase == FingerMotionPhase.Ended )
				{
					LogicManager.LevelManager.GetWind().StartUpdateWind();
				}
			}
		}else
		{
			//show the unable to move feedback
			if ( e.Phase == FingerMotionPhase.Started )
				EventManager.Instance.PostEvent( EventDefine.UnableToMove );
		}
	}

	void OnPinchBack(  PinchGesture guesture )
	{
		if ( !m_enable ) return;

		if ( guesture.Phase == ContinuousGesturePhase.Updated )
		{
			float orthSize = GetCameraOrthoSize();
			orthSize += guesture.Delta * PinchSense;
			orthSize = Mathf.Clamp( orthSize , focusOtherSize , ZoomOutOtherSize );
			AllCameraSetOrthoSize( orthSize );
			if (  isZoomOutTutorial ) 
			{
				if ( guesture.Delta < 0 && GetCameraOrthoSize() > 6f )
				{
					EventManager.Instance.PostEvent( EventDefine.ZoomOut  );
					isZoomOutTutorial = false;
				}
			}
		}
	}
		

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Vector3 accessTopRight = Camera.main.ScreenToWorldPoint( Vector3.zero );
		Vector3 accessBotLeft = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width , Screen.height ));
		Vector3 accesableSize = accessBotLeft - accessTopRight;
		accesableSize.x += frame.x ;
		accesableSize.y += frame.y ;

//		Gizmos.DrawWireCube( transform.position + frameOffset , accesableSize );

		Gizmos.DrawWireCube( frameOffset , accesableSize );

		Gizmos.color = new Color( 1f ,  0.5f , 0.5f );

		Vector3 initFrame = initPos;
		initFrame.z = Global.CAMERA_INIT_POSITION.z;
		Gizmos.DrawWireCube( initFrame , accessBotLeft - accessTopRight );

	}
}
