using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	[SerializeField] protected GameObject level;
	[SerializeField] protected int blowTime = 999;
	[SerializeField] protected List<PointArea> pointAreas = new List<PointArea>();
	[SerializeField] protected List<Land> lands = new List<Land>();
	[SerializeField] protected WindAdv wind;
	[SerializeField] protected EndingCondition endCondition;

	virtual public GameObject GetLevelObject() {
		if ( level != null )
			return level;
		return GameObject.Find("level");
	}

	void Start() {
		Init();
	}

	virtual protected void Init()
	{
		if ( pointAreas == null || pointAreas.Count <= 0 )
		{
			GameObject[] areas = GameObject.FindGameObjectsWithTag("FinalPoint");
			foreach( GameObject area in areas )
			{
				if ( area.GetComponent<PointArea>() != null )
				{
					pointAreas.Add( area.GetComponent<PointArea>());
				}
			}
		}

		if ( lands == null || lands.Count <= 0 )
		{
			GameObject[] lObjs = GameObject.FindGameObjectsWithTag("Land");
			foreach( GameObject lObj in lObjs )
			{
				if ( lObj.GetComponent<Land>() != null )
				{
					lands.Add( lObj.GetComponent<Land>());
				}
			}
		}
	}

	virtual public bool CheckLevelFinished()
	{
		if ( endCondition == EndingCondition.PointArea )
		{
			if ( pointAreas == null ) Init();
			foreach( PointArea pa in pointAreas )
			{
				if ( ! pa.isFinished )
					return false;
			}
			return true;
		} else if ( endCondition == EndingCondition.Land )
		{
			if ( lands == null ) Init();
			foreach( Land l in lands )
			{
				if ( !l.IsCompleted() )
				{
					return false;
				}
			}
			return true;
		}

		return false;
	}

	public bool CheckLevelDead()
	{
		if ( lands == null ) Init();
		foreach( Land l in lands )
		{
			if ( !l.IsAllDie() )
			{
				return false;
			}
		}
		return true;
	}

	virtual public bool CheckLevelFinishWithoutLand( Land land )
	{
		if ( lands == null ) Init();
		foreach( Land l in lands )
		{
			if ( !l.IsCompleted() && l != land )
			{
				return false;
			}
		}
		return true;
	}

	public WindAdv GetWind()
	{
		if ( wind != null ) 
			return wind;
		
		GameObject windObj = GameObject.FindWithTag( "Wind");

		if ( windObj != null )
		{
			wind = windObj.GetComponent<WindAdv>();
		}

		return wind;
	}

	virtual public int GetBlowTime()
	{
		return blowTime;
	}

	virtual public int GetEvaluation()
	{
		return 3;
	}
}
