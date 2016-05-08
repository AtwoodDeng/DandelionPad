using UnityEngine;
using System.Collections;

public class PetalAdv : Petal {

    [SerializeField] float mass=1f;
    [SerializeField] float rotationMass=1f;
    [SerializeField] float blowIntense=1f;
    [SerializeField] float drag = 0.01f;
    [SerializeField] float rotationDrag = 0.01f;
    // [SerializeField] float raiseK = 1f;
    [SerializeField] float gravity = 0.1f;
    [SerializeField] float maxVel = 0.1f;
    [SerializeField] float maxRotVel = 0.1f;

    Vector3 myUpDiff;
    public override void Init (Flower _flower, int index) {
        base.Init(_flower, index);
        myUpDiff = Global.V2ToV3 (Global.GetRandomDirection() * 0.33f );
        chaosFunction = StartCoroutine(GenerateChaos(0.1f));
    }

    public override void AddForce (Vector2 force) {
        myForce += force;
    }
    
    public override void Blow (Vector2 vel, BlowType blowType = BlowType.Normal) {
        base.Blow( vel);
		AddForce((vel.normalized + 0.4f * Global.GetRandomDirection()).normalized * vel.magnitude * blowIntense );
    }

    Vector2 myVelocity = Vector2.zero;
    float myRotationVelocity = 0;
    Vector2 myForce = Vector2.zero;

    void LateUpdate()
    {
        UpdateForce();
    }

    protected void UpdateForce()
    {

        // add chaos to force
        myForce += chaosForce;

        // update the velocity
        myVelocity += myForce / mass - new Vector2(0,gravity);

        Vector3 myUp = transform.up + myUpDiff;
        
        //Rotate the petal
        myRotationVelocity = (  Vector3.Cross(myUp, myForce).z 
                                + Vector3.Cross(- myUp , Vector3.down * rotationMass * 0.2f ).z ) / rotationMass;

        //do nothing if the petal is still link to the flower
        if (state == PetalState.Link)
        {
            myVelocity = Vector3.zero;
            myRotationVelocity = 0;
        }

         // control the velocity
        if (myVelocity.magnitude > maxVel)
            myVelocity = myVelocity.normalized * maxVel;

        // update the position and rotation
        myRotationVelocity = Mathf.Clamp(myRotationVelocity, maxRotVel, -maxRotVel);
        transform.position += Global.V2ToV3(myVelocity);
        transform.Rotate(Vector3.back, myRotationVelocity);

        // drag the velocity
        myVelocity *= ( 1f - drag );
        myRotationVelocity *= (1f - rotationDrag);

        // set force back to zero
        myForce = Vector2.zero;

    }

    Vector2 chaosForce;
    Coroutine chaosFunction;
    // update the chaos(this will run forever)
    IEnumerator GenerateChaos(float time)
    {
        float timer = 0;
        chaosForce = Global.GetRandomDirection() * myForce.magnitude * Random.Range(0.1f, 1f);
        while(true)
        {
            chaosForce = (chaosForce.normalized + Global.GetRandomDirection().normalized * 0.1f) * chaosForce.magnitude;

            timer += Time.deltaTime;
            if (timer > time)
                break;
            yield return null;
        }
        chaosFunction = StartCoroutine(GenerateChaos(Random.Range(.5f, .8f)));
    }

    protected override void SelfDestory () {
        base.SelfDestory();
        StopCoroutine(chaosFunction);
    }
}
