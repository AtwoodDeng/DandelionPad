﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class WindAdv : MonoBehaviour {

	[SerializeField] Vector3 Size;
	[SerializeField] float Densiity = 1.0f;
	[SerializeField] int ThreadNumber = 1;
	[SerializeField] float Vorticity = 1f;
	[SerializeField] int IterationTime = 50;
	int m_iterationTime = 0;

	// *************  UI **************
	[SerializeField] SpriteRenderer windBackUI;
	[SerializeField] AnimationCurve ArrowScaleCurve;
	[SerializeField] GameObject windArrowPrefab;
	[SerializeField] Sprite[] windArrowSprites;
	[SerializeField] float UIDensity = 0.5f;
	ArrowEntry[] UIarrows;
	[SerializeField] float ArrowShowThreshod = 0.5f;
//	WindTest[] WindTests;
	  
	struct ArrowEntry
	{
		public SpriteRenderer sprite;
		public bool isIncluded;
		public float value;
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.SwitchWind,OnSwitchWind);
		EventManager.Instance.RegistersEvent(EventDefine.LevelInitialized,OnLevelInit);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.SwitchWind,OnSwitchWind);
		EventManager.Instance.UnregistersEvent(EventDefine.LevelInitialized,OnLevelInit);
	}

	void OnLevelInit( Message msg )
	{
		StartUpdateWind();
	}

	void Update()
	{
		WindSensable tem = null;
		for(int i = WindSensableList.Count - 1 ; i >= 0 ; i -- )
		{
			if ( WindSensableList[i] == null )
				WindSensableList.RemoveAt(i);
			else {
				WindSensablParameter par = WindSensableList[i].getParameter();

				if ( !par.shouldStore)
				{
					tem = WindSensableList[i];
				}else
				{
					if ( par.shouldUpdate ) {
						WindSensableList[i].SenseWind(GetVelocity(WindSensableList[i].getPosition()));
					}
				}
			}

		}

		// remove one per frame
		if ( tem != null)
		{
			WindSensableList.Remove(tem);
			tem.ExitWind();
		}
	}

//////////// wind field//////////////////////////
	public struct PairInt
	{
		public int x;
		public int y;
		public PairInt(int _x , int _y ) { x = _x ; y = _y ;}
	}

	public Vector2 CellSize
	{
		get {
			return new Vector2( 1.0f / Densiity , 1.0f / Densiity ) ;
		}
	}
	public Vector3 Center
	{
		get {
			return transform.position;
		}
	}

	int width;
	int height;
	Vector2[] m_force_velocity;
	Vector2[] m_velocity; 
	float[] m_divergence;
	float[] m_vorticity;

	float[] m_pressure;
	int[] m_obstacle; // 2 for detected 2d land , 1 for detected 3d land , 0 for detected nothing

	static public int OBSTACLE_2D_LAND = 2;
	static public int OBSTACLE_3D_LAND = 1;
	static public int OBSTACLE_EMPTY = 0;

	void Start()
	{
		Init();
		InitBuffers();
		InitUI();
		StartUpdateWind();
	}

	Coroutine m_updateWindCor;
	public void StartUpdateWind()
	{
		ForAllInt(m_obstacle,UpdateObstacle);
		ForAllVec2(m_velocity, InitVelocity);
		ForAllVec2(m_force_velocity, InitForceVelocity);
		if ( m_updateWindCor == null)
			m_updateWindCor = StartCoroutine(UpdateWind(IterationTime));
		else
			m_iterationTime = IterationTime;
	}


	void Init()
	{
		width = (int)( Size.x / CellSize.x );
		height = (int)( Size.y / CellSize.y );

		GetComponent<BoxCollider>().size = Size;

		gameObject.tag = "Wind";
	}

	void InitBuffers()
	{
		m_obstacle = new int[width * height];
		m_velocity = new Vector2[width * height];
		m_divergence = new float[width * height];
		m_pressure = new float[width * height];
		m_vorticity = new float[width * height];
		m_force_velocity = new Vector2[width * height];
	}

