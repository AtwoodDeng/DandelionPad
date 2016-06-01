using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class LogicManager : MonoBehaviour {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;


	static LevelManager m_levelManager;
	static public LevelManager LevelManager
	{
		get {
			if ( m_levelManager != null )
				return m_levelManager;

			if ( Instance != null )
				m_levelManager = Instance.GetComponent<LevelManager>();

			if ( m_levelManager == null && Level != null )
			{
				m_levelManager = Level.GetComponent<LevelManager>();
			}

			return m_levelManager;
		}
	}

	static public Vector3 WorldPos2LevelPos ( Vector3 worldPos )
	{
		return worldPos - Level.transform.position;
	}

	static public Vector3 LevelPos2WorldPos ( Vector3 LevelPos )
	{
		return LevelPos + Level.transform.position;
	}

	static CameraManager m_cameraManager;
	static public CameraManager CameraManager
	{
		get {
			if ( m_cameraManager != null )
				return m_cameraManager;

			if ( Instance != null )
				m_cameraManager = Instance.GetComponent<CameraManager>();

			if ( m_cameraManager == null && Camera.main != null )
			{
				m_cameraManager = Camera.main.GetComponent<CameraManager>();
			}

			return m_cameraManager;
		}
	}

	static GameObject m_level;
	static public GameObject Level
	{
		get {
			if ( m_level != null )
				return m_level;
			
			if ( m_levelManager != null )
				m_level = m_levelManager.GetLevelObject();

			if ( m_level == null )
				m_level = GameObject.Find("level");
				
			return m_level;
		}
	}

	static float m_animTimeRate = 1f;
	[SerializeField] float IAnimTimeRate = 1f;
	static public float AnimTimeRate
	{
		get {
			if ( Instance != null )
			{
				m_animTimeRate = Instance.IAnimTimeRate;
			}
			return m_animTimeRate;
		}
	}

	static float m_physTimeRate = 1f;
	[SerializeField] float IPhysTimeRate = 1f;
	static public float PhysTimeRate
	{
		get {
			if ( Instance != null )
			{
				m_physTimeRate = Instance.IPhysTimeRate;
			}
			return m_physTimeRate;
		}
	}

	static float m_UITimeRate = 1f;
	[SerializeField] float IUITimeRate = 1f;
	static public float UITimeRate
	{
		get {
			if ( Instance != null )
			{
				m_UITimeRate = Instance.IUITimeRate;
			}
			return m_UITimeRate;
		}
	}

	static bool m_isSetting = false;
	static public bool isSetting
	{
		get { 
			return m_isSetting;
		}
	}

	int m_swipeTime = 3;
	public int RemainBlowTime
	{
		get { return m_swipeTime; }
	}

	bool m_isBegin = false;
	public bool isBegin
	{
		get { 
			return m_isBegin;
		}
	}

	bool m_isEnded = false;
	public bool isEnded
	{
		get { 
			return m_isEnded;
		}
	}


	bool m_isDead = false;
	public bool isDead
	{
		get { 
			return m_isDead;
		}
	}

	public Color trailColor = Color.black;

	static public Color TrailColor
	{
		get {
			if ( Instance.trailColor == Color.black )
				return Global.DefaultTrailColor;
			
			return Instance.trailColor;
		}
	}


	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
		EventManager.Instance.RegistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower,BlowFlower);
		EventManager.Instance.RegistersEvent(EventDefine.GrowFinalFlower,GrowFinalFlower);
		EventManager.Instance.RegistersEvent(EventDefine.AllBlackEndLevel,AllBlackEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.AllBlackRetry,AllBlackRetry);
		EventManager.Instance.RegistersEvent(EventDefine.BloomFlower,OnBloomFlower);
		EventManager.Instance.RegistersEvent(EventDefine.GrowFirstFlower,OnGrowFirstFlower);
		EventManager.Instance.RegistersEvent(EventDefine.EndLevel,OnEndLevel);
		EventManager.Instance.RegistersEvent(EventDefine.SwitchSetting,OnSwitchSetting);
		EventManager.Instance.RegistersEvent(EventDefine.SwitchWind,OnSwitchWind);

	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.RenewSwipeTime,RenewSwipeTime);
		EventManager.Instance.UnregistersEvent(EventDefine.AddSwipeTime,AddSwipeTime);
		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower,BlowFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFinalFlower,GrowFinalFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.AllBlackEndLevel,AllBlackEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.AllBlackRetry,AllBlackRetry);
		EventManager.Instance.UnregistersEvent(EventDefine.BloomFlower,OnBloomFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.GrowFirstFlower,OnGrowFirstFlower);
		EventManager.Instance.UnregistersEvent(EventDefine.EndLevel,OnEndLevel);
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchSetting,OnSwitchSetting);
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchWind,OnSwitchWind);


	}

	void Awake()
	{
		GameObject bgm = GameObject.FindGameObjectWithTag("BGM");
		if ( bgm == null )
		{
			bgm = Instantiate( Resources.Load(Global.BGM_PATH) ) as GameObject;

		}
		if ( bgm != null )
		{
			DontDestroyOnLoad( bgm );
		}
		if ( GameObject.FindGameObjectWithTag("Transformation") == null )
		{
			GameObject Trans = Instantiate( Resources.Load(Global.TRANS_PATH) ) as GameObject;
			Trans.transform.position = Vector3.zero;
		}

	}

	void Start()
	{
		if ( LevelManager != null )
			m_swipeTime = LevelManager.GetBlowTime();
		
		EventManager.Instance.PostEvent( EventDefine.BeginLevel );

	}

	/// <summary>
	/// Switch the wind UI mode of the wind
	/// </summary>
	/// <param name="msg"> </param>
	void OnSwitchWind( Message msg )
	{
		m_isSetting = (bool)msg.GetMessage("WindActive");

		IPhysTimeRate = isSetting? 0 : 1;

		if ( isSetting ) 
			DOTween.PauseAll();
		else
			DOTween.PlayAll();
	}

	void OnSwitchSetting( Message msg )
	{
		m_isSetting = (bool)msg.GetMessage("isSetting");
		IPhysTimeRate = isSetting? 0 : 1;

		if ( isSetting ) 
			DOTween.PauseAll();
		else
			DOTween.PlayAll();

	}

	void OnEndLevel( Message msg )
	{
		m_isEnded = true;
	}

	void OnGrowFirstFlower( Message msg )
	{
		m_isBegin = true;
	}

	void OnBloomFlower( Message msg)
	{
		if ( m_levelManager.CheckLevelFinished() )
		{
			m_isEnded = true;
		}
	}


	void AllBlackEndLevel( Message msg )
	{
		DOTween.KillAll();
		AsyncOperation AO = SceneManager.LoadSceneAsync( Global.NextLevel());
		StartCoroutine( LoadNextLevel( AO ));
	}

	IEnumerator LoadNextLevel( AsyncOperation AO )
	{
		AO.allowSceneActivation = false;

		float timer = 5f; 
		while( ! AO.isDone && timer > 0 )
		{	
			timer -= Time.deltaTime;
			Debug.Log("AO " + AO.progress );
			yield return null;
		}

		AO.allowSceneActivation = true;
	}

	void AllBlackRetry( Message msg )
	{
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	}

	void Update()
	{
		if ( m_isEnded && !checkEnd )
		{
			EventManager.Instance.PostEvent( EventDefine.EndLevel );
			checkEnd = true;
//			Debug.Log("End level");
		}

		if ( m_levelManager.CheckLevelDead() && !m_isDead && m_isBegin && !m_isEnded )
		{
			m_isDead = true;
			EventManager.Instance.PostEvent(EventDefine.LevelDead);
		}

		if ( Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftControl))
		{
			EventManager.Instance.PostEvent(EventDefine.EndLevel);
		}

		if ( Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
		{
			EventManager.Instance.PostEvent(EventDefine.LevelDead);
		}
	}

	bool checkEnd = false;
	void GrowFinalFlower( Message msg )
	{
		if ( m_levelManager.CheckLevelFinished() )
		{
			m_isEnded = true;
		}
	}

	void BlowFlower( Message msg )
	{
		m_swipeTime --; 
	}
		
	void RenewSwipeTime(Message msg)
	{
		int time = (int)msg.GetMessage("time");
		m_swipeTime = time;
	}

	void AddSwipeTime(Message msg)
	{
		m_swipeTime ++;
	}
		
}
