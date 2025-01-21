using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Behaviors.MeleeCriature.States {

    public class Attack : State {

        private MeleeCriatureController controller;
        private MeleeCriatureHelper helper;
        private float endAttackCooldown;
        private IEnumerator attackCoroutine;

        public Attack(MeleeCriatureController controller) : base("Attack"){
            this.controller = controller;
            this.helper = controller.helper;
        }

        public override void Enter()
        {
            base.Enter();

            if(controller.hitMagicBeholder != null){
                var hitPosition = new Vector3(controller.transform.position.x, controller.transform.position.y+2,controller.transform.position.z);
                var hitRotation = controller.transform.rotation;
                var hitMagicEffect = Object.Instantiate(controller.hitMagicBeholder, hitPosition, hitRotation);
                hitMagicEffect.transform.SetParent(controller.transform);
            }

            endAttackCooldown = controller.attackDuration;

            attackCoroutine = ScheduleAttack();
            controller.StartCoroutine(attackCoroutine);

            controller.thisAnimator.SetTrigger("tAttack");
        }
        
        public override void Exit()
        {
            base.Exit();

            if(attackCoroutine != null){
                controller.StopCoroutine(attackCoroutine);
            }
        }

        public override void Update()
        {
            base.Update();

            helper.FacePlayer();

            if((endAttackCooldown -= Time.deltaTime) <= 0f){
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
        }

        private IEnumerator ScheduleAttack(){
            yield return new WaitForSeconds(controller.damageDelay);
            PerformAttack();
        }

        private void PerformAttack(){

            var origin = controller.transform.position;
            var direction = controller.transform.rotation * Vector3.forward;
            var radius = controller.attackRadius;
            var maxDistance = controller.attackSphereRadius;

            var attackPosition = direction * radius + origin;
            var layerMask = LayerMask.GetMask("Player");
            Collider[] colliders = Physics.OverlapSphere(attackPosition, maxDistance, layerMask);
            foreach(var collider in colliders){
                var hitObject = collider.gameObject;
                    
                var hitLifeScript = hitObject.GetComponent<LifeScript>();
                if (hitLifeScript != null){
                    var attacker = controller.gameObject;
                    var attackDamage = controller.attackDamage;
                    hitLifeScript.InflictDamage(attacker, attackDamage);
                }

            }

        }

    }   

}
