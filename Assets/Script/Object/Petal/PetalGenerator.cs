using UnityEngine;
using System.Collections;

public class PetalGenerator : MonoBehaviour {

	[SerializeField] GameObject petalPrefab;
	[SerializeField] EventDefine triggerEvent;
	[SerializeField] float generateTime;
	[SerializeField] float NumberPerSecond;
	[SerializeField] float radius;
	[SerializeField] MaxMin scale;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent( triggerEvent , OnEvent);

	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent( triggerEvent , OnEvent );
	}


	void OnEvent( Message msg )
	{
		StartCoroutine( BeginGeneratePetal());
	}

	IEnumerator BeginGeneratePetal()
	{
		float timer = 0;
		float count = 0;
		while( timer < generateTime )
		{
			timer += Time.deltaTime;
			count += Time.deltaTime * NumberPerSecond;
			for ( float i = 0 ; i < count ; i = i + 1f )
			{
				GameObject petal = Instantiate( petalPrefab ) as GameObject;
				petal.transform.SetParent( transform , true );
				petal.transform.localPosition = Global.GetRandomDirection() * Random.Range( 0 , radius );
				petal.transform.rotation = Random.rotation;
				petal.transform.localScale = Vector3.one * Global.GetRandomMinMax(scale);
				count = count - 1f;

			}
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere( transform.position , radius );
	}
}

