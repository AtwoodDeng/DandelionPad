using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Petal : MonoBehaviour  {

	protected Flower flower;
	
	[SerializeField] protected GameObject flowerPrefab;
	[SerializeField] float minGrowDistance = 0.3f;
//	[SerializeField] int growLimit;
	[SerializeField] public Transform top;
	[SerializeField] public float selfDestoryTime = 2f;

	public PetalState state = PetalState.Link;

	public int ID
	{
		get {
			return m_ID;
		}
	}
	int m_ID;
	static int m_ID_Pool=0;

	public PetalInfo myGrowInfo;

	public enum BlowType
	{
		Normal,
		FlyAway,
		Final,
	}

	void OnEnable()
	{
//		EventManager.Instance.RegistersEvent(EventDefine.GrowFlowerOn, OnOthersGrowFlower);
	}

	void OnDisable()
	{
//		EventManager.Instance.UnregistersEvent(EventDefine.GrowFlowerOn, OnOthersGrowFlower);
	}


//	List<PetalInfo> flowerGrowInfoList = new List<PetalInfo>();
//	void OnOthersGrowFlower(Message msg)
//	{	
//		PetalInfo info = (PetalInfo) msg.GetMessage("info");
//		flowerGrowInfoList.Add(info);
//	}

	void Awake()
	{
		m_ID = m_ID_Pool++;
	}

	virtual public void Init(Flower _flower, int index)
	{
		// link the parent flower
		flower = _flower;
		//initialize the init rotation
		Grow(index);
	}

	public virtual void Grow( int index )
	{
		float rotation = 0.5f;
		if ( flower != null )
			rotation = index / flower.petalNum;
		transform.localRotation = Quaternion.Euler(new Vector3(0,0, 2f * Mathf.PI * rotation));

	}

	public virtual float GetGrowTime()
	{
		return 0;
	}

	// grow a new flower on the position
	protected void GrowFlowerOn(Vector3 position, Vector3 normal , Land land  )
	{
		GameObject flowerObj = Instantiate(flowerPrefab) as GameObject;
		if (land != null )
			flowerObj.transform.SetParent(land.transform);
		flowerObj.transform.localScale = land.GetGrowScale();

		Flower flowerCom = flowerObj.GetComponent<Flower>();
		Vector3 growPos = position - flowerCom.root.localPosition;
		growPos.z = land.transform.position.z;
		flowerObj.transform.position = growPos ;

		flowerCom.growParameter.normal = normal;
		if ( LogicManager.LevelManager.CheckLevelFinishWithoutLand( land ) )
			myGrowInfo.type = PetalType.Final;
		flowerCom.growParameter.petalType = myGrowInfo.type;
		flowerCom.Init();

		SendGrowMessage(position,land.transform , flowerCom);
	}

	virtual protected void SendGrowMessage(Vector3 position,Transform parent , Flower flower)
	{
		Message growMsg = new Message();
		myGrowInfo.position = position;
		myGrowInfo.parent = parent;
		if ( state == PetalState.Init)
			myGrowInfo.type = PetalType.Init;
		growMsg.AddMessage("info", myGrowInfo);
		growMsg.AddMessage("flower" , flower );
		growMsg.AddMessage("land" , parent.gameObject );
		EventManager.Instance.PostEvent(EventDefine.GrowFlowerOn, growMsg,this);
	}


	// Called by Wind.cs
	// Called every frame when wind force the petal
	virtual public void AddForce(Vector2 force)
	{
	}


    void OnCollisionEnter2D(Collision2D coll)
	{
		Land land = coll.gameObject.GetComponent<Land>();
		if(land != null || coll.gameObject.tag == Global.LAND_TAG)
		{
			OnLand(coll);
		}
	}


	void OnCollisionEnter(Collision coll)
	{
		Land land = coll.gameObject.GetComponent<Land>();
		if(land != null || coll.gameObject.tag == Global.LAND_TAG)
		{
			OnLand(coll);
		}
	}


//	void OnTriggerEnter2D(Collider2D coll)
//	{
//		if (state != PetalState.Fly)
//			return;
//		Land land = coll.gameObject.GetComponent<Land>();
//		if(land != null || coll.gameObject.tag == Global.LAND_TAG)
//		{
//			OnLand(coll);
//		}
//	}

	public void OnLand(Collision2D coll)
	{
		OnLand( Global.V2ToV3( coll.contacts[0].point )
			, Global.V2ToV3( coll.contacts[0].normal ) 
			, coll.collider.gameObject );
	}

	public void OnLand(Collision coll)
	{
		OnLand( coll.contacts[0].point , coll.contacts[0].normal , coll.collider.gameObject );
	}

	protected Message destoryMessage = new Message();

	virtual public void OnLand(Vector3 point , Vector3 normal , GameObject obj)
	{

		//check the land
		Land land = obj.GetComponent<Land>();

		if ( land == null )
			return;

		transform.SetParent( land.transform , true );

		if ( state == PetalState.Fly || state == PetalState.Init )
		{
			// set the petal stable
			if ( GetComponent<Rigidbody>() != null ) {
				GetComponent<Rigidbody>().isKinematic = true;
			}

			//Change the State of the petal
			if ( state == PetalState.Init )
				EventManager.Instance.PostEvent( EventDefine.GrowFirstFlower );
//			state = PetalState.Land;


			//Grow a new flower on the collision point
			Vector3 contactPoint = new Vector3( point.x , point.y , 0 );
			Vector3 _normal = new Vector3( normal.x , normal.y , 0 );
			Vector3 growPoint = contactPoint - _normal.normalized * 0.1f;

			//cast a ray to find if actual hit point
			int layerMask = 1 << LayerMask.NameToLayer("Land");
			RaycastHit hitInfo ;

			if ( Physics.Raycast( point , Vector3.down , out hitInfo, 2f , layerMask ) )
			{
				growPoint = Global.V2ToV3( hitInfo.point ) + Vector3.down * hitInfo.distance * 2;
				_normal = hitInfo.normal;
			}

			destoryMessage.AddMessage("OnLand" , 1);

			if (checkCanGrowFlower(growPoint , _normal , land)) 
			{
				GrowFlowerOn(growPoint, _normal , land );
				state = PetalState.LandGrow;

			} else
			{
				destoryMessage.AddMessage("FailToGrow" , 1);
				state = PetalState.LandDead;
			}

			transform.DOScale( 0 , selfDestoryTime ).OnComplete(SelfDestory);
			transform.DOLocalMove( - 0.1f * _normal , selfDestoryTime ).SetRelative(true);

			SetColliderTrigger(true);

		}else if ( state == PetalState.FlyAway)
		{
			transform.DOScale( 0 , selfDestoryTime ).OnComplete(SelfDestory);
			SetColliderTrigger(true);
		}
	}

	public void SetColliderTrigger(bool to )
	{
		//		Collider2D[] colliders = GetComponents<Collider2D>();
		//		foreach(Collider2D c in colliders)
		//		{
		//			c.isTrigger = to;
		//		}
		//
		//		colliders = GetComponentsInChildren<Collider2D>();
		//		foreach(Collider2D c in colliders)
		//		{
		//			c.isTrigger = to;
		//		}

		Collider[] colliders = GetComponents<Collider>();
		foreach(Collider c in colliders)
		{
			c.isTrigger = to;
		}

		colliders = GetComponentsInChildren<Collider>();
		foreach(Collider c in colliders)
		{
			c.isTrigger = to;
		}
	}

	virtual protected bool checkCanGrowFlower(Vector3 position , Vector3 normal , Land land = null )
	{
		if ( land != null && !land.IfCanGrowFlower() )
			return false;
		Debug.Log("Check 1");

		Debug.DrawRay( position , normal );
		if ( Vector3.Dot( Vector3.up , normal.normalized ) < 0.3f )
			return false;

		Debug.Log("Check 2");

		foreach( Flower f in land.flowers )
		{
			Vector3 deltaPos = f.transform.position - position;
			deltaPos.z = 0;
			if ( deltaPos.magnitude < minGrowDistance )
				return false;
		}
		Debug.Log("Check 3");

//		if (flowerGrowInfoList.Count >= growLimit)
//			return false;
//		
//		Debug.Log("Check 3");
//			
//		foreach( PetalInfo info in flowerGrowInfoList)
//		{
//			if ( (info.position - position).magnitude < minGrowDistance )
//			{
//				return false;
//			}
//		}
//
//		Debug.Log("Check 4");

		return true;
	}

//	virtual public void OnLand(Collider2D coll)
//	{
//		if ( state == PetalState.Fly )
//		{
//			
//			//Change the State of the petal
//			state = PetalState.Land;
//			//Grow a new flower on the collision point
//			Vector3 contactPoint = transform.position;
//
//
//			if (checkCanGrowFlower(contactPoint))
//				GrowFlowerOn(contactPoint,coll.transform);
//			//selfDestory
//			SelfDestory();
//		}
//	}

	virtual protected void SelfDestory()
	{
		if ( state != PetalState.Dead )
		{
			transform.position = new Vector3(999999f,999999f,999999f);
			gameObject.SetActive(false);


			destoryMessage.AddMessage("petal" , this );
			EventManager.Instance.PostEvent(EventDefine.PetalDestory , destoryMessage );
			// Destroy(this.gameObject);

			state = PetalState.Dead;
		}

	}

	// Called by Flower.cs 
	// Call when player blow the dendalion
	public void Blow(Vector2 move, float vel, BlowType blowType = BlowType.Normal)
	{
		Blow( move.normalized * vel , blowType );
	}
	virtual public void Blow(Vector2 vel, BlowType blowType = BlowType.Normal)
	{
		if (blowType == BlowType.Normal )
			state = PetalState.Fly;
		else if (blowType == BlowType.FlyAway)
			state = PetalState.FlyAway;
		else if ( blowType == BlowType.Final )
		{
			Debug.Log("Blow Final");
			state = PetalState.FinalBlow;
		}
	}

	PetalState beforeKeep;
	virtual public void Keep()
	{
		beforeKeep = state;
		state = PetalState.Keep;

	}

	virtual public void Release()
	{
		state = beforeKeep;
	}
}
