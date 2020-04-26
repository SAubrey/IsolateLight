using UnityEngine;

public class ShadowCaptain : Enemy {
    public override void Start() {
        lookDistance = 5;
        combatDistance = 1.2f;
        MAX_HP = 100;
        MAX_STAMINA = 100;
        MAX_VEL = 2f;
        base.Start();
    }

    public override void Update() {
        base.Update();
    }
}
