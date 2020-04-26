using UnityEngine;
//using UnityEngine.AI;

public abstract class Enemy : Body {
    protected GameObject playerObj;
    protected Player player;
    protected float lookDistance = 4;
    protected float alertDistance = 3;
    protected float combatDistance = 1;
    protected float distanceFromPlayer = 0;
    protected float confidence = 1f; // Chance to 0 (run away) - 1 (pursue).

    public virtual void Start() {
        Init();
        _isEnemy = true;
        playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Player>();
        SizeHealthbar(healthbar.maxValue);
        SizeStambar(staminaBar.maxValue);
        alertDistance = player.light2d.pointLightOuterRadius;
        
        if (!player)
            Die();
    }
    
    public override void Update() {
        if (player.alive) {
            base.Update();
        }
    }

    void FixedUpdate() {
        if (player.alive) {
            if (stunned) 
                return;
            CheckZone();
        }
    }

    private void CheckZone() {
        distanceFromPlayer = GetDistanceFromPlayer();
        if (distanceFromPlayer <= combatDistance) {
            CombatUpdate();
        } else if (distanceFromPlayer <= alertDistance) {
            AlertUpdate();
        } else if (distanceFromPlayer <= lookDistance) {
            LookUpdate();
        }
    }

    protected virtual void CombatUpdate() {
        // Strategy: Defend before attacking
        if (!FacingPlayer() && !attacking) {
            RotateToPlayer();
        }
        if (player.GetSword().swinging) {
            Dash(-GetDirectionToPlayer());
        } else if (stamina >= sword.swingCost) {
            Attack();
        }
    }

    private void AlertUpdate() {
        RotateToPlayer();

        if (FacingPlayer()) {
            Move(GetDirectionToPlayer(), movementForce, MAX_VEL);
        }
    }

    private void LookUpdate() {
        RotateToPlayer();
    }

    public override void TakeDamage(int dmg) {
        base.TakeDamage(dmg);
        if (invulnerable)
            return;
        
        if (hp <= 0) {
            Die();
        }
        Stun(timers.normalStunTime);
    }

    protected float GetDistanceFromPlayer() {
        return Vector3.Distance(playerObj.transform.position, transform.position);
    }

    protected Vector2 GetDirectionToPlayer() {
        return StaticOperations.TargetUnitVec(body.position, player.body.position);
    }

    protected bool FacingPlayer() {
        // Bit shift the index of the layer (8) to get a bit mask
        //int layerMask = 1 << 8;
        
        //RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        //transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
        var hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.up));
        if (hit.collider) {
            
                //transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity)) {
            Debug.DrawRay(transform.position, 
                transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            return true;
        } else {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 100, Color.white);
            //Debug.Log("Did not Hit");
        }
        return false;
    }

    private void RotateToPlayer() {
        RotateTowards(new Vector2(playerObj.transform.position.x,
            playerObj.transform.position.y));
    }

    protected float DetermineCombatDistance() {
        return 0;
    }

    private void Die() {
        Destroy(gameObject, 0);
    }

}
