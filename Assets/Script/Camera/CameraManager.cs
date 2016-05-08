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

	[SerializeField] float focusTolerance = 5f;
	[SerializeField] bool ifSendMoveMessage = false;

	public Vector3 OffsetFromInit{
		get {
			Vector3 tem;
			if ( m_state != CameraState.Free )
				tem = levelPosStartFollow - initPos;
			else 
				tem = LogicManager.LevelManager.GetLevelObject().transform.position - initPos;
			tem.z = 0;
			return  tem;
		}
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower , OnBlow);
		EventManager.Instance.RegistersEvent(EventDefine.PetalDestory , OnPetalDestory);
		EventManager.Instance.RegistersEvent(EventDefine.BloomFlower , OnBloomFlower);
		EventManager.Instance.RegistersEvent(EventDefine.SwitchZoom , OnSwitchZoom);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel, OnEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFirstFlower, OnFirstFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower , OnBlow);
		EventManager.Instance.UnregistersEvent(EventDefine.PetalDestory , OnPetalDestory);
		EventManager.Instance.UnregistersEvent(EventDefine.BloomFlower , OnBloomFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchZoom , OnSwitchZoom);
	}

	List<Petal> blowPetals = new List<Petal>();
	void OnBlow(Message msg )
	{
		int i = 0 ;

		List<Petal> newPetal = new List<Petal>();

		while ( msg.ContainMessage( "petal" + i.ToString()))
		{
			Petal p = (Petal) msg.GetMessage("petal" + i.ToString());
			if ( p.state == PetalState.Fly )
				newPetal.Add( p );
			i++;
		}

		blowPetals.AddRange(newPetal);

		if ( newPetal.Count > 0 )
			CameraStartFollow( newPetal[ Random.Range( 0, newPetal.Count)].transform );
	}

	void OnBloomFlower( Message msg )
	{
		Petal p = msg.sender as Petal;

	
		CameraStopFollow();

		if ( p != null )
		{
			for ( int i = 0 ; i < blowPetals.Count ; ++ i )
			{
				if ( blowPetals[i].ID == p.ID )
				{
					blowPetals.Clear();
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
			for ( int i = 0 ; i < blowPetals.Count ; ++ i )
			{
				if ( blowPetals[i].ID == p.ID  )
				{
				// do this if the petal failed to grow the flower
					if ( msg.ContainMessage("FailToGrow" ))
					{
						blowPetals.Clear();
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

	[SerializeField] Transform follow;
	[SerializeField] Vector3 levelPosStartFollow;
	void CameraStartFollow(Transform trans )
	{
		if ( ( m_state == CameraState.Free || m_state == CameraState.Disable ) && StartFollowTime == Mathf.Infinity )
		{
			Debug.Log("Follow " + trans.name );
			RecordLevelPosition();
			StartFollowTime = Time.time;
			m_state = CameraState.FollowTrans;
			follow = trans;

//			Camera[] childCameras = GetComponentsInChildren<Camera>();
//			foreach( Camera c in childCameras )
//			{
//				c.DOOrthoSize( focusOtherSize , CameraFollowFadeTime ).SetEase(Ease.InCubic);
//			}

			AllCameraDoOrthoSize( focusOtherSize , CameraFollowFadeTime , 0 , Ease.InCubic );

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
		Debug.Log("Stop Follow Camera");
		if ( m_state == CameraState.FollowTrans && StopFollowTime == Mathf.Infinity )
		{
			Sequence seq = DOTween.Sequence();

//			Camera[] childCameras = GetComponentsInChildren<Camera>();
//			foreach( Camera c in childCameras )
//			{
//				seq.Join( c.DOOrthoSize( normalOtherSize , CameraFollowFadeTime ).SetEase(Ease.OutCubic));
//			}
			AllCameraDoOrthoSize( normalOtherSize , CameraFollowFadeTime , 0 , Ease.OutCubic );
			VignetteAndChromaticAberration effect = GetComponentInChildren<VignetteAndChromaticAberration>();
			if ( effect != null )
			{
				seq.Join( DOTween.To( () => effect.intensity , (x) => effect.intensity = x , 0.24f , CameraFollowFadeTime ).SetEase(Ease.InCubic));
			}
			seq.Join(LogicManager.LevelManager.GetLevelObject().transform.DOMove(levelPosStartFollow,CameraFollowFadeTime));
			seq.AppendInterval( 0.1f );
			seq.AppendCallback(CameraStopFollowComplete);

			follow = null;
			StopFollowTime = Time.time;
			StartFollowTime = Mathf.Infinity;
		}
	}

	void AllCameraDoOrthoSize( float to , float duration, float delay , Ease easeType )
	{
		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			c.DOOrthoSize( to , duration ).SetEase(easeType).SetDelay(delay);
		}
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
		m_state = CameraState.Free;
	}

	void OnEndLevel(Message msg )
	{
		m_state = CameraState.Disable;
		RecordLevelPosition();
		//fade this camera
		GetComponent<Camera>().DOOrthoSize( EndFadeSize , EndFadeTime ).SetDelay( EndFadeDelay );
//		Camera[] childCameras = GetComponentsInChildren<Camera>();
//		foreach( Camera c in childCameras )
//		{
//			c.DOOrthoSize( EndFadeSize , EndFadeTime ).SetDelay( EndFadeDelay );
//		}
		AllCameraDoOrthoSize( EndFadeSize , EndFadeTime , EndFadeDelay , Ease.Linear );
		LogicManager.LevelManager.GetLevelObject().transform.DOMove( Vector3.zero , EndFadeTime ).SetDelay(EndFadeDelay);
	}

	void Awake()
	{
		initPos.z = 0;

		Camera[] childCameras = GetComponentsInChildren<Camera>();
		foreach( Camera c in childCameras )
		{
			c.orthographicSize = normalOtherSize;
		}
	}

	void Start()
	{
		LogicManager.LevelManager.GetLevelObject().transform.position = - initPos;
		RecordLevelPosition();
	}

	void RecordLevelPosition()
	{
		levelPosStartFollow = LogicManager.LevelManager.GetLevelObject().transform.position;
	}

	Vector3 followLastPosition = Vector3.zero;
	float StartFollowTime = Mathf.Infinity;

	void LateUpdate () {
		UpdateFollow();
	}

	void UpdateFollow()
	{
		if ( m_state == CameraState.FollowTrans )
		{
			// check the follow time, if follow too long then stop
			float timeFromStart = Time.time - StartFollowTime;
			if ( ( Time.time - StartFollowTime ) > MaxFollowTime )
			{
				CameraStopFollow();
			}

			Transform lvlTrans = LogicManager.LevelManager.GetLevelObject().transform;

			// if the camera is out of range, then stop
			if ( !ifInFrame( - lvlTrans.position ,  focusTolerance ) )
			{
				CameraStopFollow();
			}

			// if the follow game object is destoried, then stop
			if ( follow != null && !follow.gameObject.activeSelf )
			{
				 CameraStopFollow();
			}

			float timeFromStop = Time.time - StopFollowTime;
			Vector3 toPos = lvlTrans.position;

			float rate = 0.1f;
			// if the follow not equal to null
			// then the camera should follow the object
			if ( follow != null && follow.gameObject.activeSelf ){
				toPos = - (follow.gameObject.transform.position - lvlTrans.position );
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
				if ( timeFromStop > 0 )
				{
					rate = Mathf.Clamp01( timeFromStop / CameraFollowFadeTime );
				}
			}

//			Debug.Log(follow.name + " to " + toPos );
			Vector3 pos = lvlTrans.position;
			
			pos = Vector3.Lerp( pos , toPos , rate );
			lvlTrans.position = pos;

		}
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
		if ( isZoomOut ) return;

		AllCameraDoOrthoSize( ZoomOutOtherSize , Global.zoomFadeTime , 0 , Ease.OutCubic );
		EventManager.Instance.PostEvent( EventDefine.ZoomOut );
		isZoomOut = true;


	}

	void ZoomIn()
	{
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


/// <summary>
/// Creats the ink. Create the in by the position 
/// </summary>
/// <param name="posFinger">Position to create the ink.</param>
/// <param name="finger">Which Finger .</param>
	void CreatInk( Vector3 posFinger , FingerGestures.Finger finger )
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

// Finger related 

	void OnFingerUpBack( FingerUpEvent e )
	{
		if ( inkDict.ContainsKey( e.Finger.Index ) && inkDict[e.Finger.Index] != null)
		{
			inkDict[e.Finger.Index].Fade();
			inkDict[e.Finger.Index] = null;
		}
	}

	void OnFingerHoverBack( FingerHoverEvent e )
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

	void OnFingerStationaryBack( FingerMotionEvent e )
	{
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
		GameObject selection = e.Selection;
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

	float pinchStartGap = 0;
	void OnPinchBack(  PinchGesture guesture )
	{
		if ( guesture.Phase == ContinuousGesturePhase.Started )
		{
			pinchStartGap = guesture.Gap;
		}
		if ( guesture.Phase == ContinuousGesturePhase.Ended )
		{
			if ( guesture.Gap > pinchStartGap )
			{
				ZoomOut();
			}
			else
			{
				ZoomIn();
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

		Gizmos.DrawWireCube( transform.position + frameOffset , accesableSize );

		Gizmos.color = new Color( 1f ,  0.5f , 0.5f );

		Vector3 initFrame = initPos;
		initFrame.z = Global.CAMERA_INIT_POSITION.z;
		Gizmos.DrawWireCube( initFrame , accessBotLeft - accessTopRight );

	}
}
