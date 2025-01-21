using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : State
{
    private PlayerController controller;
    public Walking(PlayerController controller) : base("Walking")
    {
        this.controller = controller;
    }

    public override void Enter()
    {
        base.Enter();

        controller.stepSound.Play();
    }

    public override void Exit()
    {
        base.Exit();

        controller.stepSound.Stop();
    }

    public override void Update()
    {
        base.Update();

        if(controller.AttemptToAttack()){
            return;
        }

        if(controller.hasDefenseInput){
            controller.stateMachine.ChangeState(controller.defendState);
            return;
        }

        if(controller.hasJumpInput){
            controller.stateMachine.ChangeState(controller.jumpState);
            return;
        }   

        if(controller.movementVector.IsZero()){
            controller.stateMachine.ChangeState(controller.idleState);
            return;
        }

    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 walkVector = new Vector3(controller.movementVector.x, 0, controller.movementVector.y);
        walkVector = controller.GetForward()* walkVector;
        walkVector = Vector3.ProjectOnPlane(walkVector, controller.slopeNormal);
        walkVector *= controller.movementSpeed;

        controller.thisRigidyBody.AddForce(walkVector, ForceMode.Force);

        controller.RotateBodyToFaceInput();
    }
}
