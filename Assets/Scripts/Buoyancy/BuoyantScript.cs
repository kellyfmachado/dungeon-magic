using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuoyantScript : MonoBehaviour
{
    public PlayerController controller;
    public float underwaterDrag = 3f;
    public float underwaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;

    public float buoyantForce = 10;
    
    private Rigidbody thisRigidyBody;

    private bool hasTouchedWater;
    
    void Awake()
    {
        thisRigidyBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float diffY = transform.position.y;
        bool isUnderwater = diffY < 0;

        if(isUnderwater){
            hasTouchedWater = true;
        }

        if(!hasTouchedWater){
            return;
        }

        if(isUnderwater){
            Vector3 vector = Vector3.up * buoyantForce * -diffY;
            thisRigidyBody.AddForce(vector, ForceMode.Acceleration);
        }

        thisRigidyBody.drag = isUnderwater? underwaterDrag : airDrag;
        thisRigidyBody.angularDrag = isUnderwater? underwaterAngularDrag : airAngularDrag;

        controller.stateMachine.ChangeState(controller.deadState);  

    }
}
