using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M2DLight : MonoBehaviour {

	[SerializeField] GameObject lightPrefab;
	[SerializeField] GameObject shadowPrefab;
	[SerializeField] Color lightColor;
	[SerializeField] Color shadowColor;

	public float direction{
		get {
			return ( transform.rotation.eulerAngles.z - 90f ) * Mathf.Deg2Rad;
		}
	}
	public Vector2 direction2D {
		get {
			return new Vector2( Mathf.Cos( direction ) , Mathf.Sin( direction ));
		}
	}

	[SerializeField] float width;
	[SerializeField] float length;
	[SerializeField] float density;

	float lightWidth;
	int lightNum;


	List< SpriteRenderer> lights = new List<SpriteRenderer>();

	void Start()
	{
//		Vector3 toward = new Vector3( Mathf.Cos( direction ) , Mathf.Sin( direction ));
//		Vector3 side = new Vector3( Mathf.Sin( direction ) , - Mathf.Cos( direction ));
		lightWidth = 1f / density;
		lightNum = (int) (width / lightWidth);


		InitLights();
		UpdateLights();
	}


	void InitLights()
	{
		if ( lightPrefab != null )
		{
			for ( int i = 0 ; i < lightNum ; ++ i )
			{
				GameObject lightObj = Instantiate( lightPrefab ) as GameObject;
				SpriteRenderer light = lightObj.GetComponent<SpriteRenderer>();

				if ( light != null )
				{
					light.color = lightColor;
					light.transform.SetParent(transform );
					light.transform.localRotation = Quaternion.Euler( 0 , 0 , 0 );
					light.transform.localPosition = Vector3.right * ( 1.0f * ( i - 0.5f * lightNum ) + 0.5f ) * lightWidth;
					light.transform.localScale = new Vector3( lightWidth / light.sprite.texture.width * 100f , length / light.sprite.texture.height * 100f );


					light.material = new Material( light.material.shader );
					light.material.SetColor("_Color" , lightColor );
					light.material.SetColor("_ShadowColor" , shadowColor );

					lights.Add(light);
				}
			}
		}
	}

	void LateUpdate()
	{
		UpdateLights();
	}

	void UpdateLights()
	{
		int mask = 0 ;
		mask |= 1 << LayerMask.NameToLayer("FlowerExtra");
		mask |= 1 << LayerMask.NameToLayer("Land");
		for( int i = 0 ; i< lightNum ; ++ i )
		{
			RaycastHit2D hit2D =  Physics2D.Raycast( lights[i].transform.position , direction2D , length , mask );
			if ( hit2D.collider != null )
			{

				lights[i].material.SetFloat( "_Rate" , hit2D.distance / length );
				
			}else
			{
				lights[i].material.SetFloat("_Rate" , 1f );
			}

		}




		for( int i = 0 ; i< lightNum ; ++ i )
		{
			RaycastHit hitInfo;
			if ( Physics.Raycast( lights[i].transform.position , direction2D , out hitInfo , length , mask ) )
			{
				if ( lights[i].material.GetFloat( "_Rate" ) > hitInfo.distance / length )
					lights[i].material.SetFloat( "_Rate" , hitInfo.distance / length );

			}else
			{
				if ( lights[i].material.GetFloat( "_Rate" ) > 1f )
					lights[i].material.SetFloat("_Rate" , 1f );
			}

		}
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;

		Vector3 toward = new Vector3( Mathf.Cos( direction ) , Mathf.Sin( direction ));
		Vector3 side = new Vector3( Mathf.Sin( direction ) , - Mathf.Cos( direction ));

		Vector3 myPos = transform.position;
		Gizmos.DrawLine( myPos + side * width / 2 , myPos + toward * length + side * width / 2 );
		Gizmos.DrawLine( myPos - side * width / 2 , myPos + toward * length - side * width / 2 );
	}

//	public void SensableEnter( LightSensable s )
//	{
////		ShadowParameter sp = new ShadowParameter();
////		sp.sensable = s;
////		sp.shadow = Instantiate( shadowPrefab ) as GameObject;
////		sp.shadow.transform.SetParent( transform );
////
////		lightSensables.Add( sp );
//	}
//
//	public void SensableExit( LightSensable s )
//	{
////		for( int i = 0 ; i < lightSensables.Count ; ++ i )
////		{
////			if ( lightSensables[i].sensable == s ) {
////				lightSensables.RemoveAt(i);
////				break;
////			}
////		}
//	}
}
