using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States{

    public class Jump : State
    {
        private PlayerController controller;

        private bool hasJumped;
        private float coolDown;
        public Jump(PlayerController controller) : base("Idle")
        {
            this.controller = controller;
        }

        public override void Enter(){
            base.Enter();

            hasJumped = false;
            coolDown = 0.5f;

            controller.thisAnimator.SetBool("bJumping", true);

            controller.jumpSound.Play();

        }

        public override void Exit(){
            base.Exit();

            controller.thisAnimator.SetBool("bJumping", false);

            controller.jumpSound.Stop();
            
        }

        public override void Update()
        {
            base.Update();

            coolDown -= Time.deltaTime;

            if(hasJumped && controller.isGrounded && coolDown <= 0){

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

            if(!hasJumped){
                hasJumped = true;
                Impulse();
            }

            Vector3 walkVector = new Vector3(controller.movementVector.x, 0, controller.movementVector.y);
            walkVector = controller.GetForward()* walkVector;
            walkVector *= controller.movementSpeed * controller.jumpMovementFactor;

            controller.thisRigidyBody.AddForce(walkVector, ForceMode.Force);

            controller.RotateBodyToFaceInput();
        }

        public void Impulse(){

            Vector3 forceVector = Vector3.up * controller.jumpPower;
            controller.thisRigidyBody.AddForce(forceVector, ForceMode.Impulse);

        }

        public static implicit operator Jump(Attack v)
        {
            throw new NotImplementedException();
        }
    }

}