//////////////////////// deal array //////////////////////////

	void ForAllVec2(Vector2[] arr , DealVec2 func , int threadID = -1 )
	{
		int begI = (threadID < 0 )? 0 : threadID * width / ThreadNumber;
		int endI = (threadID < 0 )? width : ( threadID + 1 ) * width / ThreadNumber;

		for( int i = begI ; i < endI ; ++ i )
		for( int j = 0 ; j < height ; ++ j )
		{
			int pos = ij2index( i , j );
			arr[pos] = func(arr[pos] , i , j );
		}
	}

	void ForAllFloat(float[] arr , DealFloat func , int threadID = -1)
	{
		int begI = (threadID < 0 )? 0 : threadID * width / ThreadNumber;
		int endI = (threadID < 0 )? width : ( threadID + 1 ) * width / ThreadNumber;

		for( int i = begI ; i < endI ; ++ i )
		for( int j = 0 ; j < height ; ++ j )
		{
			int pos = ij2index( i , j );
			arr[pos] = func(arr[pos] , i , j );

		}
	}

	void ForAllInt(int[] arr , DealInt func , int threadID = -1)
	{
		int begI = (threadID < 0 )? 0 : threadID * width / ThreadNumber;
		int endI = (threadID < 0 )? width : ( threadID + 1 ) * width / ThreadNumber;

		for( int i = begI ; i < endI ; ++ i )
		for( int j = 0 ; j < height ; ++ j )
		{
			int pos = ij2index( i , j );
			arr[pos] = func(arr[pos] , i , j );
		}
	}

	void SwitchFloatArr( ref float[] a , ref float[] b )
	{
		float[] t = a ;
		a = b;
		b = t;
	}

	void SwitchVect2Arr( ref Vector2[] a , ref Vector2[] b )
	{
		Vector2[] t = a ;
		a = b;
		b = t;
	}

	void SwitchIntArr( ref int[] a , ref int[] b )
	{
		int[] t = a ;
		a = b;
		b = t;
	}

	delegate Vector2 DealVec2( Vector2 val , int i , int j );
	delegate float DealFloat( float val , int i , int j );
	delegate int DealInt( int val , int i , int j );

	int[] getNeighbour(int i , int j )
	{
		int[] neighbours = new int[4];

		neighbours[0] = ij2index(Mathf.Clamp(i-1, 0, width-1)	, j);
		neighbours[1] = ij2index(Mathf.Clamp(i+1, 0, width-1)	, j);
		neighbours[2] = ij2index(i , Mathf.Clamp(j - 1 , 0, height-1 ) );
		neighbours[3] = ij2index(i , Mathf.Clamp(j + 1 , 0, height-1 ) );
		return neighbours;
	}

