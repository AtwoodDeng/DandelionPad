using UnityEngine;
using System.Collections;

public class PetalBasic : Petal {

	[SerializeField] Joint2D[] joints;
	[SerializeField] Collider2D m_collider;

	[SerializeField] float blowIntense;

	public Rigidbody2D top2D;
	public Rigidbody2D root;

	// public void SetConnectBody(Rigidbody2D source)
	// {
	// 	joints = GetComponents<Joint2D>();

	// 	foreach(Joint2D j in joints)
	// 		j.connectedBody = source;
	// }

	void RemoveConnectBody()
	{
		// SetConnectBody(null);
		// joints = GetComponents<Joint2D>();
		// foreach(Joint2D j in joints)
		// 	j.enabled = false;

		top2D.isKinematic = false;
		root.isKinematic = false;

		m_collider.isTrigger = false;
	}

 //    void OnCollisionEnter2D(Collision2D coll)
	// {
	// 	if (state != PetalState.Fly)
	// 		return;
	// 	if(coll.gameObject.tag == "Green")
	// 	{
	// 		Vector3 contactPoint = new Vector3( coll.contacts[0].point.x , coll.contacts[0].point.y , 0 );
	// 		GrowFlowerOn(contactPoint);
	// 		Destroy(this.gameObject);
	// 	}
	// }

	override public void AddForce(Vector2 force)
	{
		top2D.AddForce(force, ForceMode2D.Impulse);
	}

	override public void Blow(Vector2 vel, BlowType blowType = BlowType.Normal)
	{
		base.Blow(vel);
		RemoveConnectBody();
		top2D.AddForce( ( vel.normalized + Global.GetRandomDirection() * 0.1f ).normalized * vel.magnitude * blowIntense, ForceMode2D.Impulse);
	}

	public override void  Init (Flower _flower , int index )
	{
		base.Init(_flower,index);


		if (m_collider == null )
			m_collider = GetComponent<Collider2D>();
		m_collider.isTrigger = true;

		float massRate = Random.Range(0.5f, 1.2f);
		top2D.mass *= massRate;
		root.mass *= massRate;
    }
}
