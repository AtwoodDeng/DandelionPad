using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Grass : MonoBehaviour {

	[SerializeField] Transform[] parts;
	[SerializeField] MaxMin partscaleRangeY;
	[SerializeField] MaxMin partscaleRangeX;
	[SerializeField] float AngleDiff;
	[SerializeField] float growTime;
	[SerializeField] float normalAffect = 0.33f;


	public void Init( Vector3 normal )
	{
		float angel = Vector3.Angle( Vector3.up , normal );
		if ( Vector3.Dot( Vector3.left , normal ) > 0)
			angel *= -1f;
		transform.Rotate( new Vector3( 0 , 0 , - angel * normalAffect + Random.Range( -5f , 5f )));

		StartCoroutine( GrowAfterSetUp());
	}

	IEnumerator GrowAfterSetUp()
	{
		float partRotateAngle = Random.Range( -AngleDiff , AngleDiff );
		float totalGrowTime = 0 ;

		Vector3 ps0 = parts[0].localScale;
		ps0.x *= Random.Range( partscaleRangeX.min ,  partscaleRangeX.max );
		parts[0].localScale = ps0;
		for(int i = 0 ; i < parts.Length ; i++)
		{
			Transform s = parts[i];

			// set the scale(start from V3(1,0,1))
			Vector3 scale = s.localScale;
			float scaleY = 1f;
			if (i != 0 ) scaleY = Random.Range(partscaleRangeY.min, partscaleRangeY.max);
			scale.y *= 0.001f * scaleY;
			s.localScale = scale;

			// set the rotation
			// each of the stem would rotate a little bit
			s.Rotate(new Vector3(0,0, partRotateAngle));


			// set the grow animation
			float myGrowTime = growTime/parts.Length * scaleY / 0.7f / LogicManager.AnimTimeRate;
			if (i == parts.Length - 1 )
				s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(totalGrowTime).SetEase(Ease.Linear);
			else
				s.DOScaleY(scale.y * 1000f, myGrowTime ).SetDelay(totalGrowTime).SetEase(Ease.Linear);

			// update the grow time
			totalGrowTime += myGrowTime ;

		}

		yield break;
	}

}
