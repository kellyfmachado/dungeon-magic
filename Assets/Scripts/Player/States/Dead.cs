using EventArgs;
using UnityEngine;

namespace Player.States {
    public class Dead : State {

        private PlayerController controller;

        public Dead(PlayerController controller) : base("Dead") {
            this.controller = controller;
        }

        public override void Enter() {
            base.Enter();
            controller.thisAnimator.SetTrigger("tGameOver");
            
            // Make player invulnerable
            controller.thisLife.isVulnerable = false;

            GameManager.Instance.gameplayUI.gameObject.SetActive(false);
            GameManager.Instance.bossMusic.Stop();
            GameManager.Instance.ambienceMusic.Stop();
            GameManager.Instance.gameplayMusic.Stop();
            
            // Game over =(
            GlobalEvents.Instance.InvokeGameOver(this, new GameOverArgs());
        }

        public override void Exit() {
            base.Exit();
        }

        public override void Update() {
            base.Update();
        }

        public override void LateUpdate() {
            base.LateUpdate();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
        }

    }
}