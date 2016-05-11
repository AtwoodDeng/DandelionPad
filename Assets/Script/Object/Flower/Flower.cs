using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Flower : MonoBehaviour {

	[SerializeField] protected List<GameObject> petalPrefabList = new List<GameObject>();
	[SerializeField] protected MaxMinInt petalNumRange;
	[SerializeField] protected Transform petalRoot;

	protected List<Petal> petals = new List<Petal>();

	[SerializeField] protected bool useChance = true;
	[SerializeField] protected float blowChance = 1f;
	[SerializeField] protected float flyAwayChance = 0.77f;
	[SerializeField] protected int blowNumber = 0;
	[SerializeField] protected Transform stemModel;

	[SerializeField] GameObject petalModelPrefab;
	[SerializeField] float growPetalDelay = 0;


	public int petalNum=-1;
	public Transform root;

	[SerializeField] public GrowParameter growParameter;

	[System.SerializableAttribute]
	public struct GrowParameter
	{
		public float growTime;
		public MaxMin growPeterInterval;
		public float growHeight;
		public Vector3 normal;
		public PetalType petalType;
	}

	bool isInit = false;

	void Start()
	{
		Init();
	}

	virtual public void Init( )
	{
		if ( isInit )
			return ;

		isInit = true;


		if (petalNum <= 0 )
			petalNum = Random.Range(petalNumRange.min,petalNumRange.max);

		Grow();
	}

	// record the last petal initilization time
	protected float initPetalTime;

	virtual public void CreatePatel()
	{
		GameObject petalObj = Instantiate(petalPrefabList[Random.Range(0, petalPrefabList.Count)]) as GameObject;
		petalObj.transform.parent = petalRoot;
		petalObj.transform.localPosition = Vector3.zero;
		petalObj.transform.localScale = Vector3.one ;

		Petal petal = petalObj.GetComponent<Petal>();
		petal.Init(this,petals.Count);
		petals.Add(petal);

		initPetalTime = Time.time;
	}


	virtual public void Grow()
	{
		// transform.localScale = Vector3.one * 0.001f;
		// float ScaleChange = Random.Range(0.33f, 0.8f);
		//grow by position
		stemModel.transform.DOLocalMoveY(growParameter.growHeight, growParameter.growTime).SetRelative(true);
		
		
		// grow by scale
		// stemModel.transform.localScale *= ScaleChange;
		// petalRoot.transform.localPosition *= ScaleChange;
		transform.DOScale(Vector3.one , growParameter.growTime).OnComplete(GrowPatel);
	}

	virtual public void GrowPatel()
	{
		Vector3 rootLS = petalRoot.transform.localScale;
		Vector3 rootGS = petalRoot.transform.lossyScale;
		petalRoot.transform.localScale = new Vector3( rootLS.x / rootGS.x , rootLS.y / rootGS.y , rootLS.z / rootGS.z);
		// petalRoot.transform.parent = this.transform;
		// petalRoot.transform.localScale = Vector3.one;	
		// Debug.Log("Root lossy " + petalRoot.transform.lossyScale.ToString() );


		StartCoroutine(GrowPetalCor());
	}

	protected IEnumerator GrowPetalCor()
	{
		yield return new WaitForSeconds( growPetalDelay );
		for(int i = 0 ; i < petalNum ; ++ i)
		{
			CreatePatel();
			yield return new WaitForSeconds(Random.Range(growParameter.growPeterInterval.min, growParameter.growPeterInterval.max));
		}
	}

	virtual protected bool canBlow()
	{
//		return LogicManager.Instance.RemainBlowTime > 0;
		return true;
	}

	public void Blow(Vector2 dir , float velocity)
	{
		Blow( dir.normalized * velocity );
	}

	virtual public void Blow(Vector2 velocity )
	{
		if ( canBlow() )
		{
			Message msg = new Message();
			msg.AddMessage("Flower" , this );
			msg.AddMessage("Velocity" , velocity );


			if ( useChance )
			{
				int blow = 0;
				foreach(Petal petal in petals)
				{
					if (petal.state == PetalState.Link && Random.Range(0, 1f) < blowChance)
					{
						if (Random.Range(0, 1f) < flyAwayChance) {
							petal.Blow( (velocity.normalized + 0.6f * Global.GetRandomDirection()) * velocity.magnitude , Petal.BlowType.FlyAway);
						}
						else {
							msg.AddMessage("petal" + blow.ToString() , petal );
							blow++;
							petal.Blow( ( velocity.normalized + Random.Range(0,0.4f) * Global.GetRandomDirection() ) * velocity.magnitude , Petal.BlowType.Normal);
						}
						petal.transform.parent = LogicManager.Level.transform;
					}
				}
			}else{
				int blow = 0 ;
				for ( int i = 0 ; blow < blowNumber && i < petals.Count ; ++ i )
				{
					if ( petals[i].state == PetalState.Link )
					{
						petals[i].Blow( (velocity.normalized + Random.Range(0,0.4f) * Global.GetRandomDirection()) * velocity.magnitude , Petal.BlowType.Normal );
						petals[i].transform.parent = LogicManager.Level.transform;
						msg.AddMessage("petal" + blow.ToString() , petals[i] );
						blow ++;
					}
				}
//				Debug.Log("Blow " + blow );

				int linkPetal = GetPetalNumByType(PetalState.Link);

				int flyAwayNumber = (int)(linkPetal * flyAwayChance) + 1 ;
				int flyAway = 0;

				for ( int i = blowNumber ; flyAway < flyAwayNumber && i < petals.Count ; ++ i )
				{
					if ( petals[i].state == PetalState.Link )
					{
						petals[i].Blow( (velocity.normalized + 0.6f * Global.GetRandomDirection()) * velocity.magnitude , Petal.BlowType.FlyAway );
						petals[i].transform.parent = LogicManager.Level.transform;
						flyAway ++;
					}
				}
			}

			EventManager.Instance.PostEvent( EventDefine.BlowFlower , msg );
		}

	}


	public int GetPetalNumByType( PetalState compare )
	{
		int count = 0; 
		foreach(Petal petal in petals)
		{
			if ( petal.state == compare )
				count ++ ;
		}
		return count;
	}

	public bool isAllDead()
	{
		return GetPetalNumByType( PetalState.Dead ) >= petalNum;
	}

}