///////////// DealVec2 /////////////////
	Vector2 GetRandom( Vector2 val , int i , int j )
	{
		if ( m_obstacle[ij2index(i,j)] > 0 )
			return Vector2.zero;
		return new Vector2( Mathf.Cos(Random.Range(0, 999f) ) , Mathf.Sin(Random.Range(0, 999f) ) );
	}

	Vector2 InitVelocity( Vector2 val , int i , int j )
	{
		if ( m_obstacle[ ij2index(i, j) ] != OBSTACLE_EMPTY )
			return Vector2.zero;
		// set up test ray and mask
		Vector3 pos = ij2Pos(i,j);
		pos.z += -5.0f;
		Ray ray = new Ray();
		ray.direction = Vector3.forward;
		ray.origin = pos;
		float distance = 20f;
		int mask = LayerMask.GetMask("WindGenerator");

		RaycastHit[] hitInfo = Physics.RaycastAll(ray , distance , mask) ;// if there is land in forward 
		if ( hitInfo != null )
		{
			foreach ( RaycastHit hit in hitInfo)
			{
				WindGenerator generator = hit.collider.gameObject.GetComponent<WindGenerator>();
				if ( generator != null )
				{
					return generator.GetVelocity( ij2Pos(i , j ) );
				}
			}
		}


		RaycastHit2D[] hitInfo2d = Physics2D.GetRayIntersectionAll(ray, distance , mask );
		if ( hitInfo2d != null )
		{
			foreach ( RaycastHit2D hit in hitInfo2d)
			{
				WindGenerator generator = hit.collider.gameObject.GetComponent<WindGenerator>();
				if ( generator != null )
				{
					return generator.GetVelocity( ij2Pos(i , j ) );
				}
			}
		}

		return Vector2.zero;
	}

	Vector2 InitForceVelocity( Vector2 val , int i , int j )
	{
		if ( m_obstacle[ ij2index(i, j) ] != OBSTACLE_EMPTY )
			return Vector2.zero;
		// set up test ray and mask
		Vector3 pos = ij2Pos(i,j);
		pos.z += -5.0f;
		Ray ray = new Ray();
		ray.direction = Vector3.forward;
		ray.origin = pos;
		float distance = 20f;
		int mask = LayerMask.GetMask("WindGenerator");

		RaycastHit[] hitInfo = Physics.RaycastAll(ray , distance , mask) ;// if there is land in forward 
		if ( hitInfo != null )
		{
			foreach ( RaycastHit hit in hitInfo)
			{
				WindGenerator generator = hit.collider.gameObject.GetComponent<WindGenerator>();
				if ( generator != null && generator.isForce)
				{
					return generator.GetVelocity( ij2Pos(i , j ) );
				}
			}
		}

		RaycastHit2D[] hitInfo2d = Physics2D.GetRayIntersectionAll(ray, distance , mask );
		if ( hitInfo2d != null )
		{
			foreach ( RaycastHit2D hit in hitInfo2d)
			{
				WindGenerator generator = hit.collider.gameObject.GetComponent<WindGenerator>();
				if ( generator != null && generator.isForce)
				{
					return generator.GetVelocity( ij2Pos(i , j ) );
				}
			}
		}

		return Vector2.zero;
	}

	Vector2 DrawVec( Vector2 val , int i , int j )
	{
		Vector3 pointPos = ij2Pos(i,j);
		Gizmos.DrawLine(pointPos, pointPos + 0.1f * new Vector3(val.x,val.y,0) );
		return val;
	}

	Vector2 SubstractGradient(Vector2 val , int i , int j )
	{
		int[] n = getNeighbour(i, j);
		float x1 = m_pressure[n[0]];
		float x2 = m_pressure[n[1]];
		float y1 = m_pressure[n[2]];
		float y2 = m_pressure[n[3]];
		float p = m_pressure[ij2index(i,j)];

		if(m_obstacle[n[0]] != OBSTACLE_EMPTY ) x1 = p;
		if(m_obstacle[n[1]] != OBSTACLE_EMPTY ) x2 = p;
		if(m_obstacle[n[2]] != OBSTACLE_EMPTY ) y1 = p;
		if(m_obstacle[n[3]] != OBSTACLE_EMPTY ) y2 = p;


		Vector2 velocity = m_velocity[ij2index(i,j)];
		velocity.x -= 0.5f * (x2 - x1);
		velocity.y -= 0.5f * (y2 - y1);
		return velocity;
	}

	Vector2 ResetBoundary(Vector2 val , int i , int j )
	{
		if ( m_obstacle[ij2index(i,j)] == OBSTACLE_EMPTY )
		{
			if ( m_force_velocity[ij2index(i, j)] != Vector2.zero )
			{
				return m_force_velocity[ij2index(i,j)];
			}else
			{
				return val;
			}
		}

		return Vector2.zero;
	}

	Vector2 ApplyVorticity(Vector2 val , int i , int j )
	{
		int[] n = getNeighbour(i, j);

		float vL = m_vorticity[n[0]];
		float vR = m_vorticity[n[1]];
		float vB = m_vorticity[n[2]];
		float vT = m_vorticity[n[3]];
		float vC = m_vorticity[ij2index(i,j)];

		Vector2 force = 0.5f * new Vector2(Mathf.Abs(vT) - Mathf.Abs(vB), Mathf.Abs(vR) - Mathf.Abs(vL));

		float EPSILON = 2.4414e-4f;
		float magSqr = Mathf.Max( EPSILON, Vector2.Dot(force, force)); 
		force = force / Mathf.Sqrt(magSqr);

		force.x *= Vorticity * vC ; 
		force.y *= - Vorticity * vC ;

		return val + force;
	}

