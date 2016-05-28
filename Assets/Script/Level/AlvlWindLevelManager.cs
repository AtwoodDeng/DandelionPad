using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlvlWindLevelManager : LevelManager {

	[SerializeField] InkIvyFlower[] inkIvyFlowers;
	[SerializeField] GameObject IvyRock;

	protected override void Init ()
	{
		base.Init ();

		if ( IvyRock != null )
		{
			inkIvyFlowers = IvyRock.GetComponentsInChildren<InkIvyFlower>();
		}
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent( EventDefine.BlowFlower , OnBlow);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent( EventDefine.BlowFlower , OnBlow );
	}

	List<Petal> focusPetals;
	int alreadyBlowTime = 0;
	public void OnBlow(Message msg )
	{
		alreadyBlowTime += 1;
		if ( alreadyBlowTime > 0 ) 
		{
			foreach( InkIvyFlower inkF in inkIvyFlowers )
			{
				inkF.collider.enabled = true;
			}
		}

	}

}
