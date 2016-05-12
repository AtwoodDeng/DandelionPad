using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TreeCreator : MonoBehaviour {

	[SerializeField] GameObject[] treePrefabs;
	[SerializeField] GameObject[] dropPrefabs;
	[SerializeField] Vector3 Range;
	[SerializeField] MaxMin scaleRange;
//	[SerializeField] float dropScale = 1.2f;

	[SerializeField] float treeRate = 0.7f;

	void Awake()
	{
		if ( GetComponent<BoxCollider>() != null )
		{
			GetComponent<BoxCollider>().size = Range;
		}
	}

	void OnFingerDownSpecial( FingerDownEvent e )
	{
		if ( Random.Range(0,1f) < treeRate )
			CreateTree();
		else
			CreateDrop();
	}

	void CreateTree()
	{
		GameObject obj = Instantiate( treePrefabs[Random.Range(0 , treePrefabs.Length)] ) as GameObject;
		obj.transform.SetParent(transform,true);
		SpriteRenderer objSprite = obj.GetComponentInChildren<SpriteRenderer>();
		Vector3 size = Vector3.one * 2f;
		float toScale = Random.Range( scaleRange.max , scaleRange.max );
		obj.transform.localScale *= toScale;
		if ( objSprite != null )
		{
			size.x = objSprite.sprite.rect.width * toScale * Global.Pixel2Unit;
			size.y = objSprite.sprite.rect.height * toScale * Global.Pixel2Unit;
			Debug.Log("toscale " + toScale );
		}
		Vector3 localPosition = Vector3.zero;
		localPosition.x = Random.Range( - Range.x / 2f + size.x / 2f ,  Range.x / 2f - size.x / 2f);
		localPosition.y = Random.Range( - Range.y / 2f + size.y / 2f ,  Range.y / 2f - size.y / 2f);
		obj.transform.position = transform.position + localPosition;

		Debug.Log("LP " + localPosition + obj.transform.localPosition  );
		obj.transform.Rotate( Vector3.forward , Random.Range( 0 , 360f ));
	}

	void CreateDrop()
	{

//		GameObject obj = Instantiate( dropPrefabs[Random.Range(0 , dropPrefabs.Length)] ) as GameObject;
//		obj.transform.SetParent(transform,true);
//		SpriteRenderer objSprite = obj.GetComponentInChildren<SpriteRenderer>();
//		Vector3 size = Vector3.one * 2f;
//		if ( objSprite != null )
//		{
//			size.x = objSprite.sprite.rect.width;
//			size.y = objSprite.sprite.rect.height;
//		}
//		obj.transform.localPosition = new Vector3( 
//			Random.Range( - Range.x / 2 + size.x / 2 ,  Range.x / 2 - size.x / 2) ,
//			Random.Range( - Range.y / 2 + size.y / 2 ,  Range.y / 2 - size.y / 2) ,
//			0);
//		obj.transform.localScale = Vector3.one * Random.Range( scaleRange.max , scaleRange.max );
//		obj.transform.Rotate( Vector3.forward , Random.Range( 0 , 360f ));
//		obj.transform.DOScale( dropScale , 2f ).SetEase(Ease.OutCubic);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position , Range );
	}
}
