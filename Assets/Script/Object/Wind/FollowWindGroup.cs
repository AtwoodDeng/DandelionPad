using UnityEngine;
using System.Collections;

public class FollowWindGroup : FollowWind {

	[SerializeField] GameObject[] objectList;

	AngleData[] datas;

	bool isAttachToScale = true;

	struct AngleData
	{
		public float initAngle;
		public float angleVol;
		public GameObject obj;

		public float swind; // sense of wind
		public float K ;    // 
	}

	override public void Init () {
		base.Init();
		datas = new AngleData[ objectList.Length ]; 
		for( int i= 0 ; i < objectList.Length ; ++ i )
		{
			datas[i].obj = objectList[i];
			datas[i].initAngle = Global.StandardizeAngle( datas[i].obj.transform.eulerAngles.z );
			datas[i].angleVol = 0;

			if (isAttachToScale ) {
				datas[i].swind = swind * datas[i].obj.transform.localScale.x;
				datas[i].K = K * datas[i].obj.transform.localScale.x;
			}else
			{
				datas[i].swind = swind * Random.Range(0.5f, 2f);
				datas[i].K = K * Random.Range(0.5f, 2f);
			}
		}
    }


	protected override void UpdateObject (float dt ) {
		
		float edt = dt * LogicManager.PhysTimeRate;
	    float windForce = 0;

        //test if in the wind
        if ( windVelocity != Vector2.zero)
        {
       		Vector3 windDirect = Global.V2ToV3(windVelocity.normalized);
       		float windEffect = Vector3.Dot(Vector3.back , Vector3.Cross(transform.up, windDirect) );
       		windForce = windEffect * windVelocity.magnitude;
        }

		for( int i = 0 ; i < datas.Length ; ++ i )
		{
	        float offSetAngle = Mathf.DeltaAngle(transform.eulerAngles.z, datas[i].initAngle);
	        
	        float swindForce = - offSetAngle * datas[i].K;

			datas[i].angleVol += ( windForce * datas[i].swind + swindForce) * edt;
	        datas[i].angleVol *= drag;

	        //Rotate the object
			datas[i].obj.transform.Rotate(Vector3.back, datas[i].angleVol * edt );
		}
    }
}
