using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Petal3DWind : MonoBehaviour , WindSensable {

    [SerializeField] float WindSense = 0.001f;
    [SerializeField] Petal3D parent;

    Vector2 windVelocity;
    public void SenseWind(Vector2 velocity)
    {
        windVelocity = velocity;
    }

    public void ExitWind(){
        windVelocity = Vector2.zero;
    }

    public Vector3 getPosition()
    {
        if ( parent == null )
            return transform.position;
        return parent.transform.position;
    }

    [SerializeField]public WindSensablParameter windSensablParameter;

    public WindSensablParameter getParameter()
    {
        return windSensablParameter;
    }
	
	void Awake()
	{
		if ( parent == null )
		if (transform.parent != null )
			parent = transform.parent.GetComponent<Petal3D>();
        WindSense *= Random.Range(0.5f, 2f);
	}
    void Update()
    {
        DealwithWind();
    }

    protected void DealwithWind()
    {
        if (windVelocity != Vector2.zero )
        {

            Vector2 windForce = windVelocity * WindSense 
                * (1f + Mathf.Abs( Vector3.Dot(transform.up, Global.V2ToV3(windVelocity.normalized)) ) );
            // Debug.Log("add wind force " + windForce + " " + parent.state );
            parent.AddForce(windForce);
        }
    }

}
