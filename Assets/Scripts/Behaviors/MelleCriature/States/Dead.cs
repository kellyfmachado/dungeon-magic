using UnityEngine;

namespace Behaviors.MeleeCriature.States {

    public class Dead : State {

        private MeleeCriatureController controller;
        private MeleeCriatureHelper helper;
        public Dead(MeleeCriatureController controller) : base("Dead"){
            this.controller = controller;
            this.helper = controller.helper;
        }

        public override void Enter()
        {
            base.Enter();

            controller.thisLife.isVulnerable = false;

            controller.thisAnimator.SetTrigger("tDead");

            controller.thisCollider.enabled = false;

            // Create knockout effect
            var knockoutEffect = controller.knockoutEffect;
            if (knockoutEffect != null) {
                var position = controller.transform.position;
                var rotation = knockoutEffect.transform.rotation;
                Object.Instantiate(knockoutEffect, position, rotation);
            }
        }
        
        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();

            var distanceToPlayer = helper.GetDistanceToPlayer();
            if(distanceToPlayer >= controller.destroyIfFar){
                Object.Destroy(controller.gameObject);
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

    }   

}
