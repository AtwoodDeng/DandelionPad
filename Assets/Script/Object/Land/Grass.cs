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
	[SerializeField] float perishTime = 5f;
	[SerializeField] MaxMin perishAngle;
	[SerializeField] MaxMin perishDelay;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.LevelDead, OnLevelDead);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.LevelDead, OnLevelDead);
	}

	void OnLevelDead(Message msg )
	{
		Sequence seq = DOTween.Sequence();
		seq.AppendCallback( PerishPre );
		seq.AppendInterval( Random.Range( perishDelay.min , perishDelay.max ));
		seq.AppendCallback( Perish );
	}

	public void Init( Vector3 normal , Color col )
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
			if ( s == null )
				continue;

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

	void PerishPre()
	{
		float angle = ( Random.Range( -1 , 1 ) < 0 ? -1f : 1f ) * Random.Range( perishAngle.min , perishAngle.max );
		for( int i = 1 ; i < parts.Length ; ++ i )
		{
			parts[i].DOKill();
			parts[i].DOLocalRotate( new Vector3( 0 , 0 , angle ) , perishTime ).SetRelative(true).SetDelay( Random.Range( 0,1f));;
		}
	}

	void Perish()
	{
		for( int i = 1 ; i < parts.Length ; ++ i )
		{
			parts[i].DOKill();
			parts[i].DOScale( 0 , perishTime ).SetDelay( 1f - i * 0.2f );
		}

		if ( GetComponent<FollowWind>() != null )
			GetComponent<FollowWind>().enabled = false;
	}
}
