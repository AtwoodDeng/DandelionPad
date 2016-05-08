using UnityEngine;
using System.Collections;

public class Level13 : LevelManager {

	[SerializeField] int useSwipeLimit;
	[SerializeField] float timeLimit;
	[SerializeField] int findSpecial;

	public override int GetEvaluation ()
	{
		int evaluation = 0 ;

		int usedSwipeTime = GetBlowTime() - LogicManager.Instance.RemainBlowTime;
		if ( usedSwipeTime < useSwipeLimit )
			evaluation ++;
		
		if ( Time.timeSinceLevelLoad < timeLimit )
			evaluation ++;

		evaluation++;

		return evaluation;
	}
}