/////////// DealFloat ///////////////////
	float UpdateDivergence(float val , int i , int j )
	{
		int[] n = getNeighbour(i, j);

		float x1 = m_velocity[n[0]].x;
		float x2 = m_velocity[n[1]].x;
		float y1 = m_velocity[n[2]].y;
		float y2 = m_velocity[n[3]].y;

		if(m_obstacle[n[0]] != OBSTACLE_EMPTY ) x1 = 0;
		if(m_obstacle[n[1]] != OBSTACLE_EMPTY ) x2 = 0;
		if(m_obstacle[n[2]] != OBSTACLE_EMPTY ) y1 = 0;
		if(m_obstacle[n[3]] != OBSTACLE_EMPTY ) y2 = 0;

		return 0.5f * ((x2 - x1) + (y2 - y1));
	}

	float ResetFloat(float val , int i , int j )
	{
		return 0;
	}

	float Poisson(float val , int i , int j )
	{
		float rbeta = 0.25f;

		int[] n = getNeighbour(i,j);
  
		float p = m_pressure[ij2index(i, j)];

		float x1 = (  m_obstacle[n[0]] != OBSTACLE_EMPTY) ? p : m_pressure[n[0]];
		float x2 = (  m_obstacle[n[1]] != OBSTACLE_EMPTY) ? p : m_pressure[n[1]];
		float y1 = (  m_obstacle[n[2]] != OBSTACLE_EMPTY) ? p : m_pressure[n[2]];
		float y2 = (  m_obstacle[n[3]] != OBSTACLE_EMPTY) ? p : m_pressure[n[3]];

		float b = m_divergence[ij2index(i, j)];

		return (x1 + x2 + y1 + y2 - b) * rbeta;

	}


	float DrawFloat( float val , int i , int j )
	{
		Vector3 pointPos = ij2Pos(i,j);
		Gizmos.DrawLine(pointPos, pointPos + val * Vector3.up * 0.1f );
		return val;
	}

	float CalcVorticity(float val , int i , int j )
	{
		int[] n = getNeighbour(i, j);

		Vector2 vL = m_velocity[n[0]];
		Vector2 vR = m_velocity[n[1]];
		Vector2 vB = m_velocity[n[2]];
		Vector2 vT = m_velocity[n[3]];

		return 0.5f * ((vR.y - vL.y) - (vT.x - vB.x));
	}

/////////// DealInt ///////////////////
	int UpdateObstacle(int val , int i , int j )
	{
		// set up test ray and mask
		Vector3 pos = ij2Pos(i,j);
		pos.z += -5.0f;
		Ray ray = new Ray();
		ray.direction = Vector3.forward;
		ray.origin = pos;
		float distance = 20f;
		int mask = LayerMask.GetMask("Land");

		if ( Physics.Raycast(ray , distance , mask) ) // if there is land in forward 
			return OBSTACLE_3D_LAND;

		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, distance , mask );
		if ( hit.collider != null )
			return OBSTACLE_2D_LAND;

		return OBSTACLE_EMPTY ;
	}



////////////////////////////////////////

	/// <summary>
	/// i j to world position
	/// </summary>
	Vector3 ij2Pos( int i , int j )
	{
		Vector3 res = Center;
		res.x += ( i - width / 2 ) * CellSize.x ;
		res.y += ( j - height / 2 ) * CellSize.y ;
		return res;
	}

	/// <summary>
	/// world position to i j 
	/// </summary>
	/// <param name="pos">Position.</param>
	int Pos2ij( Vector3 pos )
	{
		int x = (int)((pos.x - Center.x ) / CellSize.x ) + width / 2;
		x = Mathf.Clamp(x, 0, width-1);
		int y = (int)((pos.y - Center.y ) / CellSize.y ) + height / 2 ;
		y = Mathf.Clamp(y, 0, height-1);
		return ij2index(x,y);
	}

	int ij2index(int i , int j )
	{
		return i * height + j ;
	}



