using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ToCaveLevelManager : LevelManager {

//	[SerializeField] Land testLand;
//	void OnEnable()
//	{
//		EventManager.Instance.RegistersEvent(EventDefine.BlowFlower, OnBlowFlower);
//	}
//
//	void OnDisable()
//	{
//		EventManager.Instance.UnregistersEvent(EventDefine.BlowFlower, OnBlowFlower);
//	}
//
//	bool isFinal = false;
//	void OnBlowFlower( Message msg )
//	{
//		Flower flower = (Flower)msg.GetMessage("Flower");
//
//		if ( flower == null )
//			return;
//
//		Land land = flower.transform.parent.GetComponent<Land>();
//
//		if ( land == null )
//			return;
//
//		if ( land == testLand )
//		{
//			if ( land.transform.position.x > 3f )
//			{
//				isFinal = true;
//				for( int i = 0 ; i < 3 ; ++ i )
//				{
//					if ( msg.ContainMessage( "petal" + i.ToString()) )
//					{
//						Petal p = (Petal)msg.GetMessage( "petal" + i.ToString());
//						if ( p != null )
//						{
////							Debug.Log("Kill Petal " + p.name + " " + p.transform.DOKill() );
//							p.AvoidDeath();
//						}
//					}
//				}
//			}
//		}
//	}
//
//	public override bool CheckLevelFinished ()
//	{
//		return isFinal;
//	}
//
//	public override bool CheckLevelFinishWithoutLand (Land land)
//	{
//		return false;
//	}
}
