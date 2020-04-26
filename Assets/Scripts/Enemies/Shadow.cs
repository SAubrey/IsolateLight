using UnityEngine;

public class Shadow : Enemy {
    public override void Start() {
        lookDistance = 4;
        combatDistance = 1;
        MAX_HP = 80;
        MAX_STAMINA = 80;
        MAX_VEL = 1f;
        attAnimSpeedMultiplier = .4f;
        base.Start();
    }

    public override void Update() {
        base.Update();
    }
}
