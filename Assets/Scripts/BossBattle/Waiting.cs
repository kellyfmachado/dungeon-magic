using System;
using UnityEngine;

namespace BossBattle {
    public class Waiting  : State {

        public Waiting() : base("Waiting") {
        }

        public override void Enter() {
            base.Enter();
        }

        public override void Exit() {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();

            var currentIntensity = GameManager.Instance.directionalLightBossRoom.intensity;
            GameManager.Instance.directionalLightBossRoom.intensity = Mathf.Lerp(currentIntensity, 1, 0.1f);
        }

    }
}