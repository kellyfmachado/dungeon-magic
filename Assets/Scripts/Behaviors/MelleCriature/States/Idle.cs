using UnityEngine;

namespace Behaviors.MeleeCriature.States {

    public class Idle : State {

        private MeleeCriatureController controller;
        private MeleeCriatureHelper helper;

        private float searchCooldown;
        public Idle(MeleeCriatureController controller) : base("Idle"){
            this.controller = controller;
            this.helper = controller.helper;
        }

        public override void Enter()
        {
            base.Enter();

            searchCooldown = controller.targetSearchInterval;
        }
        
        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();

            if(GameManager.Instance.isGameOver) return;

            searchCooldown -= Time.deltaTime;
            if(searchCooldown <= 0){
                searchCooldown = controller.targetSearchInterval;

                if(helper.IsPlayerOnSight()){
                    controller.stateMachine.ChangeState(controller.followState);
                    return;
                }
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
