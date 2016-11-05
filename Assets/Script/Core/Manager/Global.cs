using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Global {

	static public Vector2 V3ToV2(Vector3 v)
	{
		return new Vector2(v.x,v.y);
	}
	static public Vector3 V2ToV3(Vector2 v)
	{
		return new Vector3(v.x,v.y,0);
	}
	static public Vector2 GetRandomDirection()
	{
		float angle = Random.Range(0, Mathf.PI * 2 );
		return new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
	}

	static public Vector3 GetRandomDirectionV3()
	{
		float theta = Random.Range(0, Mathf.PI * 2f );
		float beta = Random.Range(0, Mathf.PI * 2f );
		return new Vector3(Mathf.Sin(theta) * Mathf.Cos(beta)
			,Mathf.Sin(theta) * Mathf.Sin(beta)
			,Mathf.Cos(theta)).normalized;
	}

	static public string LAND_TAG = "Land";

	static public float StandardizeAngle(float angle)
	{
		float res = angle;
		while(res > 180f) res -= 360f;
		while(res <= -180f) res += 360f;
		return res;
	}


	static public int ROCK_ORDER 				= 50;  
	static public int SHADOW_ORDER 				= 40;  
	static public int FLOWER_STEM_ORDER 		= 10;
	static public int FLOWER_LEAF_ORDER 		= 8;
	static public int LIGHT_ORDER 				= 0;
	static public int WIND_ARROW_ORDER 			= -25;
	static public int WIND_BACK_ORDER 			= -30;

	static public float WIND_UI_Z = 0f;

	static public string[] levelNames =
	{
		"begin",
		"alvlStrike",
		"alvlResize",
		"alvlMoveVerticle",
		"alvlFourRock",
		"alvlWind",
		"alvlIvyWall",
		"alvlIvyDragon",
		"alvlToCave",
		"blvlCave",
	};

	static public string[] inactiveWindLevel =
	{
		"begin",
		"alvlStrike",
		"alvlResize",
		"alvlMoveVerticle",
		"alvlFourRock",
	};

	static public string[] inactiveZoomLevel =
	{
		"begin",
		"alvlStrike",
	};

	static public float GaussSigma = 2.5f;
	static public int GaussSize = 9;

	static public float[] GaussValue = {
		0.002333f,	0.004054f,	0.006015f,	0.007623f,	0.008249f,	0.007623f,	0.006015f,	0.004054f,	0.002333f,
		0.004054f,	0.007044f,	0.010453f,	0.013247f,	0.014335f,	0.013247f,	0.010453f,	0.007044f,	0.004054f,
		0.006015f,	0.010453f,	0.015512f,	0.019657f,	0.021272f,	0.019657f,	0.015512f,	0.010453f,	0.006015f,
		0.007623f,	0.013247f,	0.019657f,	0.02491f,	0.026956f,	0.02491f,	0.019657f,	0.013247f,	0.007623f,
		0.008249f,	0.014335f,	0.021272f,	0.026956f,	0.02917f,	0.026956f,	0.021272f,	0.014335f,	0.008249f,
		0.007623f,	0.013247f,	0.019657f,	0.02491f,	0.026956f,	0.02491f,	0.019657f,	0.013247f,	0.007623f,
		0.006015f,	0.010453f,	0.015512f,	0.019657f,	0.021272f,	0.019657f,	0.015512f,	0.010453f,	0.006015f,
		0.004054f,	0.007044f,	0.010453f,	0.013247f,	0.014335f,	0.013247f,	0.010453f,	0.007044f,	0.004054f,
		0.002333f,	0.004054f,	0.006015f,	0.007623f,	0.008249f,	0.007623f,	0.006015f,	0.004054f,	0.002333f,
		};

	static public string NextLevel()
	{
		string nowLevel = SceneManager.GetActiveScene().name;
		int i = 0 ;
		while( i < levelNames.Length && levelNames[i] != nowLevel )
			i ++;
		return levelNames[(i+1) % levelNames.Length];

	}

	static public string LastLevel()
	{
		return levelNames[LastLevelIndex()];
	}

	static public int LastLevelIndex()
	{
		string nowLevel = SceneManager.GetActiveScene().name;
		int i = 0 ;
		while( i < levelNames.Length && levelNames[i] != nowLevel )
			i ++;
		return (i-1) % levelNames.Length;
		
	}

	static public string GetLevel( int index )
	{
		return levelNames[index];
	}

	static public string BGM_PATH = "Prefab/System/BGM";
	static public string TRANS_PATH = "Prefab/System/Transform";
	static public string INKSPREAD_PATH = "Prefab/System/InkSpread";

	static public string GRASS_CRASH_SOUND_PATH = "Sound/Grass/GrassCrash2";

	static public Vector3 CAMERA_INIT_POSITION = new Vector3( 0  , 0 , -10f);

	static float m_pixel2Unit = -1f;
	static public float Pixel2Unit
	{
		get {
			if ( m_pixel2Unit > 0 )
				return m_pixel2Unit;
			
			if ( Camera.main != null )
			{
				Vector3 zero = Camera.main.ScreenToWorldPoint( new Vector3( 0 , 0 , 0 ));
				Vector3 one = Camera.main.ScreenToWorldPoint( new Vector3( 1f , 0 , 0 ));
				m_pixel2Unit = ( one.x - zero.x ) / 1f ;
			}
			else
				return 0.01f;
			return m_pixel2Unit;
		}
	}

	static public float zoomFadeTime = 1.33f;

	static public float GetRandomMinMax( MaxMin mm )
	{
		if ( mm.max > mm.min )
			
			return Random.Range( mm.min , mm.max );
		else
			return Random.Range( mm.max , mm.min );
	}

	static public Color DefaultTrailColor = new Color( 0.5f , 0.5f , 0.5f );

	static public Dictionary<string,string> LoadImageDict = new Dictionary<string, string>
	{ 
		{"default" , "cliff&flowerDistance&flowerGrow&button"},
//		{"alvlResize" , "spread" },
//		{"alvlFourRock" , "perspective" },
//		{"alvlWind" , "windArrow" },
//		{"alvlIvyWall" , "windStone" },
	};

	static public string LOADLEVEL_IMG_PATH = "Img/LoadLevel/";
	static public Sprite GetLoadLevelImageInThisLevel()
	{
		string levelName = SceneManager.GetActiveScene().name;
		string imgList = "";

		if ( LoadImageDict.ContainsKey( levelName ) )
		{
			imgList = LoadImageDict[levelName];
		}else
		{
			return null;
		}

		string[] imgs = imgList.Split('&');
		string imgName = imgs[Random.Range( 0 , imgs.Length)];

		string path = LOADLEVEL_IMG_PATH + "loadlevel-" + imgName;
		Texture2D tex = Resources.Load<Texture2D>( path );
		if ( tex != null )
			Debug.Log("tex " + tex.name );
		
		Rect rec = new Rect(0,0,tex.width ,tex.height );
		Sprite res = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);
//		Sprite res = Resources.Load<Sprite>( path );
		Debug.Log( "path " + path );
		Debug.Log( "res " + res );

		return res;
	}
}

[System.SerializableAttribute]
public struct MaxMin<T>{
	public T max;
	public T min;
}

[System.SerializableAttribute]
public struct MaxMinInt{
	public int max;
	public int min;
}

[System.SerializableAttribute]
public struct MaxMin{
	public float max;
	public float min;
}

public enum PetalState
{
	Link,     // the petal on the flower
	Fly,      // fly and be able to grow on land 
	Land,     // No Used
	FlyAway,  // fly away and dead petal
	Init,     // the first petal of the level
	Dead,     // after self destory, set to Dead
	LandGrow, // hit the land and grow
	LandDead, // hit the land and dead
	Keep,    // keep by the monster
	FinalBlow,   // used for final blow on level end
}
[System.SerializableAttribute]
public struct WindSensablParameter
{
	public bool shouldStore;
	public bool shouldUpdate;
}

public enum PetalType
{
	Normal,
	Final,
	Init,
}

[System.Serializable]
public struct PetalInfo
{
	public Vector3 position;
	public PetalType type;
	public Area affectPointArea;
	public Transform parent;
}

[System.Serializable]
public enum CameraState
{
	Disable,
	Free,
	FollowTrans,
	Final,
}

[System.Serializable]
public enum EndingCondition
{
	PointArea,
	Land,
}