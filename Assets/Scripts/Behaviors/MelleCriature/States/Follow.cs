using UnityEngine;

namespace Behaviors.MeleeCriature.States {

    public class Follow : State {

        private MeleeCriatureController controller;
        private MeleeCriatureHelper helper;

        private readonly float updateInterval = 1;        
        private float updateCooldown;
        private float ceaseForPlayerCooldown;  

        public Follow(MeleeCriatureController controller) : base("Follow"){
            this.controller = controller;
            this.helper = controller.helper;
        }

        public override void Enter()
        {
            base.Enter();

            controller.stepSound.Play();

            updateCooldown = 0;
            ceaseForPlayerCooldown = controller.ceaseFollowInterval;
        }
        
        public override void Exit()
        {
            base.Exit();

            controller.stepSound.Stop();

            controller.thisAgent.ResetPath();
        }

        public override void Update()
        {
            base.Update();

            if((updateCooldown -= Time.deltaTime) <= 0f){
                updateCooldown = updateInterval;
                var player = GameManager.Instance.player;
                var playerPosition = player.transform.position;
                controller.thisAgent.SetDestination(playerPosition);
            }

            if((ceaseForPlayerCooldown -= Time.deltaTime) <= 0f){
                if(!helper.IsPlayerOnSight()){
                    controller.stateMachine.ChangeState(controller.idleState);
                    return;
                }
            }

            if(helper.GetDistanceToPlayer() <= controller.distanceToAttack){
                controller.stateMachine.ChangeState(controller.attackState);
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

    }   

}
