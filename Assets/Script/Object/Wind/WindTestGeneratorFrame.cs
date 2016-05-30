using UnityEngine;
using System.Collections;

public class WindTestGeneratorFrame : WindTestGenerator {

	[SerializeField] Vector2 Frame;
	[SerializeField] float GenerateInterval;

	public override void ResetWindTest (WindTest windTest)
	{
		Vector3 pos = new Vector3( Random.Range( Frame.x / 2f , - Frame.x / 2f ) 
			, Random.Range( Frame.y / 2f , - Frame.y / 2f ),
			0);
		
		windTest.Enter( pos );
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red ;

		Gizmos.DrawWireCube( transform.position , Frame );
	}
		
	public override void InitWindTestPos ()
	{
		StartCoroutine( WindTestEnterDo() );
	}

	IEnumerator WindTestEnterDo()
	{
		for ( int i = 0 ; i < windTestNum ; ++ i )
		{
			Debug.Log("Reset " +  i );
			ResetWindTest( WindTests[i] );

			yield return new WaitForSeconds( GenerateInterval );
		}
	}
}