///////////////////////
	IEnumerator UpdateWind( int time )
	{
		float[] tem_pressure = new float[width * height];
		Vector2[] tem_velocity = new Vector2[width * height];
		m_iterationTime += time;

		while( m_iterationTime > 0)
		{
			//divergence
			ForAllFloat(m_divergence, UpdateDivergence );

			//vorticity
			ForAllFloat(m_vorticity, CalcVorticity );

			//appy vorticity
			ForAllVec2(m_velocity, ApplyVorticity );


			// pressure
			ForAllFloat(m_pressure , ResetFloat );

			for ( int i = 0 ; i < 5 ; ++ i )
			{
				for ( int k = 0 ; k < 5 ; ++ k )
				{
					ForAllFloat(tem_pressure, Poisson  );
					SwitchFloatArr(ref tem_pressure, ref m_pressure);
				}
				yield return null;
			}

			//substract gradient
			ForAllVec2(tem_velocity, SubstractGradient );
			SwitchVect2Arr(ref tem_velocity, ref m_velocity);

			ForAllVec2(m_velocity, ResetBoundary);

			m_iterationTime --;

			yield return null;
		}

		m_updateWindCor = null;
	}

///////// Trigger /////////////

	List<WindSensable> WindSensableList = new List<WindSensable>();
	void OnTriggerEnter2D(Collider2D col)
	{
		WindSensable ws = col.gameObject.GetComponent<WindSensable>();
		if (ws != null )
		{
			WindSensableList.Add(ws);
		}
	}


	void OnTriggerExit2D(Collider2D col)
	{
		WindSensable ws = col.gameObject.GetComponent<WindSensable>();
		if (ws != null )
		{
			if ( WindSensableList.Remove(ws) )
				ws.ExitWind();
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}


	void OnTriggerEnter(Collider col)
	{
		WindSensable ws = col.gameObject.GetComponent<WindSensable>();
		if (ws != null )
		{
			WindSensableList.Add(ws);
		}
	}

	void OnTriggerExit(Collider col)
	{
		WindSensable ws = col.gameObject.GetComponent<WindSensable>();
		if (ws != null )
		{
			if ( WindSensableList.Remove(ws) )
				ws.ExitWind();
			// rigid.AddTorque(Random.Range(torqueIntense.min, torqueIntense.max));
		}
	}

	Vector2 GetVelocity(Vector3 pos )
	{
		int index = Pos2ij(pos);
		if ( m_obstacle[index] != OBSTACLE_EMPTY )
		{
			int[] n = getNeighbour( index / height , index % height );
			for ( int i = 0 ; i < n.Length ; ++ i )
			{
				if ( m_obstacle[n[i]] != OBSTACLE_EMPTY ) {
					int[] nn = getNeighbour( n[i] / height , n[i] % height );
					for ( int j = 0 ; j < nn.Length ; ++ j )
					{
						if ( m_obstacle[nn[j]] == OBSTACLE_EMPTY) {
							return m_velocity[nn[j]];
						}
					}
				}else {
					return m_velocity[n[i]];
				}
			}
		}
		return m_velocity[index];
	}

	/// <summary>
	/// Check if the position is in the obsticle
	/// </summary>
	/// <returns><c>true</c>, if is not in the obstcle, <c>false</c> otherwise.</returns>
	/// <param name="position">world position.</param>
	public bool checkIsObsticle(Vector3 position )
	{
		if ( m_obstacle == null )
			return true;
		return m_obstacle[Pos2ij(position)] == OBSTACLE_EMPTY;
	}

//////////////////////


// ************ UI *************

	public bool UIShowed = false;
	int UIWidth;
	int UIHeight;
	void InitUI()
	{
		float rate = Densiity / UIDensity;
		UIWidth = (int)( width * UIDensity );
		UIHeight =  (int)( height * UIDensity );
		UIarrows = new ArrowEntry[ UIWidth * UIHeight ];
		for( int i = 0 ; i < UIWidth; ++ i )
		{
			for( int j = 0 ; j < UIHeight ; ++ j )
			{
				GameObject arrowObj = Instantiate( windArrowPrefab ) as GameObject;
				arrowObj.transform.SetParent( transform , true );
				arrowObj.transform.position = ij2Pos( (int)(i * rate) , (int)(j * rate) );
				arrowObj.transform.localScale = Vector3.one;

				SpriteRenderer sprite = arrowObj.GetComponent<SpriteRenderer>();
				sprite.enabled = false;

				UIarrows[ i * UIHeight + j ].sprite = sprite;
			}
		}

		UIShowed = false;
	}

//	float lastSwitchTime;
//	public void UISwitch()
//	{
//		if ( Time.time - lastSwitchTime < 2f  )
//			return;
//		lastSwitchTime = Time.time;
//
//		if ( UIShowed )
//			HideUI();
//		else 
//			ShowUI();
//	}

//
	void OnSwitchWind( Message msg )
	{
		bool windActive = (bool)msg.GetMessage("WindActive");
		float time = (float)msg.GetMessage("time");

		if ( windActive ) 
			ShowUI( time );
		else
			HideUI( time );
	}

	public void ShowUI( float time )
	{

		windBackUI.enabled = true;
		UpdateArrow();

		UIShowed = true;

		EventManager.Instance.PostEvent(EventDefine.ShowWind);

		StartCoroutine( ShowUICor( time ) );
	}

	IEnumerator ShowUICor( float time )
	{

		float timer = 0 ; 
		while ( timer < time )
		{
			float process = timer / time;
			timer += Time.deltaTime;

			{
				Color col = windBackUI.color;
				col.a = 0.5f * process;
				windBackUI.color = col;
			}

			for( int i = 0 ; i < UIWidth ; ++ i )
				for ( int j = 0 ; j < UIHeight ; ++ j )
				{
					SpriteRenderer arrow = UIarrows[i * UIHeight + j].sprite;
					{
						Color col = arrow.color;
						float P_ij = 1f * ( i + j ) / ( UIWidth + UIHeight - 2 );
						col.a = Mathf.Clamp01( process * 2f - P_ij ); 
						arrow.color = col;
					}
				}
			yield return null;

		}

//		{
//			Color col = windBackUI.color;
//			col.a = 0.5f;
//			windBackUI.color = col;
//			windBackUI.enabled = false;
//		}

//		for( int i = 0 ; i < UIWidth ; ++ i )
//			for ( int j = 0 ; j < UIHeight ; ++ j )
//			{
//				SpriteRenderer arrow = UIarrows[i * UIHeight + j];
//				{
//					Color col = arrow.color;
//					col.a = 1f;
//					arrow.color = col;
//				}
//			}

//		UpdateArrow();
	}


	public void HideUI( float time )
	{
		EventManager.Instance.PostEvent(EventDefine.HideWind);

		StartCoroutine( HideUICor( time ));
	}

	IEnumerator HideUICor ( float time )
	{
		float timer = 0 ; 

		UIShowed = false;

		while ( timer < time )
		{
			float process = timer / time;
			timer += Time.deltaTime;

			{
				Color col = windBackUI.color;
				col.a = 0.5f * ( 1f - process);
				windBackUI.color = col;
			}

			for( int i = 0 ; i < UIWidth ; ++ i )
				for ( int j = 0 ; j < UIHeight ; ++ j )
				{
					SpriteRenderer arrow = UIarrows[i * UIHeight + j].sprite;
					{
						Color col = arrow.color;
						float P_ij = 1f * ( i + j ) / ( UIWidth + UIHeight - 2 );
						col.a = 1f - Mathf.Clamp01( process * 2f - P_ij ); 
						arrow.color = col;
					}
				}
			yield return null;

		}
		HideUIComplete();

		
//		{
//			Color col = windBackUI.color;
//			col.a = 0f;
//			windBackUI.color = col;
//			windBackUI.enabled = false;
//		}

//		for( int i = 0 ; i < UIWidth ; ++ i )
//			for ( int j = 0 ; j < UIHeight ; ++ j )
//			{
//				SpriteRenderer arrow = UIarrows[i * UIHeight + j].sprite;
//				arrow.enabled = false;
//				{
//					Color col = arrow.color;
//					col.a = 0f;
//					arrow.color = col;
//				}
//			}
	}

	public void HideUIComplete()
	{
		windBackUI.enabled = false;
		for( int i = 0 ; i < UIWidth ; ++ i )
			for ( int j = 0 ; j < UIHeight ; ++ j )
			{
				SpriteRenderer arrow = UIarrows[i * UIHeight + j].sprite;
				arrow.enabled = false;
			}
	}


	void UpdateArrow()
	{	
		float rate = Densiity / UIDensity;

		for( int i = 0 ; i < UIWidth ; ++ i )
			for ( int j = 0 ; j < UIHeight ; ++ j )
			{
				UIarrows[i * UIHeight + j].sprite.transform.position = ij2Pos( (int)(i * rate) , (int)(j * rate) );
				if ( checkIsObsticle( UIarrows[i * UIHeight + j].sprite.transform.position ))
				{
					UIarrows[i * UIHeight + j].isIncluded = true;
					UIarrows[i * UIHeight + j].value = 1f;
				}else
				{
					UIarrows[i * UIHeight + j].isIncluded = false;
					UIarrows[i * UIHeight + j].value = 0f;
				}
			}

		int k = 2;
		for( int i = 0 ; i < UIWidth ; ++ i )
			for ( int j = 0 ; j < UIHeight ; ++ j )
				if ( UIarrows[i * UIHeight + j].isIncluded )
					{
						if ( ! IfDifferentFromNeighbour( i , j , k , ArrowShowThreshod ))
						{
							MergeNeighbough( i , j ,  k );
						}
					}

		for( int i = 0 ; i < UIWidth ; ++ i )
			for ( int j = 0 ; j < UIHeight ; ++ j )
				if ( UIarrows[i * UIHeight + j].isIncluded )
				{

					SpriteRenderer arrow = UIarrows[i * UIHeight + j].sprite;
					Vector2 velocity = GetVelocity( arrow.transform.position );
					arrow.enabled = true;


//					float vel = ArrowScaleCurve.Evaluate( velocity.magnitude );
//					arrow.transform.localScale = new Vector3( 0.3f , vel , 1f ) 
//						* Mathf.Pow( UIarrows[i * UIHeight + j].value , 0.3f );
					arrow.transform.localScale = Vector3.one * 0.3f
						* Mathf.Pow( velocity.magnitude , 0.25f );
					float angel = Mathf.Atan2( velocity.y , velocity.x ) * Mathf.Rad2Deg;
					angel -= 90f ;
					arrow.transform.eulerAngles = new Vector3( 0 , 0 , angel );

					arrow.sprite = ( velocity.magnitude > 5f ) ? windArrowSprites[2] : (velocity.magnitude > 1f)? windArrowSprites[1] : windArrowSprites[0];
				}
				else
				{
					UIarrows[i * UIHeight + j].sprite.enabled = false;
				}
	
//				SpriteRenderer arrow = UIarrows[i * UIHeight + j].sprite;
//				arrow.transform.position = ij2Pos( (int)(i * rate) , (int)(j * rate) );
//					
//				Vector2 velocity = GetVelocity( arrow.transform.position );
//				Vector2 neighbouVel = GetNeighboughUIAverage( i , j , 1 );
//				if ( !checkIsObsticle( arrow.transform.position ) )
//				{
//					arrow.enabled = false;
//				}else
//				{
//					arrow.enabled = true;
//
//					float vel = ArrowScaleCurve.Evaluate( velocity.magnitude );
//					arrow.transform.localScale = new Vector3( 0.2f , vel , 1f );
//					float angel = Mathf.Atan2( velocity.y , velocity.x ) * Mathf.Rad2Deg;
//					angel -= 90f ;
//					arrow.transform.eulerAngles = new Vector3( 0 , 0 , angel );
//				}
//			}

	}

	void MergeNeighbough( int i , int j , int radius)
	{

		for( int ii = i - radius ; ii <= i + radius ; ++ ii )
		{
			for ( int jj = j - radius ; jj <= j + radius ; ++ jj )
			{
				if ( ii >= 0 && ii < UIWidth && jj >= 0 && jj < UIHeight )
				{
					if ( i != ii && j != jj && UIarrows[ ii * UIHeight + jj ].isIncluded)
					{
						UIarrows[ i * UIHeight + j ].value += UIarrows[ ii * UIHeight + jj ].value;
						UIarrows[ ii * UIHeight + jj ].isIncluded = false;
						UIarrows[ ii * UIHeight + jj ].value = 0;
					}
				}
			}
		}
	}

	bool IfDifferentFromNeighbour( int i , int j , int radius , float thredshod )
	{
		Vector3 velocity = GetVelocity( UIarrows[ i * UIHeight + j ].sprite.transform.position );

		int count = 0;
		int total = 0;
		for( int ii = i - radius ; ii <= i + radius ; ++ ii )
		{
			for ( int jj = j - radius ; jj <= j + radius ; ++ jj )
			{
				if ( ii >= 0 && ii < UIWidth && jj >= 0 && jj < UIHeight )
				{
					if ( i != ii && j != jj && UIarrows[ ii * UIHeight + jj ].isIncluded )
					{
						Vector3 neighbouVel = GetVelocity( UIarrows[ ii * UIHeight + jj ].sprite.transform.position ) ;
						if ( ( velocity - neighbouVel ).magnitude / velocity.magnitude < thredshod )
						{
							count ++;
						}
						total ++;
					}
				}
			}
		}
		if ( total == 0 ) 
			return true;
		return (1f * count / total) < 0.1f;
	}

	Vector2 GetNeighboughUIAverage( int i , int j , int radius )
	{
		int count = 0 ;
		Vector2 sum = Vector3.zero;
		for( int ii = i - radius ; ii <= i + radius ; ++ ii )
		{
			for ( int jj = j - radius ; jj <= j + radius ; ++ jj )
			{
				if ( ii >= 0 && ii < UIWidth && jj >= 0 && jj < UIHeight )
				{
					if ( i != ii && j != jj )
					{
						sum += GetVelocity( UIarrows[ ii * UIHeight + jj ].sprite.transform.position ) ;
						count ++;
					}
				}
			}
		}
		if ( count > 0 )
			return sum / count;
		
		return sum;
	}

//	void InitUI()
//	{
//		windBackUI.transform.localScale = new Vector3( Size.x * 100f / windBackUI.sprite.texture.width 
//			, Size.y * 100f / windBackUI.sprite.texture.height );
//		windBackUI.DOFade(0,0);
//
//
//		WindTests = new WindTest[ windTestNum ];
//		for ( int i = 0 ; i < windTestNum ; ++ i )
//		{
//			GameObject windTest = Instantiate( WindTestPrefab ) as GameObject;
//			windTest.transform.SetParent(transform );
//			windTest.SetActive(false);
//
//			WindTests[i] = windTest.GetComponent<WindTest>();
//			WindTests[i].wind = this;
//		}
//			
//
//		UIShowed = false;
//	}
//
//	public void InitWindTestPos()
//	{
//		for ( int i = 0 ; i < windTestNum ; ++ i )
//		{
//			Vector3 ranPos = new Vector3 ( Random.Range( - Size.x / 2 , Size.x / 2 ) , Random.Range( - Size.y / 2 , Size.y / 2 ) , 0 ) + transform.position;
//			while( m_obstacle[Pos2ij(ranPos)] != OBSTACLE_EMPTY  ) 
//			{
//				ranPos = new Vector3 ( Random.Range( - Size.x / 2 , Size.x / 2 ) , Random.Range( - Size.y / 2 , Size.y / 2 ) , 0 );
//			}
//			WindTests[i].Enter(ranPos);
//		}
//	}
//
//	float lastSwitchTime;
//	public void UISwitch()
//	{
//		if ( Time.time - lastSwitchTime < 2f  )
//			return;
//		lastSwitchTime = Time.time;
//
//		if ( UIShowed )
//			HideUI();
//		else 
//			ShowUI();
//	}
//	public void ShowUI()
//	{
//		
//		windBackUI.DOFade( 0.5f , 2f ).SetEase(Ease.OutExpo);
//		InitWindTestPos();
//
//		EventManager.Instance.PostEvent(EventDefine.ShowWind);
//
//		UIShowed = true;
//	}
//
//	public void HideUI()
//	{
//		windBackUI.DOFade( 0f , 2f ).SetEase(Ease.Linear);
//
//		for ( int i = 0 ; i < windTestNum ; ++ i )
//		{
//			
//			WindTests[i].Exit();
//		}
//
//		EventManager.Instance.PostEvent(EventDefine.HideWind);
//
//		UIShowed = false;
//	}



//////////////////////

	public Vector3 GetSize()
	{
		return Size;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(Center, Size );

		ForAllVec2(m_velocity, DrawVec);

		Gizmos.color = Color.red;
		ForAllFloat(m_divergence , DrawFloat);
	}
}
